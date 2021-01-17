using PointBlank.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

/* 
    [PORTUGUESE BRAZIL]
    Este código-fonte é de autoria da <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ é um software privado: você não pode usar,
    redistribuir e/ou modificar sem autorização.

    Você deveria ter recebido uma autorização com esse software/codigo-fonte
    para ter cópias, se você não recebeu, encerre/exclua imediatamente 
    todos os vestígios do mesmo para não ter futuros problemas.

    Atenciosamente, <BR.UZ/>, MomzGames, ExploitTeam.
*/

/*
    [INGLISH USA]
    This source code is written by <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ a private software: you will not be able to use it,
    redistribute it and / or modify without authorization.

    You should have received an authorization with this software/source code
    to have copies, if you did not receive it, immediately shutdown/exclude
    all traces of it so that you have no future problems.

    Sincerely, <BR.UZ/>, MomzGames, ExploitTeam.
*/

namespace PointBlank
{
    /* INFOS
     * UniqueRoomId é uma máscara que consiste em ServerId, ChannelId, RoomId
     * UniqueRoomId = (serverId & 0x00000000ff) << 20 | (channelId & 0x00000000ff) << 12 | roomId & 0x0000000fff;
     * 
     * //var ServerId = (int)((UniqueRoomId >> 20) & 0xfff);
     */
    public class BattleHandler
    {
        public byte Opcode;
        public byte SlotId;
        public byte Round;
        public ushort Length;
        public byte AccountId;
        public byte Unk1;
        public byte RespawnNumber;
        public byte RoundNumber;
        public float Time;
        public DateTime LastReceive;

        public BattleHandler(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.LastOperation != SocketAsyncOperation.ReceiveFrom)
                {
                    return;
                }
                if (e.SocketError != SocketError.Success)
                {
                    if ((DateTime.Now - Logger.LastSaveLogUdpBattle1).Minutes >= 1)
                    {
                        Logger.Error(" [BattleManager] [EventArgs_Completed] SocketError: " + e.SocketError);
                        Logger.LastSaveLogUdpBattle1 = DateTime.Now;
                    }
                    return;
                }
                IPEndPoint remoteEndPoint = (IPEndPoint)e.RemoteEndPoint;
                byte[] buffer = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, 0, buffer, 0, buffer.Length);
                if (buffer.Length < 22 || buffer.Length > 2048)
                {
                    if ((DateTime.Now - Logger.LastSaveLogUdpBattle2).Minutes >= 1)
                    {
                        Logger.Attacks($" [Battle] Buffer inválid [!] Length: {buffer.Length} IP: {remoteEndPoint}");
                        //FirewallSecurity.RemoveRuleUdp(remoteEndPoint.Address.ToString());
                        Logger.LastSaveLogUdpBattle2 = DateTime.Now;
                    }
                    return;
                }

                MemoryStream stream = new MemoryStream(buffer);
                BinaryReader reader = new BinaryReader(stream);
                Opcode = reader.ReadByte();
                SlotId = reader.ReadByte();
                Time = reader.ReadSingle();
                Round = reader.ReadByte(); //Session?
                Length = reader.ReadUInt16();
                RespawnNumber = reader.ReadByte(); //Respawn atual
                RoundNumber = reader.ReadByte(); //Round ou passagens no portal
                AccountId = reader.ReadByte();
                Unk1 = reader.ReadByte();

                stream.Close();
                reader.Close();

                if (Opcode < 3 || Opcode > 132 || SlotId > 15 || Length != buffer.Length)
                {
                    if ((DateTime.Now - Logger.LastSaveLogUdpBattle3).Minutes >= 1)
                    {
                        Logger.Attacks($" [Battle] Inválid Informations [!] AccountId: {AccountId} Time: {Time} SlotId: {SlotId} LengthCompare: {Length}/{buffer.Length} EndPoint: {remoteEndPoint}");
                        //FirewallSecurity.RemoveRuleUdp(remoteEndPoint.Address.ToString());
                        Logger.LastSaveLogUdpBattle3 = DateTime.Now;
                    }
                    return;
                }

                Logger.Battle($"\n Address: {remoteEndPoint}\n Opcode: {Opcode}\n Slot: {SlotId}\n Time: {Time}" +
                              $"\n Length: {Length}\n Round: {Round}\n Respawns: {RespawnNumber}\n RoundNumber: {RoundNumber}" +
                              $"\n AccountId: {AccountId}\n Unknown: {Unk1}\n");

                LastReceive = DateTime.Now;
                Socket mainSocket = (Socket)e.UserToken;

                byte[] result = new byte[buffer.Length - 13];
                Array.Copy(buffer, 13, result, 0, result.Length);
                byte[] WithEndData = Decrypt(result, Length % 6 + 1);
                byte[] NoEndData = new byte[WithEndData.Length - 9];
                Array.Copy(WithEndData, NoEndData, NoEndData.Length);

                stream = new MemoryStream(WithEndData);
                reader = new BinaryReader(stream);

                if (Opcode == 3) //3 PVP
                {
                    reader.BaseStream.Position += NoEndData.Length;
                    var UniqueRoomId = reader.ReadInt32();
                    var DedicationSlot = reader.ReadByte(); //PVP: 0
                    var RoomSeed = reader.ReadInt32();
                    var ChannelId = (UniqueRoomId >> 12) & 0xff;
                    var RoomId = UniqueRoomId & 0xfff;

                    Channel channel = ServersManager.GetChannel(ChannelId);
                    Room room = channel?.GetRoom(RoomId);
                    if (room == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_3] Room null! Id: {RoomId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.sessionPort == 0)
                    {
                        Logger.Warning($" [Battle] [REQUEST_3] Session port invalid! Id: {RoomId} SessionPort: {room.sessionPort} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.roomSeed != RoomSeed)
                    {
                        Logger.Warning($" [Battle] [REQUEST_3] Seed invalid! RoomSeed: {room.roomSeed} Seed: {RoomSeed} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    Slot slot = room.GetPlayer(SlotId, remoteEndPoint);
                    if (slot == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_3] Player null! SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                        return;
                    }

                    DateTime now = DateTime.Now;
                    int secs = (now - slot.lastTick).Seconds;
                    double totalsecs = (now - slot.lastTick).TotalSeconds;
                    double totalmilisecs = (now - slot.lastTick).TotalMilliseconds;
                    if (secs >= 1)
                    {
                        Logger.Warning($" TICKS: {slot.TICKS} Secs: {secs} Seconds: {totalsecs} Slot: {slot.Id} IP: {(IPEndPoint)e.RemoteEndPoint}");
                        slot.TICKS = 0;
                    }
                    else
                    {
                        slot.TICKS++;
                    }
                    slot.lastTick = now;

                    if (!slot.AccountIdIsValid(AccountId))
                    {
                        Logger.Warning($" [Battle] [REQUEST_3] AccountId invalid! AccountId: {AccountId} PlayerIdRegister: {slot.PlayerIdRegister} SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                    }
                    slot.PartidaTime = Time;
                    if (room.StartTime == new DateTime())
                    {
                        return;
                    }
                    if (room.IsCase4BotMode)
                    {
                        Logger.Battle($" [Battle] [REQUEST_3] [PVP in BOT] DedicationSlot: {DedicationSlot} PacketSlot: {SlotId} [{slot.client}]");
                        byte[] Actions = WriteDataBot(NoEndData, slot, room);
                        byte[] Code = WriteCode(4, Actions, SlotId, room.GetStartTime(), Round);
                        for (int slotIndex = 0; slotIndex < 16; slotIndex++)
                        {
                            Slot playerRoom = room.slots[slotIndex];
                            if (playerRoom.client != null && slotIndex != SlotId)
                            {
                                SendPacket(mainSocket, Code, playerRoom.client);
                            }
                        }
                    }
                    else
                    {
                        if (DedicationSlot == 255)
                        {
                            Logger.Warning($" [Battle] [REQUEST_3] DedicationSlot invalid in PVP! DedicationSlot: {DedicationSlot} EndPoint: {remoteEndPoint}");
                        }
                        byte[] Actions = WriteDataPlayer(NoEndData, slot, room, slot.GetSlotTime());
                        byte[] Code = WriteCode(4, Actions, byte.MaxValue, room.GetStartTime(), Round);
                        for (int slotIndex = 0; slotIndex < 16; slotIndex++)
                        {
                            Slot playerRoom = room.slots[slotIndex];
                            if (playerRoom.client != null)
                            {
                                SendPacket(mainSocket, Code, playerRoom.client);
                            }
                        }
                    }
                }
                else if (Opcode == 4) //4 IA
                {
                    reader.BaseStream.Position += NoEndData.Length;
                    var UniqueRoomId = reader.ReadInt32();
                    var DedicationSlot = reader.ReadByte(); //255=HOST 0=PLAYERS
                    var RoomSeed = reader.ReadInt32();
                    var ChannelId = (UniqueRoomId >> 12) & 0xff;
                    var RoomId = UniqueRoomId & 0xfff;

                    Channel channel = ServersManager.GetChannel(ChannelId);
                    Room room = channel?.GetRoom(RoomId);
                    if (room == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_4] Room null! Id: {RoomId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.sessionPort == 0)
                    {
                        Logger.Warning($" [Battle] [REQUEST_4] Session port invalid! Id: {RoomId} SessionPort: {room.sessionPort} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.roomSeed != RoomSeed)
                    {
                        Logger.Warning($" [Battle] [REQUEST_4] Seed invalid! RoomSeed: {room.roomSeed} Seed: {RoomSeed} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    Slot slot = room.GetPlayer(SlotId, remoteEndPoint);
                    if (slot == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_4] Player null! SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (!slot.AccountIdIsValid(AccountId))
                    {
                        Logger.Warning($" [Battle] [REQUEST_4] AccountId invalid! AccountId: {AccountId} PlayerIdRegister: {slot.PlayerIdRegister} SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                    }
                    room.IsCase4BotMode = true;
                    if (room.StartTime == new DateTime())
                    {
                        return;
                    }
                    byte[] Actions = WriteDataBot(NoEndData, slot, room);
                    byte[] Code = WriteCode(4, Actions, SlotId, slot.GetSlotTime(), Round);
                    for (int slotIndex = 0; slotIndex < 16; slotIndex++)
                    {
                        Slot playerRoom = room.slots[slotIndex];
                        if (playerRoom.client != null && slotIndex != SlotId)
                        {
                            SendPacket(mainSocket, Code, playerRoom.client);
                        }
                    }
                }
                else if (Opcode == 65)  //65 REGISTER
                {
                    /*
                     * OBTÉM UMA SALA, ADICIONA E ATUALIZA O JOGADOR
                     */
                    var UdpVersion = $"{reader.ReadInt32()}.{reader.ReadInt32()}";
                    var UniqueRoomId = reader.ReadInt32();
                    var RoomSeed = reader.ReadInt32();
                    var DedicationSlot = reader.ReadByte(); //Slot Real do Jogador na Sala [0-15]
                    var ChannelId = (UniqueRoomId >> 12) & 0xff;
                    var RoomId = UniqueRoomId & 0xfff;

                    if (Settings.UdpVersion != UdpVersion)
                    {
                        Logger.Warning($" [Battle] [REQUEST_65] Udp Version invalid. ({Settings.UdpVersion}/{UdpVersion})");
                        return;
                    }
                    if (DedicationSlot != SlotId)
                    {
                        Logger.Warning($" [Battle] [REQUEST_65] Slot invalid. DedicationSlot: {DedicationSlot} SlotId: {SlotId}");
                        return;
                    }

                    Channel channel = ServersManager.GetChannel(ChannelId);
                    Room room = channel?.GetRoom(RoomId);
                    if (room == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_65] Room null! Id: {RoomId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.sessionPort == 0)
                    {
                        Logger.Warning($" [Battle] [REQUEST_65] Session port invalid! Id: {RoomId} SessionPort: {room.sessionPort} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.roomSeed != RoomSeed)
                    {
                        Logger.Warning($" [Battle] [REQUEST_65] Seed invalid! RoomSeed: {room.roomSeed} Seed: {RoomSeed} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    int MapId = RoomSeed >> 4;
                    int Mode = RoomSeed & 15;
                    if (room.mapId != MapId)
                    {
                        Logger.Warning($" [Battle] [REQUEST_65] MapId invalid! RoomMapId: {room.mapId} MapId: {MapId}");
                        return;
                    }
                    if (room.mode != (RoomTypeEnum)Mode)
                    {
                        Logger.Warning($" [Battle] [REQUEST_65] Mode invalid! RoomMode: {room.mode} Mode: {Mode}");
                        return;
                    }
                    Slot slot = room.GetSlot(DedicationSlot);
                    if (slot != null && !slot.CompareIP(remoteEndPoint) && DedicationSlot == SlotId)
                    {
                        byte accountId = BitConverter.GetBytes(slot.playerId)[0];
                        if (accountId == AccountId)
                        {
                            if (!BattleManager.SocketConnections[room.sessionPort - Settings.PortBattle].TryAdd(remoteEndPoint.ToString(), room.sessionPort))
                            {
                                Logger.Warning($" NÃO FOI POSSIVEL ADICIONAR NA LISTA DE SESSOES DO UDP. sessionPort: {room.sessionPort} remoteEndPoint: {remoteEndPoint}");
                            }
                            slot.client = remoteEndPoint;
                            slot.PlayerIdRegister = AccountId;
                            slot.pingDate = LastReceive;
                            slot.ResetBattleInfos();
                            SendPacket(mainSocket, PackageDataManager.Packet66CreatorBuffer, slot.client);
                            Logger.White($" [Battle] [REQUEST_65] Player connected. UdpVersion: {UdpVersion} UniqueRoomId: {UniqueRoomId} RoomSeed: {RoomSeed} DedicationSlot: {DedicationSlot} PacketSlot: {SlotId} [{slot.client.Address}:{slot.client.Port}]");
                        }
                        else
                        {
                            Logger.Warning($" [Battle] [REQUEST_65] AccountId invalid! SlotAccountId: {accountId} AccountId: {AccountId}");
                        }
                    }
                    else
                    {
                        Logger.Warning($" [Battle] [REQUEST_65] Player null! SlotId: {SlotId} DedicationSlot: {DedicationSlot} SlotIP: {slot.client} EndPoint: {remoteEndPoint}");
                    }
                }
                else if (Opcode == 67) //67 UNREGISTER
                {
                    /*
                     * DESCONECTA O JOGADOR DA BATALHA
                     */
                    var Unknown = $"{reader.ReadInt32()}.{reader.ReadInt32()}"; //Alguma info da sala pois todos os players recebem os mesmos valores.
                    var UniqueRoomId = reader.ReadInt32();
                    var RoomSeed = reader.ReadInt32();
                    var DedicationSlot = reader.ReadByte(); //Slot Real do Jogador na Sala [0-15]
                    var ChannelId = (UniqueRoomId >> 12) & 0xff;
                    var RoomId = UniqueRoomId & 0xfff;

                    if (DedicationSlot != SlotId)
                    {
                        Logger.Warning($" [Battle] [REQUEST_67] Slot invalid. DedicationSlot: {DedicationSlot} SlotId: {SlotId}");
                        return;
                    }

                    Channel channel = ServersManager.GetChannel(ChannelId);
                    Room room = channel?.GetRoom(RoomId);
                    if (room == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_67] Room null! Id: {RoomId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.sessionPort == 0)
                    {
                        Logger.Warning($" [Battle] [REQUEST_67] Session port invalid! Id: {RoomId} SessionPort: {room.sessionPort} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.roomSeed != RoomSeed)
                    {
                        Logger.Warning($" [Battle] [REQUEST_67] Seed inválid! RoomSeed: {room.roomSeed} Seed: {RoomSeed} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    Slot slot = room.GetPlayer(DedicationSlot, remoteEndPoint);
                    if (slot == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_67] Player null! DedicationSlot: {DedicationSlot} SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (!BattleManager.SocketConnections[room.sessionPort - Settings.PortBattle].TryRemove(remoteEndPoint.ToString(), out int sessionPort))
                    {
                        Logger.Warning($" NÃO FOI POSSIVEL REMOVER NA LISTA DE SESSOES DO UDP. sessionPort: {room.sessionPort}/{sessionPort} remoteEndPoint: {remoteEndPoint}");
                    }
                    if (!slot.AccountIdIsValid(AccountId))
                    {
                        Logger.Warning($" [Battle] [REQUEST_67] AccountId invalid! AccountId: {AccountId} PlayerIdRegister: {slot.PlayerIdRegister} SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                    }
                    slot.ResetAllInfos();
                    Logger.White($" [Battle] [REQUEST_67] Player disconnected. Unknown: {Unknown} UniqueRoomId: {UniqueRoomId} RoomSeed: {RoomSeed} DedicationSlot: {DedicationSlot} PacketSlot: {SlotId} [{remoteEndPoint.Address}:{remoteEndPoint.Port}]");
                }
                else if (Opcode == 97) //97 ALL
                {
                    var UniqueRoomId = reader.ReadInt32();
                    var DedicationSlot = reader.ReadByte(); //255 no PVP
                    var RoomSeed = reader.ReadInt32();
                    var ChannelId = (UniqueRoomId >> 12) & 0xff;
                    var RoomId = UniqueRoomId & 0xfff;

                    Channel channel = ServersManager.GetChannel(ChannelId);
                    Room room = channel?.GetRoom(RoomId);
                    if (room == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_97] Room null! Id: {RoomId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.sessionPort == 0)
                    {
                        Logger.Warning($" [Battle] [REQUEST_97] Session port invalid! Id: {RoomId} SessionPort: {room.sessionPort} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.roomSeed != RoomSeed)
                    {
                        Logger.Warning($" [Battle] [REQUEST_97] Seed invalid! RoomSeed: {room.roomSeed} Seed: {RoomSeed} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    Slot slot = room.GetPlayer(remoteEndPoint);
                    if (slot == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_97] Player null! SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (!slot.AccountIdIsValid(AccountId))
                    {
                        Logger.Warning($" [Battle] [REQUEST_97] AccountId invalid! AccountId: {AccountId} PlayerIdRegister: {slot.PlayerIdRegister} SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                    }
                    slot.pingDate = DateTime.Now;
                    SendPacket(mainSocket, buffer, remoteEndPoint);
                    Logger.Battle($" CASE 97. UniqueRoomId: {UniqueRoomId} RoomSeed: {RoomSeed} DedicationSlot: {DedicationSlot} PacketSlot: {SlotId} [{remoteEndPoint.Address}:{remoteEndPoint.Port}]");
                }
                else if (Opcode == 131) //131 IA BY PLAYER
                {
                    reader.BaseStream.Position += NoEndData.Length;
                    var UniqueRoomId = reader.ReadInt32();
                    var DedicationSlot = reader.ReadByte();
                    var RoomSeed = reader.ReadInt32();
                    int ChannelId = (UniqueRoomId >> 12) & 0xff;
                    int RoomId = UniqueRoomId & 0xfff;

                    //if (DedicationSlot != SlotId)
                    //{
                    //    Logger.Warning($" [Battle] [REQUEST_131] Slot invalid. DedicationSlot: {DedicationSlot} SlotId: {SlotId}");
                    //}

                    Channel channel = ServersManager.GetChannel(ChannelId);
                    Room room = channel?.GetRoom(RoomId);
                    if (room == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_131] Room null! Id: {RoomId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.sessionPort == 0)
                    {
                        Logger.Warning($" [Battle] [REQUEST_131] Session port invalid! Id: {RoomId} SessionPort: {room.sessionPort} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.roomSeed != RoomSeed)
                    {
                        Logger.Warning($" [Battle] [REQUEST_131] Seed invalid! RoomSeed: {room.roomSeed} Seed: {RoomSeed} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    Slot slot = room.GetPlayer(SlotId, remoteEndPoint);
                    if (slot == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_131] Player null! SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (!slot.AccountIdIsValid(AccountId))
                    {
                        Logger.Warning($" [Battle] [REQUEST_131] AccountId invalid! AccountId: {AccountId} PlayerIdRegister: {slot.PlayerIdRegister} SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                    }
                    Logger.Battle($" BATTLE CASE 131 [IA SYNC PLAYER] DedicationSlot: {DedicationSlot} PacketSlot: {SlotId} [{slot.client.Address}:{slot.client.Port}]");
                    room.IsCase4BotMode = true;
                    Slot playerDedication = room.GetSlot(DedicationSlot);
                    if (playerDedication != null /*&& playerDedication.client != null*/)
                    {
                        byte[] Actions = WriteDataIA(NoEndData);
                        byte[] Code = WriteCode(132, Actions, DedicationSlot, playerDedication.GetSlotTime(), Round);
                        for (int slotIndex = 0; slotIndex < 16; slotIndex++)
                        {
                            Slot playerRoom = room.slots[slotIndex];
                            if (playerRoom.client != null && slotIndex != SlotId)
                            {
                                SendPacket(mainSocket, Code, playerRoom.client);
                            }
                        }
                    }
                    else
                    {
                        Logger.Warning($" [Battle] [REQUEST_131] PlayerDedication null! DedicationSlot: {DedicationSlot}");
                    }
                }
                else if (Opcode == 132) //132 IA BY HOST
                {
                    reader.BaseStream.Position += NoEndData.Length;
                    var UniqueRoomId = reader.ReadInt32();
                    var DedicationSlot = reader.ReadByte(); //DedicationSlot = 255 SlotId = 0-15
                    var RoomSeed = reader.ReadInt32();
                    var ChannelId = (UniqueRoomId >> 12) & 0xff;
                    var RoomId = UniqueRoomId & 0xfff;

                    Channel channel = ServersManager.GetChannel(ChannelId);
                    Room room = channel?.GetRoom(RoomId);
                    if (room == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_132] Room null! Id: {RoomId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.sessionPort == 0)
                    {
                        Logger.Warning($" [Battle] [REQUEST_132] Session port invalid! Id: {RoomId} SessionPort: {room.sessionPort} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (room.roomSeed != RoomSeed)
                    {
                        Logger.Warning($" [Battle] [REQUEST_132] Seed invalid! RoomSeed: {room.roomSeed} Seed: {RoomSeed} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    Slot slot = room.GetPlayer(SlotId, remoteEndPoint);
                    if (slot == null)
                    {
                        Logger.Warning($" [Battle] [REQUEST_132] Player null! SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                        return;
                    }
                    if (!slot.AccountIdIsValid(AccountId))
                    {
                        Logger.Warning($" [Battle] [REQUEST_132] AccountId invalid! AccountId: {AccountId} PlayerIdRegister: {slot.PlayerIdRegister} SlotId: {SlotId} EndPoint: {remoteEndPoint}");
                    }
                    room.IsCase4BotMode = true;
                    byte[] Actions = WriteDataIA(NoEndData);
                    byte[] Code = WriteCode(132, Actions, SlotId, slot.GetSlotTime(), Round);
                    for (int slotIndex = 0; slotIndex < 16; slotIndex++)
                    {
                        Slot playerRoom = room.slots[slotIndex];
                        if (playerRoom.client != null && slotIndex != SlotId)
                        {
                            SendPacket(mainSocket, Code, playerRoom.client);
                        }
                    }
                }
                else
                {
                    if ((DateTime.Now - Logger.LastSaveLogUdpBattle3).Minutes >= 1)
                    {
                        Logger.Attacks(" [Battle] Unknown Opcode: " + Opcode + " [!]");
                        //FirewallSecurity.RemoveRuleUdp(remoteEndPoint.Address.ToString());
                        Logger.LastSaveLogUdpBattle3 = DateTime.Now;
                    }
                }
                reader.Close();
                reader.Dispose();
                stream.Close();
                stream.Dispose();
                stream = null;
                reader = null;
            }
            catch (Exception ex)
            {
                Logger.Error($" [Battle] [Handler] [Exception] Message: {ex}");
            }
        }

        public void SendPacket(Socket mainSocket, byte[] buffer, IPEndPoint remote)
        {
            try
            {
                SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
                eventArgs.RemoteEndPoint = remote;
                eventArgs.SetBuffer(buffer, 0, buffer.Length);

                if (!mainSocket.SendToAsync(eventArgs))
                {
                    Logger.Warning(" [BattleHandler] (SendPacket) A operação de E/S foi concluída de forma síncrona.");
                }
                eventArgs.Dispose();
                //mainSocket.SendTo(buffer, 0, buffer.Length, SocketFlags.None, remote);
            }
            catch (Exception ex)
            {
                Logger.Error($" [BattleHandler] [SendPacket] Exception: {ex}");
            }
        }

        public void SendLogToPlayer(Account player, string log)
        {
            //if (player != null)
            //{
            //    //player.SendPacket(new LOBBY_CHATTING_PAK("Battle", player.GetSessionId(), 0, true, log));
            //}
            Logger.Battle(log);
        }

        public byte[] WriteCode(byte opcode, byte[] actions, byte slotId, float time, byte round)
        {
            int shift = (13 + actions.Length) % 6 + 1;
            byte[] actionsBuffer = Encrypt(actions, shift);
            using (BattlePacketWriter send = new BattlePacketWriter())
            {
                send.WriteC(opcode);
                send.WriteC(slotId); //255 = Todos
                send.WriteT(time); //Opcode 4 = Time of Room / Opcode 132 = Time of Player
                send.WriteC(round);
                send.WriteH((ushort)(13 + actionsBuffer.Length)); //Length
                send.WriteD(0); //Final do Cabeçalho do pacote recebido?
                send.WriteB(actionsBuffer);
                return send.memory.ToArray();
            }
        }

        public byte[] WriteDataPlayer(byte[] data, Slot slot, Room room, float time)
        {
            List<ObjectHitInfo> objects = new List<ObjectHitInfo>();
            BattlePacketReader receive = new BattlePacketReader(data);
            using (BattlePacketWriter send = new BattlePacketWriter())
            {
                for (int j = 0; j < 16; j++) //P2P = Cada slot recebe e envia sua informação para os outros jogadores.
                {
                    ActionModel action = new ActionModel();
                    try
                    {
                        action.Type = receive.ReadByteOutException(out bool exception);
                        if (exception)
                        {
                            break;
                        }
                        action.Slot = receive.ReadUshort();
                        action.LengthData = receive.ReadUshort();
                        if (action.LengthData == 65535)
                        {
                            break;
                        }
                        send.WriteC(action.Type);
                        send.WriteH(action.Slot);
                        send.WriteH(action.LengthData);

                        P2PSubHeadEnum SubHead = (P2PSubHeadEnum)action.Type;
                        if (SubHead == P2PSubHeadEnum.USER || SubHead == P2PSubHeadEnum.STAGEINFO_CHARA)
                        {
                            action.Flags = (EventsEnum)receive.ReadUint();
                            action.Data = receive.ReadB(action.LengthData - 9);

                            Logger.Battle($" (P2PSubHead: {SubHead}) (WriteDataPlayer) (Slot: {action.Slot}) ActionFlags: {action.Flags}");

                            if (action.Flags.HasFlag(EventsEnum.WeaponSync))
                            {
                                if ((action.Flags & (EventsEnum.TakeWeapon | EventsEnum.DropWeapon)) > 0)
                                {
                                    action.Flags -= EventsEnum.WeaponSync;
                                }
                            }
                            Slot player = room.GetPlayer(action.Slot, true);
                            if (player == null)
                            {
                                Logger.Warning($" WriteActionBytes player is null actionSlot: {action.Slot} slot.Id: {slot.Id}");
                                break;
                            }
                            if (player.Id != action.Slot)
                            {
                                Logger.Warning($" WriteActionBytes actionSlot: {action.Slot} != slot.Id: {slot.Id}");
                                break;
                            }
                            Account playerCache = room.GetPlayerBySlot(player);
                            if (playerCache == null)
                            {
                                Logger.Warning($" WriteActionBytes playercache is null actionSlot: {action.Slot} slot.Id: {slot.Id}");
                                break;
                            }
                            //player.weaponModel = null;

                            uint flagsValue = 0;
                            BattlePacketReader receiveFlags = new BattlePacketReader(action.Data);
                            using (BattlePacketWriter sendFlags = new BattlePacketWriter())
                            {
                                if (action.Flags.HasFlag(EventsEnum.ActionState))
                                {
                                    flagsValue++;
                                    byte Unknown1 = receiveFlags.ReadByte();
                                    byte Unknown2 = receiveFlags.ReadByte();
                                    byte Unknown3 = receiveFlags.ReadByte();
                                    byte Unknown4 = receiveFlags.ReadByte();

                                    Logger.Battle($" (Flag: ActionState) (Slot: {action.Slot}) Unknown1: {Unknown1} Unknown2: {Unknown2} Unknown3: {Unknown3} Unknown4: {Unknown4}");

                                    sendFlags.WriteC(Unknown1);
                                    sendFlags.WriteC(Unknown2);
                                    sendFlags.WriteC(Unknown3);
                                    sendFlags.WriteC(Unknown4);
                                }
                                if (action.Flags.HasFlag(EventsEnum.Animation))
                                {
                                    flagsValue += 2;
                                    //Fazer Proteção FlyHacker
                                    ushort positionVertical = receiveFlags.ReadUshort();
                                    sendFlags.WriteH(positionVertical);

                                    Logger.Battle($" (Flag: Animation) positionVertical: {positionVertical}");
                                }
                                if (action.Flags.HasFlag(EventsEnum.PosRotation))
                                {
                                    flagsValue += 4;
                                    var RotationX = receiveFlags.ReadUshort();
                                    var RotationY = receiveFlags.ReadUshort();
                                    var RotationZ = receiveFlags.ReadUshort();
                                    var CameraX = receiveFlags.ReadUshort();
                                    var CameraY = receiveFlags.ReadUshort();
                                    var Area = receiveFlags.ReadUshort();

                                    sendFlags.WriteH(RotationX);
                                    sendFlags.WriteH(RotationY);
                                    sendFlags.WriteH(RotationZ);
                                    sendFlags.WriteH(CameraX);
                                    sendFlags.WriteH(CameraY);
                                    sendFlags.WriteH(Area);

                                    player.position = new Half3(RotationX, RotationY, RotationZ);
                                    Logger.Battle($" (Flag: PosRotation) RotationX: {RotationX} RotationY: {RotationY} RotationZ: {RotationZ} CameraX: {CameraX} CameraY: {CameraY} Area: {Area}");
                                }
                                if (action.Flags.HasFlag(EventsEnum.OnLoadObject))
                                {
                                    flagsValue += 8;
                                    var SpaceflagsValue = receiveFlags.ReadByte();
                                    var ObjectId = receiveFlags.ReadUshort();
                                    CharaMovesEnum SpaceFlags = (CharaMovesEnum)SpaceflagsValue;

                                    Logger.Battle($" (Flag: OnLoadObject) SpaceFlags: {SpaceFlags} ObjectId: {ObjectId}");

                                    if (!room.IsCase4BotMode && ObjectId != 65535)
                                    {
                                        bool securityBlock = false;
                                        ObjectInfo objectInfo = room.GetObject(ObjectId);
                                        if (objectInfo != null)
                                        {
                                            SendLogToPlayer(playerCache, $"[OnLoadObject] ({SpaceFlags}) ObjectId: {ObjectId} Heli: {(ObjectId == 50 ? "RED" : ObjectId == 49 ? "BLUE" : "UNK")} animationId: {objectInfo.animation.Id} NextAnim: {objectInfo.animation.NextAnim} OtherAnim: {objectInfo.animation.OtherAnim} OtherObj: {objectInfo.animation.OtherObj} Duration: {objectInfo.animation.Duration} useDate: {objectInfo.useDate}");
                                            if (SpaceFlags.HasFlag(CharaMovesEnum.HELI_IN_MOVE)) //Entrando no Helicoptero em movimento.
                                            {
                                                Logger.Warning(" (Flag: OnLoadObject) HELI_IN_MOVE: Date: " + objectInfo.useDate.ToString("yyMMddHHmm"));
                                                if (objectInfo.useDate.ToString("yyMMddHHmm") == "0101010000")
                                                {
                                                    securityBlock = true;
                                                }
                                            }
                                            if (SpaceFlags.HasFlag(CharaMovesEnum.HELI_UNKNOWN)) //Quando aperta E para sair do modo atirador do Helicoptero
                                            {

                                            }
                                            if (SpaceFlags.HasFlag(CharaMovesEnum.HELI_LEAVE)) //Quando aperta E para sair do modo atirador do Helicoptero
                                            {

                                            }
                                            if (SpaceFlags.HasFlag(CharaMovesEnum.HELI_STOPPED)) //Entrando no Helicoptero parado.
                                            {
                                                if (objectInfo.animation != null && objectInfo.animation.Id == 0)
                                                {
                                                    objectInfo.info.GetAnim(objectInfo.animation.NextAnim, 0, 0, objectInfo);
                                                }
                                            }
                                            if (!securityBlock)
                                            {
                                                objects.Add(new ObjectHitInfo(3)
                                                {
                                                    ObjSyncId = 1,
                                                    ObjId = objectInfo.objectId,
                                                    ObjectLife = objectInfo.life,
                                                    AnimationId1 = 255,
                                                    AnimationId2 = objectInfo.animation != null ? objectInfo.animation.Id : 255,
                                                    SpecialUse = GetTime(objectInfo.useDate)
                                                });
                                            }
                                        }
                                    }
                                    sendFlags.WriteC(SpaceflagsValue);
                                    sendFlags.WriteH(ObjectId);
                                }
                                if (action.Flags.HasFlag(EventsEnum.Unk1)) //ActionForObjectSync
                                {
                                    flagsValue += 0x10;
                                    byte Unknown1 = receiveFlags.ReadByte();
                                    byte Unknown2 = receiveFlags.ReadByte();

                                    Logger.Battle($" (Flag: Unk1) Unknown1: {Unknown1} Unknown2: {Unknown2}");

                                    sendFlags.WriteC(Unknown1);
                                    sendFlags.WriteC(Unknown2);
                                }
                                if (action.Flags.HasFlag(EventsEnum.RadioChat))
                                {
                                    flagsValue += 0x20;
                                    var RadioId = receiveFlags.ReadByte();
                                    var Area = receiveFlags.ReadByte();
                                    Logger.Battle($" (Flag: RadioChat) RadioId: {RadioId} Area: {Area}");
                                    sendFlags.WriteC(RadioId);
                                    sendFlags.WriteC(Area);
                                }
                                if (action.Flags.HasFlag(EventsEnum.WeaponSync))
                                {
                                    flagsValue += 0x40;

                                    //Extensions = p.readC(),
                                    //WeaponId = p.readD()

                                    var weaponInfo = receiveFlags.ReadUshort();
                                    var weaponSlotInfo = receiveFlags.ReadByte();
                                    var charaModelId = receiveFlags.ReadByte();

                                    var WeaponSecondMelee = weaponSlotInfo >> 4;
                                    var WeaponSlot = weaponSlotInfo & 15;
                                    var WeaponNumber = (weaponInfo >> 6) & 1023;
                                    var WeaponClass = weaponInfo & 63;

                                    Logger.Battle($" (Flag: WeaponSync) WeaponInfo: {weaponInfo} WeaponSlotInfo: {weaponSlotInfo} CharaModelId: {charaModelId}");

                                    sendFlags.WriteH(weaponInfo);
                                    sendFlags.WriteC(weaponSlotInfo);
                                    sendFlags.WriteC(charaModelId);

                                    player.weaponClass = (ClassTypeEnum)WeaponClass;
                                    player.character = (CharactersEnum)charaModelId;
                                    player.weaponModel = WeaponsXML.GetWeapon(WeaponNumber, WeaponClass, WeaponSlot);
                                    room.SyncInfo(objects, 2);
                                }
                                if (action.Flags.HasFlag(EventsEnum.WeaponRecoil))
                                {
                                    flagsValue += 0x80;
                                    float RecoilHorzAngle = receiveFlags.ReadFloat();
                                    float RecoilHorzMax = receiveFlags.ReadFloat();
                                    float RecoilVertAngle = receiveFlags.ReadFloat();
                                    float RecoilVertMax = receiveFlags.ReadFloat();
                                    float Deviation = receiveFlags.ReadFloat();
                                    ushort WeaponId = receiveFlags.ReadUshort(); //nao é number
                                    byte WeaponSlot = receiveFlags.ReadByte();
                                    byte Unkwnon = receiveFlags.ReadByte(); //DeviationMax? ou BulletWeight(Peso da bala-QUASE CERTEZA)
                                    byte RecoilHorzCount = receiveFlags.ReadByte();

                                    //WeaponInfo weaponModel = player.weaponModel;
                                    //if (weaponModel != null)
                                    //{
                                    //if (weaponModel.recoilHorzAngle != RecoilHorzAngle)
                                    //{
                                    //    Logger.Warning($" (Flag: WeaponRecoil) WARNING [!] RecoilHorzAngle: {RecoilHorzAngle} != myWeapon.recoilHorzAngle: {weaponModel.recoilHorzAngle}");
                                    //    RecoilHorzAngle = weaponModel.recoilHorzAngle;
                                    //}
                                    //if (weaponModel.recoilHorzMax != RecoilHorzMax)
                                    //{
                                    //    Logger.Warning($" (Flag: WeaponRecoil) WARNING [!] RecoilHorzMax: {RecoilHorzMax} != myWeapon.recoilHorzMax: {weaponModel.recoilHorzMax}");
                                    //    RecoilHorzMax = weaponModel.recoilHorzMax;
                                    //}
                                    //if (weaponModel.recoilVertAngle != RecoilVertAngle)
                                    //{
                                    //    Logger.Warning($" (Flag: WeaponRecoil) WARNING [!] RecoilVertAngle: {RecoilVertAngle} != myWeapon.recoilVertAngle: {weaponModel.recoilVertAngle}");
                                    //    RecoilVertAngle = weaponModel.recoilVertAngle;
                                    //}
                                    //if (weaponModel.recoilVertMax != RecoilVertMax)
                                    //{
                                    //    Logger.Warning($" (Flag: WeaponRecoil) WARNING [!] RecoilVertMax: {RecoilVertMax} != myWeapon.recoilVertMax: {weaponModel.recoilVertMax}");
                                    //    RecoilVertMax = weaponModel.recoilVertMax;
                                    //}
                                    //if (RecoilHorzCount > weaponModel.recoilHorzMax)
                                    //{
                                    //    Logger.Warning($" (Flag: WeaponRecoil) WARNING [!] RecoilHorzCount: {RecoilHorzCount} > myWeapon.recoilHorzMax: {weaponModel.recoilHorzMax}");
                                    //    RecoilHorzCount = (byte)weaponModel.recoilHorzMax;
                                    //}
                                    //}
                                    //else
                                    //{
                                    //    Logger.Warning($" (Flag: WeaponRecoil) weaponModel is null");
                                    //}

                                    Logger.Battle($" (Flag: WeaponRecoil) (WeaponId/WeaponSlot): {WeaponId}/{WeaponSlot} Horz(Angle/Max/Count): {RecoilHorzAngle}/{RecoilHorzMax}/{RecoilHorzCount} Vert(Angle/Max): {RecoilVertAngle}/{RecoilVertMax} Deviation: {Deviation} Unk: {Unkwnon}");

                                    sendFlags.WriteT(RecoilHorzAngle);
                                    sendFlags.WriteT(RecoilHorzMax);
                                    sendFlags.WriteT(RecoilVertAngle);
                                    sendFlags.WriteT(RecoilVertMax);
                                    sendFlags.WriteT(Deviation);
                                    sendFlags.WriteH(WeaponId);
                                    sendFlags.WriteC(WeaponSlot);
                                    sendFlags.WriteC(Unkwnon);
                                    sendFlags.WriteC(RecoilHorzCount);
                                }
                                if (action.Flags.HasFlag(EventsEnum.LifeSync))
                                {
                                    flagsValue += 0x100;
                                    ushort ObjectLife = receiveFlags.ReadUshort();
                                    Logger.Battle($" (Flag: LifeSync) ObjectLife: {ObjectLife}");
                                    sendFlags.WriteH(ObjectLife);
                                }
                                if (action.Flags.HasFlag(EventsEnum.Suicide))
                                {
                                    flagsValue += 0x200;
                                    List<HitDataSuicide> hits = new List<HitDataSuicide>();
                                    List<DeathServerData> deaths = new List<DeathServerData>();
                                    int ItemId = 0;
                                    int ObjIdx = -1;
                                    int hitsCount = receiveFlags.ReadByte();
                                    for (int i = 0; i < hitsCount; i++)
                                    {
                                        HitDataSuicide hit = new HitDataSuicide
                                        {
                                            HitInfo = receiveFlags.ReadInt(),
                                            WeaponInfo = receiveFlags.ReadUshort(),
                                            WeaponSlot = receiveFlags.ReadByte(),
                                            PlayerPos = receiveFlags.ReadUshortVector()
                                        };
                                        hit.SetData();
                                        bool HitValid = true;
                                        if (!player.isDead && player.life > 0)
                                        {
                                            if (hit.Unk == 1)
                                            {
                                                ObjIdx = hit.KillerId;
                                            }
                                            ItemId = CreateItemId(hit.WeaponClass, hit.WeaponSlot, hit.WeaponId);
                                            player.life -= hit.WeaponDamage;
                                            if (player.life <= 0)
                                            {
                                                //Seta o Death
                                                player.life = 0;
                                                player.isDead = true;
                                                player.lastDie = DateTime.Now;
                                                deaths.Add(new DeathServerData { slot = player, DeathType = hit.DeathType });
                                            }
                                            else
                                            {
                                                //Seta o Hit
                                                objects.Add(new ObjectHitInfo(5)
                                                {
                                                    ObjId = player.Id,
                                                    killerId = player.Id,
                                                    DeathType = hit.DeathType,
                                                    ObjectLife = hit.HitPart
                                                });
                                            }
                                            objects.Add(new ObjectHitInfo(2)
                                            {
                                                ObjId = player.Id,
                                                ObjectLife = player.life,
                                                DeathType = hit.DeathType,
                                                HitPart = hit.HitPart,
                                                WeaponId = ItemId,
                                                Position = hit.PlayerPos
                                            });
                                        }
                                        else
                                        {
                                            HitValid = false;
                                        }
                                        if (HitValid)
                                        {
                                            hits.Add(hit);
                                        }
                                        Logger.Battle($" (Flag: Suicide) HitsCount: {hitsCount} HitInfo: {hit.HitInfo} WeaponInfo: {hit.WeaponInfo} WeaponSlot: {hit.WeaponSlot} PlayerPosition: {hit.PlayerPos}");
                                    }
                                    if (deaths.Count > 0)
                                    {
                                        Death(room, player, (byte)ObjIdx, ItemId, deaths);
                                    }
                                    sendFlags.WriteC((byte)hits.Count);
                                    for (int i = 0; i < hits.Count; i++)
                                    {
                                        HitDataSuicide hit = hits[i];
                                        sendFlags.WriteD(hit.HitInfo);
                                        sendFlags.WriteH(hit.WeaponInfo);
                                        sendFlags.WriteC(hit.WeaponSlot);
                                        sendFlags.WriteHVector(hit.PlayerPos);
                                    }
                                    hits = null;
                                    deaths = null;
                                }
                                if (action.Flags.HasFlag(EventsEnum.BombMission))
                                {
                                    flagsValue += 0x400;
                                    int BombAll = receiveFlags.ReadByte();
                                    float PlantTime = receiveFlags.ReadFloat();
                                    BombFlagEnum BombEnum = (BombFlagEnum)(BombAll & 15);
                                    int BombId = BombAll >> 4;
                                    if (room.Map != null && !player.isDead && PlantTime > 0 && !BombEnum.HasFlag(BombFlagEnum.Stop))
                                    {
                                        BombPosition bomb = room.Map.GetBomb(BombId);
                                        if (bomb != null)
                                        {
                                            bool isDefuse = BombEnum.HasFlag(BombFlagEnum.Defuse);
                                            Vector3 BombVec3d;
                                            if (isDefuse)
                                            {
                                                BombVec3d = room.BombPosition;
                                            }
                                            else if (BombEnum.HasFlag(BombFlagEnum.Start))
                                            {
                                                BombVec3d = bomb.Position;
                                            }
                                            else
                                            {
                                                BombVec3d = new Half3(0, 0, 0);
                                            }
                                            double PlayerDistance = Vector3.DistanceBomb(player.position, BombVec3d);
                                            if ((bomb.Everywhere || PlayerDistance <= 2.0) && (player.teamId == 1 && isDefuse || player.teamId == 0 && !isDefuse))
                                            {
                                                if (player.C4FTime != PlantTime)
                                                {
                                                    player.C4First = DateTime.Now;
                                                    player.C4FTime = PlantTime;
                                                }
                                                double Seconds = (DateTime.Now - player.C4First).TotalSeconds;

                                                float objective = isDefuse ? player.defuseDuration : player.plantDuration;

                                                if (((time >= PlantTime + objective) || Seconds >= objective) && ((!room.PlantedBombC4 && BombEnum.HasFlag(BombFlagEnum.Start)) || (room.PlantedBombC4 && isDefuse)))
                                                {
                                                    BombAll |= 2;
                                                    if (room.round.Timer == null && room.state == RoomStateEnum.Battle && player.state == SlotStateEnum.BATTLE)
                                                    {
                                                        if (BombEnum.HasFlag(BombFlagEnum.Defuse)) //UNINSTALL BOMB
                                                        {
                                                            if (room.PlantedBombC4)
                                                            {
                                                                using (BATTLE_MISSION_BOMB_UNINSTALL_PAK packet = new BATTLE_MISSION_BOMB_UNINSTALL_PAK(player.Id))
                                                                {
                                                                    room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                                                                }
                                                                if (room.mode != RoomTypeEnum.Tutorial && room.mapId != 44)
                                                                {
                                                                    player.objetivos++;
                                                                    room.blueRounds++;
                                                                    room.MissionCompleteBase(room.GetPlayerBySlot(player), player, MissionTypeEnum.C4_DEFUSE, 0);
                                                                    room.BattleEndRound(1, RoundEndTypeEnum.Uninstall);
                                                                }
                                                            }
                                                        }
                                                        else //INSTALL BOMB
                                                        {
                                                            if (!room.PlantedBombC4)
                                                            {
                                                                room.BombPosition = player.position;
                                                                using (BATTLE_MISSION_BOMB_INSTALL_PAK packet = new BATTLE_MISSION_BOMB_INSTALL_PAK(player.Id, (byte)BombId, player.position.X, player.position.Y, player.position.Z))
                                                                {
                                                                    room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                                                                }
                                                                if (room.mode != RoomTypeEnum.Tutorial && room.mapId != 44)
                                                                {
                                                                    room.PlantedBombC4 = true;
                                                                    player.objetivos++;
                                                                    room.MissionCompleteBase(room.GetPlayerBySlot(player), player, MissionTypeEnum.C4_PLANT, 0);
                                                                    room.StartBomb();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    sendFlags.WriteC((byte)BombAll);
                                    sendFlags.WriteT(PlantTime);
                                    Logger.Battle($" (Flag: BombMission) Time: {time} PlantTime: {PlantTime} BombId: {BombId} BombEnum: {BombEnum} BombAll: {BombAll}");
                                }
                                if (action.Flags.HasFlag(EventsEnum.TakeWeapon)) //TakeWeapon = Pegar Arma
                                {
                                    flagsValue += 0x800;
                                    byte weaponFlag = receiveFlags.ReadByte();
                                    byte weaponClass = receiveFlags.ReadByte();
                                    ushort weaponId = receiveFlags.ReadUshort();
                                    byte ammoPrin = receiveFlags.ReadByte();
                                    byte ammoDual = receiveFlags.ReadByte();
                                    ushort ammoTotal = receiveFlags.ReadUshort();

                                    sendFlags.WriteC(weaponFlag);
                                    sendFlags.WriteC(weaponClass);
                                    sendFlags.WriteH(weaponId);
                                    if (Settings.UseMaxAmmoInDrop)
                                    {
                                        sendFlags.WriteC(255);
                                        sendFlags.WriteC(ammoDual);
                                        sendFlags.WriteH(10000);
                                    }
                                    else
                                    {
                                        sendFlags.WriteC(ammoPrin); //Balas no pente da arma Principal
                                        sendFlags.WriteC(ammoDual); //Balas no pente da arma Principal Dual
                                        sendFlags.WriteH(ammoTotal); //Balas total de recarregamento da arma principal
                                    }
                                }
                                if (action.Flags.HasFlag(EventsEnum.DropWeapon)) //DropWeapon = Soltar Arma
                                {
                                    flagsValue += 0x1000;
                                    if (!room.IsCase4BotMode)
                                    {
                                        room.DropCounter++;
                                        if (room.DropCounter > Settings.MaxDrop)
                                        {
                                            room.DropCounter = 0;
                                        }
                                    }
                                    byte weaponFlag = receiveFlags.ReadByte();
                                    byte weaponClass = receiveFlags.ReadByte();
                                    ushort weaponId = receiveFlags.ReadUshort();
                                    byte ammoPrin = receiveFlags.ReadByte();
                                    byte ammoDual = receiveFlags.ReadByte();
                                    ushort ammoTotal = receiveFlags.ReadUshort();

                                    sendFlags.WriteC((byte)(weaponFlag + room.DropCounter));
                                    sendFlags.WriteC(weaponClass);
                                    sendFlags.WriteH(weaponId);

                                    if (Settings.UseMaxAmmoInDrop)
                                    {
                                        sendFlags.WriteC(255);
                                        sendFlags.WriteC(ammoDual);
                                        sendFlags.WriteH(10000);
                                    }
                                    else
                                    {
                                        sendFlags.WriteC(ammoPrin);
                                        sendFlags.WriteC(ammoDual);
                                        sendFlags.WriteH(ammoTotal);
                                    }
                                }
                                if (action.Flags.HasFlag(EventsEnum.FireSync)) //FireSync = Sincronização do Tiro
                                {
                                    flagsValue += 0x2000;

                                    //		                Effect = Packet.readC(),
                                    //	   Part = Packet.readC(),
                                    //	Index = Packet.readH(),
                                    //	 X = Packet.readUH(),
                                    //	  Y = Packet.readUH(),
                                    //	 Z = Packet.readUH(),
                                    //	 Extensions = Packet.readC(),
                                    //	 WeaponId = Packet.readD()

                                    ushort shotId = receiveFlags.ReadUshort();
                                    ushort shotIndex = receiveFlags.ReadUshort();
                                    ushort camX = receiveFlags.ReadUshort();
                                    ushort camY = receiveFlags.ReadUshort();
                                    ushort camZ = receiveFlags.ReadUshort();
                                    int weaponNumber = receiveFlags.ReadInt();

                                    Logger.Battle($" (Flag: FireSync) Id: {shotId} Index: {shotIndex} WeaponNumber: {weaponNumber} CamX: {camX} CamY: {camY} CamZ: {camZ}");

                                    sendFlags.WriteH(shotId);
                                    sendFlags.WriteH(shotIndex);
                                    sendFlags.WriteH(camX);
                                    sendFlags.WriteH(camY);
                                    sendFlags.WriteH(camZ);
                                    sendFlags.WriteD(weaponNumber);
                                }
                                if (action.Flags.HasFlag(EventsEnum.AIDamage)) //Artificial Intelligence Damage (MODO BOT, ZUMBI)
                                {
                                    flagsValue += 0x4000;
                                    int hitsCount = receiveFlags.ReadByte();
                                    sendFlags.WriteC((byte)hitsCount);
                                    for (int i = 0; i < hitsCount; i++)
                                    {
                                        uint hitInfo = receiveFlags.ReadUint();
                                        ushort WeaponInfo = receiveFlags.ReadUshort();
                                        byte WeaponSlot = receiveFlags.ReadByte();
                                        ushort Unknown = receiveFlags.ReadUshort();
                                        ushort EixoX = receiveFlags.ReadUshort();
                                        ushort EixoY = receiveFlags.ReadUshort();
                                        ushort EixoZ = receiveFlags.ReadUshort();

                                        Logger.Battle($" (Flag: AIDamage) HitsCount: {hitsCount} HitInfo: {hitInfo} WeaponInfo: {WeaponInfo} WeaponSlot: {WeaponSlot} Unknown: {Unknown} EixoX: {EixoX} EixoY: {EixoY} EixoZ: {EixoZ}");

                                        sendFlags.WriteD(hitInfo);
                                        sendFlags.WriteH(WeaponInfo);
                                        sendFlags.WriteC(WeaponSlot);
                                        sendFlags.WriteH(Unknown);
                                        sendFlags.WriteH(EixoX);
                                        sendFlags.WriteH(EixoY);
                                        sendFlags.WriteH(EixoZ);
                                    }
                                }
                                if (action.Flags.HasFlag(EventsEnum.NormalDamage))
                                {
                                    flagsValue += 0x8000;
                                    List<HitDataNormalDamage> hits = new List<HitDataNormalDamage>();
                                    List<DeathServerData> deaths = new List<DeathServerData>();
                                    int ItemId = 0;
                                    byte hitsCount = receiveFlags.ReadByte();
                                    for (byte i = 0; i < hitsCount; i++)
                                    {
                                        HitDataNormalDamage hit = new HitDataNormalDamage
                                        {
                                            HitInfo = receiveFlags.ReadInt(),
                                            BoomInfo = receiveFlags.ReadUshort(),
                                            WeaponInfo = receiveFlags.ReadUshort(),
                                            WeaponSlot = receiveFlags.ReadByte(),
                                            StartBullet = receiveFlags.ReadFloatVector(),
                                            EndBullet = receiveFlags.ReadFloatVector()
                                        };
                                        hit.SetData();
                                        if (hit.BoomInfo > 0)
                                        {
                                            hit.BoomPlayers = new List<int>();
                                            for (int slotId = 0; slotId < 16; slotId++)
                                            {
                                                int flag = 1 << slotId;
                                                if ((hit.BoomInfo & flag) == flag)
                                                {
                                                    hit.BoomPlayers.Add(slotId);
                                                }
                                            }
                                        }
                                        bool HitValid = true;
                                        if (hit.HitType == HitTypeEnum.HelmetProtection || hit.HitType == HitTypeEnum.HeadshotProtection)
                                        {
                                            continue;
                                        }
                                        WeaponInfo getWeapon = WeaponsXML.GetWeapon(hit.WeaponId, (int)hit.WeaponClass, hit.WeaponSlot);
                                        WeaponInfo weaponModel = player.weaponModel;
                                        if (weaponModel == null)
                                        {
                                            Logger.Error(" WEAPON IS NULL NormalDamage"); //ATIRAR COM O HELICOPTERO
                                            HitValid = false;
                                        }
                                        if (weaponModel.name != getWeapon.name)
                                        {
                                            Logger.Error($" (Flag: NormalDamage) VALORES NÃO COMPATIVEL NA COMPARAÇÃO! weaponModel.name: {weaponModel.name} getWeapon.name: {getWeapon.name}");
                                            weaponModel = getWeapon;
                                        }
                                        string Hollow = "NO USE";
                                        if (playerCache.effects.HasFlag(CupomEffects.HollowPoint)) { Hollow = "HollowPoint"; }
                                        if (playerCache.effects.HasFlag(CupomEffects.HollowPointF)) { Hollow = "HollowPointF"; }
                                        if (playerCache.effects.HasFlag(CupomEffects.HollowPointPlus)) { Hollow = "HollowPointPlus"; }
                                        if (playerCache.effects.HasFlag(CupomEffects.IronBullet)) { Hollow = "IronBullet"; }

                                        SendLogToPlayer(playerCache, $" (Flag: NormalDamage) HitPartValue: {hit.HitPart} HitCharaPart: {hit.CharaHitPart} HitType: {hit.HitType} HitRange: {hit.Range}/{weaponModel.range} HitDamage: {hit.WeaponDamage}/{weaponModel.damage} Hollow: {Hollow} Name: {weaponModel.name} PlayerId: {slot.playerId} EndPoint: {slot.client}");

                                        if (!weaponModel.CheckWeapon(hit, out string result))
                                        {
                                            Logger.Warning($" (Flag: NormalDamage) INVALID HIT ({result}) [!] HitPartValue: {hit.HitPart} HitCharaPart: {hit.CharaHitPart} HitType: {hit.HitType} HitRange: {hit.Range}/{weaponModel.range} HitDamage: {hit.WeaponDamage}/{weaponModel.damage} Hollow: {Hollow} Name: {weaponModel.name} PlayerId: {slot.playerId} EndPoint: {slot.client}");
                                            HitValid = false;
                                        }
                                        ItemId = CreateItemId(hit.WeaponClass, hit.WeaponSlot, hit.WeaponId);
                                        if (hit.ObjectType == ObjectTypeEnum.User)
                                        {
                                            if (room.GetPlayer(hit.WeaponObjectId, out Slot playerVictim) && !player.isDead && !playerVictim.isDead && !playerVictim.TRexImmortal)
                                            {
                                                if (room.mode == RoomTypeEnum.Sniper && hit.DeathType != CharaDeathEnum.HEADSHOT) //MODO HEADHUNTER
                                                {
                                                    hit.WeaponDamage = 1;
                                                }
                                                else if (room.mode == RoomTypeEnum.Dino && hit.DeathType == CharaDeathEnum.HEADSHOT) //MODO DINO
                                                {
                                                    if ((room.LastRound == 1 && playerVictim.teamId == 0) || (room.LastRound == 2 && playerVictim.teamId == 1))
                                                    {
                                                        hit.WeaponDamage /= 10;
                                                    }
                                                }
                                                else if (room.mode == RoomTypeEnum.Chaos) //MODO CHAOS
                                                {
                                                    hit.WeaponDamage = 200;
                                                }
                                                playerVictim.life -= hit.WeaponDamage;
                                                if (playerVictim.life <= 0)
                                                {
                                                    playerVictim.life = 0;
                                                    playerVictim.isDead = true;
                                                    playerVictim.lastDie = DateTime.Now;
                                                    deaths.Add(new DeathServerData { slot = playerVictim, DeathType = hit.DeathType });
                                                }
                                                else
                                                {
                                                    objects.Add(new ObjectHitInfo(5)
                                                    {
                                                        ObjId = playerVictim.Id,
                                                        killerId = player.Id,
                                                        DeathType = hit.DeathType,
                                                        ObjectLife = hit.HitPart
                                                    });
                                                }
                                                objects.Add(new ObjectHitInfo(2)
                                                {
                                                    ObjId = playerVictim.Id,
                                                    ObjectLife = playerVictim.life,
                                                    HitPart = hit.HitPart,
                                                    killerId = player.Id,
                                                    Position = (Vector3)playerVictim.position - player.position,
                                                    DeathType = hit.DeathType,
                                                    WeaponId = ItemId
                                                });
                                            }
                                            else
                                            {
                                                HitValid = false;
                                            }
                                        }
                                        else if (hit.ObjectType == ObjectTypeEnum.Object)
                                        {
                                            ObjectInfo objectInfo = room.GetObject(hit.WeaponObjectId);
                                            if (objectInfo != null)
                                            {
                                                if (objectInfo.info != null && objectInfo.info.IsDestroyable && objectInfo.life > 0)
                                                {
                                                    Logger.Error($" ObjectId: {objectInfo.objectId} useDate: {objectInfo.useDate} life: {objectInfo.life} needSync: {objectInfo.info.NeedSync} amin1: {objectInfo.info.Anim1} EndPoint: {slot.client}");
                                                    objectInfo.life -= hit.WeaponDamage;
                                                    if (objectInfo.life <= 0)
                                                    {
                                                        objectInfo.life = 0;
                                                        if (hit.BoomPlayers != null)
                                                        {
                                                            for (int k = 0; k < hit.BoomPlayers.Count; k++)
                                                            {
                                                                int slotBoom = hit.BoomPlayers[k];
                                                                if (room.GetPlayer(slotBoom, out Slot battleplayer) && !player.isDead && battleplayer.teamId != player.teamId || battleplayer.teamId == player.teamId && player.Id == battleplayer.Id)
                                                                {
                                                                    //Seta o Death
                                                                    battleplayer.life = 0;
                                                                    battleplayer.isDead = true;
                                                                    battleplayer.lastDie = DateTime.Now;
                                                                    deaths.Add(new DeathServerData { slot = battleplayer, DeathType = CharaDeathEnum.OBJECT_EXPLOSION });
                                                                    objects.Add(new ObjectHitInfo(2)
                                                                    {
                                                                        HitPart = 1,
                                                                        DeathType = CharaDeathEnum.OBJECT_EXPLOSION,
                                                                        ObjId = slotBoom,
                                                                        killerId = battleplayer.Id,
                                                                        WeaponId = ItemId,
                                                                    });
                                                                }
                                                            }
                                                        }
                                                    }
                                                    objectInfo.destroyState = objectInfo.info.CheckDestroyState(objectInfo.life);

                                                    if (objectInfo.info.UltraSYNC > 0 && (room.mode == RoomTypeEnum.Sabotage || room.mode == RoomTypeEnum.Defense))
                                                    {
                                                        if (objectInfo.info.UltraSYNC == 1 || objectInfo.info.UltraSYNC == 3)
                                                        {
                                                            room.Bar1 = objectInfo.life;
                                                        }
                                                        else if (objectInfo.info.UltraSYNC == 2 || objectInfo.info.UltraSYNC == 4)
                                                        {
                                                            room.Bar2 = objectInfo.life;
                                                        }
                                                        Sabotage(room, player, (ushort)hit.WeaponDamage, objectInfo.info.UltraSYNC == 4 ? 2 : 1);
                                                    }

                                                    SendLogToPlayer(playerCache, $" Damage normal no Objeto [!] ObjectId: {objectInfo.objectId} Life: {objectInfo.life} UpdateId: {objectInfo.info.UpdateId} NeedSync: {objectInfo.info.NeedSync} UseDate: {GetTime(objectInfo.useDate)} A1: {objectInfo.info.Anim1} A2: {(objectInfo.animation != null ? objectInfo.animation.Id : 255)} DestroyState: {objectInfo.destroyState}");
                                                    float SyncingTime = GetTime(objectInfo.useDate);
                                                    if (objectInfo.animation != null && objectInfo.animation.Duration > 0 && SyncingTime >= objectInfo.animation.Duration)
                                                    {
                                                        objectInfo.info.GetAnim(objectInfo.animation.NextAnim, SyncingTime, objectInfo.animation.Duration, objectInfo);
                                                    }
                                                    objects.Add(new ObjectHitInfo(objectInfo.info.UpdateId)
                                                    {
                                                        ObjId = objectInfo.objectId,
                                                        ObjectLife = objectInfo.life,
                                                        killerId = action.Slot,
                                                        ObjSyncId = objectInfo.info.NeedSync ? 1 : 0,
                                                        SpecialUse = GetTime(objectInfo.useDate),
                                                        AnimationId1 = objectInfo.info.Anim1,
                                                        AnimationId2 = objectInfo.animation != null ? objectInfo.animation.Id : 255,
                                                        DestroyState = objectInfo.destroyState
                                                    });
                                                }
                                                else if (objectInfo.info == null)
                                                {
                                                    Logger.Warning($" (Flag: NormalDamage) (ObjectModel) Object invalid! ObjectId: {hit.WeaponObjectId} MapId: {room.mapId}");
                                                }
                                            }
                                            else
                                            {
                                                Logger.Warning($" (Flag: NormalDamage) (ObjectInfo) Object invalid! ObjectId: {hit.WeaponObjectId} MapId: {room.mapId}");
                                            }
                                        }
                                        else if (hit.ObjectType == ObjectTypeEnum.UserObject)
                                        {
                                            int ownerSlot = hit.WeaponObjectId >> 4;
                                            int grenadeMapId = hit.WeaponObjectId & 15;
                                            Logger.Warning($" (Flag: NormalDamage) [UserObject] [!] HitDamage: {hit.WeaponDamage}/{weaponModel.damage} Name: {weaponModel.name} EndPoint: {slot.client} OwnerSlot: {ownerSlot} GrenadeMapId: {grenadeMapId} (Deu dano de granada no User e no Object)");
                                        }
                                        else
                                        {
                                            Logger.Warning($" (Flag: NormalDamage) Unknown ObjectType: ({hit.ObjectType}/{(int)hit.ObjectType})");
                                        }
                                        if (HitValid)
                                        {
                                            hits.Add(hit);
                                        }
                                        Logger.Battle($" (Flag: NormalDamage) HitsCount: {hitsCount} HitInfo: {hit.HitInfo} HitType:{hit.ObjectType} WeaponInfo: {hit.WeaponInfo} WeaponSlot: {hit.WeaponSlot} StartBullet: {hit.StartBullet} EndBullet: {hit.EndBullet} EndPoint: {slot.client}");
                                    }
                                    if (deaths.Count > 0)
                                    {
                                        Death(room, player, 255, ItemId, deaths);
                                    }
                                    sendFlags.WriteC((byte)hits.Count);
                                    for (int i = 0; i < hits.Count; i++)
                                    {
                                        HitDataNormalDamage hit = hits[i];
                                        sendFlags.WriteD(hit.HitInfo);
                                        sendFlags.WriteH(hit.BoomInfo);
                                        sendFlags.WriteH(hit.WeaponInfo);
                                        sendFlags.WriteC(hit.WeaponSlot);
                                        sendFlags.WriteTVector(hit.StartBullet);
                                        sendFlags.WriteTVector(hit.EndBullet);
                                    }
                                    hits = null;
                                    deaths = null;
                                }
                                if (action.Flags.HasFlag(EventsEnum.BoomDamage)) //Dano por Barril, Uso do MEDICAL KIT
                                {
                                    flagsValue += 0x10000;
                                    List<HitDataBoomDamage> hits = new List<HitDataBoomDamage>();
                                    List<DeathServerData> deaths = new List<DeathServerData>();
                                    int idx = -1;
                                    int ItemId = 0;
                                    int hitsCount = receiveFlags.ReadByte();
                                    for (int i = 0; i < hitsCount; i++)
                                    {
                                        HitDataBoomDamage hit = new HitDataBoomDamage
                                        {
                                            HitInfo = receiveFlags.ReadInt(),
                                            BoomInfo = receiveFlags.ReadUshort(),
                                            WeaponInfo = receiveFlags.ReadUshort(),
                                            WeaponSlot = receiveFlags.ReadByte(),
                                            DeathType = receiveFlags.ReadByte(),
                                            FirePos = receiveFlags.ReadUshortVector(), //pointOfExplosion X, Y, Z
                                            HitPos = receiveFlags.ReadUshortVector(), //pointOfImpact X, Y, Z
                                            GrenadesCount = receiveFlags.ReadUshort()
                                        };
                                        hit.SetData();
                                        if (hit.BoomInfo > 0)
                                        {
                                            hit.BoomPlayers = new List<int>();
                                            for (int slotId = 0; slotId < 16; slotId++)
                                            {
                                                int flag = 1 << slotId;
                                                if ((hit.BoomInfo & flag) == flag)
                                                {
                                                    hit.BoomPlayers.Add(slotId);
                                                }
                                            }
                                        }
                                        bool HitValid = true;
                                        WeaponInfo getWeapon = WeaponsXML.GetWeapon(hit.WeaponId, (int)hit.WeaponClass, hit.WeaponSlot);
                                        WeaponInfo weaponModel = player.weaponModel;
                                        if (weaponModel == null)
                                        {
                                            Logger.Error(" WEAPON IS NULL BoomDamage");
                                        }
                                        SendLogToPlayer(playerCache, $" (Flag: BoomDamage) HitPartValue: {hit.HitPart} CharaHitPart: {hit.CharaHitPart} UsageType: {weaponModel.usageType} HitRange: {hit.Range}/{weaponModel.range} HitDamage: {hit.WeaponDamage}/{weaponModel.damage} Name: {weaponModel.name} PlayerId: {slot.playerId} EndPoint: {slot.client}");
                                        if (weaponModel.name != getWeapon.name)
                                        {
                                            Logger.Error($" (Flag: BoomDamage) VALORES NÃO COMPATIVEL NA COMPARAÇÃO! weaponModel.name: {weaponModel.name} getWeapon.name: {getWeapon.name}");
                                        }
                                        if (hit.WeaponDamage > weaponModel.damage)
                                        {
                                            Logger.Warning($" (Flag: BoomDamage) INVALID HIT (Damage) [!] HitPartValue: {hit.HitPart} CharaHitPart: {hit.CharaHitPart} UsageType: {weaponModel.usageType} HitRange: {hit.Range}/{weaponModel.range} HitDamage: {hit.WeaponDamage}/{weaponModel.damage} Name: {weaponModel.name} PlayerId: {slot.playerId} EndPoint: {slot.client}");
                                            HitValid = false;
                                        }
                                        if (hit.Range > weaponModel.range)
                                        {
                                            Logger.Warning($" (Flag: BoomDamage) INVALID HIT (Range) [!] HitPartValue: {hit.HitPart} CharaHitPart: {hit.CharaHitPart} UsageType: {weaponModel.usageType} HitRange: {hit.Range}/{weaponModel.range} HitDamage: {hit.WeaponDamage}/{weaponModel.damage} Name: {weaponModel.name} PlayerId: {slot.playerId} EndPoint: {slot.client}");
                                            HitValid = false;
                                        }
                                        ItemId = CreateItemId(hit.WeaponClass, hit.WeaponSlot, hit.WeaponId);
                                        if (hit.ObjectType == ObjectTypeEnum.User)
                                        {
                                            idx++;
                                            if (hit.WeaponDamage > 0 && room.GetPlayer(hit.WeaponObjectId, out Slot playerRoom) && !playerRoom.isDead && !playerRoom.TRexImmortal)
                                            {
                                                if ((CharaDeathEnum)hit.DeathType == CharaDeathEnum.MEDICAL_KIT)
                                                {
                                                    playerRoom.life += hit.WeaponDamage;
                                                    playerRoom.CheckLifeValue();
                                                }
                                                else if ((CharaDeathEnum)hit.DeathType == CharaDeathEnum.BOOM && hit.WeaponClass != ClassTypeEnum.Dino && (idx % 2 == 0))
                                                {
                                                    int damageTotal = hit.WeaponDamage;
                                                    hit.WeaponDamage = (int)Math.Ceiling(hit.WeaponDamage / 2.7);
                                                    Logger.Warning($" (Flag: BoomDamage) WARNING: The server is reduced from {damageTotal} to {hit.WeaponDamage} damage by Explosion.");
                                                    playerRoom.life -= hit.WeaponDamage;
                                                    if (playerRoom.life <= 0)
                                                    {
                                                        //Seta o Death
                                                        playerRoom.life = 0;
                                                        playerRoom.isDead = true;
                                                        playerRoom.lastDie = DateTime.Now;
                                                        deaths.Add(new DeathServerData { slot = playerRoom, DeathType = (CharaDeathEnum)hit.DeathType });
                                                    }
                                                    else
                                                    {
                                                        //Seta o efeito do Hit
                                                        objects.Add(new ObjectHitInfo(5)
                                                        {
                                                            ObjId = player.Id,
                                                            killerId = player.Id,
                                                            DeathType = (CharaDeathEnum)hit.DeathType,
                                                            ObjectLife = hit.HitPart
                                                        });
                                                    }
                                                }
                                                else
                                                {
                                                    playerRoom.life -= hit.WeaponDamage;
                                                    if (playerRoom.life <= 0)
                                                    {
                                                        //Seta o Death
                                                        playerRoom.life = 0;
                                                        playerRoom.isDead = true;
                                                        playerRoom.lastDie = DateTime.Now;
                                                        deaths.Add(new DeathServerData { slot = playerRoom, DeathType = (CharaDeathEnum)hit.DeathType });
                                                    }
                                                    else
                                                    {
                                                        //Seta o efeito do Hit
                                                        objects.Add(new ObjectHitInfo(5)
                                                        {
                                                            ObjId = playerRoom.Id,
                                                            killerId = player.Id,
                                                            DeathType = (CharaDeathEnum)hit.DeathType,
                                                            ObjectLife = hit.HitPart
                                                        });
                                                    }
                                                }
                                                if (hit.WeaponDamage > 0)
                                                {
                                                    objects.Add(new ObjectHitInfo(2)
                                                    {
                                                        ObjId = playerRoom.Id,
                                                        ObjectLife = playerRoom.life,
                                                        DeathType = (CharaDeathEnum)hit.DeathType,
                                                        WeaponId = ItemId,
                                                        HitPart = hit.HitPart
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                HitValid = false;
                                            }
                                        }
                                        else if (hit.ObjectType == ObjectTypeEnum.Object)
                                        {
                                            ObjectInfo objectInfo = room.GetObject(hit.WeaponObjectId);
                                            if (objectInfo != null)
                                            {
                                                ObjModel objectModel = objectInfo.info;
                                                if (objectModel != null && objectModel.IsDestroyable && objectInfo.life > 0)
                                                {
                                                    objectInfo.life -= hit.WeaponDamage;
                                                    if (objectInfo.life <= 0)
                                                    {
                                                        objectInfo.life = 0;
                                                        #region BOOM DEATH (BARRIL)
                                                        if (hit.BoomPlayers != null && hit.BoomPlayers.Count > 0)
                                                        {
                                                            for (int x = 0; x < hit.BoomPlayers.Count; x++)
                                                            {
                                                                int slotBoom = hit.BoomPlayers[x];
                                                                if (room.GetPlayer(slotBoom, out Slot battleplayer) && !player.isDead && battleplayer.teamId != player.teamId || battleplayer.teamId == player.teamId && player.Id == battleplayer.Id) //Bloqueio para o jogador não poder matar o time no barril
                                                                {
                                                                    //Seta o Death
                                                                    battleplayer.life = 0;
                                                                    battleplayer.isDead = true;
                                                                    battleplayer.lastDie = DateTime.Now;
                                                                    deaths.Add(new DeathServerData { slot = battleplayer, DeathType = CharaDeathEnum.OBJECT_EXPLOSION });
                                                                    objects.Add(new ObjectHitInfo(2)
                                                                    {
                                                                        HitPart = 1,
                                                                        DeathType = CharaDeathEnum.OBJECT_EXPLOSION,
                                                                        ObjId = slotBoom,
                                                                        killerId = battleplayer.Id,
                                                                        WeaponId = ItemId,
                                                                    });
                                                                }
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                    objectInfo.destroyState = objectModel.CheckDestroyState(objectInfo.life);

                                                    if (objectModel.UltraSYNC > 0 && (room.mode == RoomTypeEnum.Sabotage || room.mode == RoomTypeEnum.Defense))
                                                    {
                                                        if (objectModel.UltraSYNC == 1 || objectModel.UltraSYNC == 3)
                                                        {
                                                            room.Bar1 = objectInfo.life;
                                                        }
                                                        else if (objectModel.UltraSYNC == 2 || objectModel.UltraSYNC == 4)
                                                        {
                                                            room.Bar2 = objectInfo.life;
                                                        }
                                                        Sabotage(room, player, (ushort)hit.WeaponDamage, objectModel.UltraSYNC == 4 ? 2 : 1);
                                                    }

                                                    if (hit.WeaponDamage > 0)
                                                    {
                                                        objects.Add(new ObjectHitInfo(objectModel.UpdateId)
                                                        {
                                                            ObjId = objectInfo.objectId,
                                                            ObjectLife = objectInfo.life,
                                                            killerId = action.Slot,
                                                            ObjSyncId = objectModel.NeedSync ? 1 : 0,
                                                            AnimationId1 = objectModel.Anim1,
                                                            AnimationId2 = objectInfo.animation != null ? objectInfo.animation.Id : 255,
                                                            DestroyState = objectInfo.destroyState,
                                                            SpecialUse = GetTime(objectInfo.useDate),
                                                        });
                                                    }
                                                }
                                                else if (objectModel == null)
                                                {
                                                    Logger.Warning($" (Flag: BoomDamage) [Object] (ObjectModel) Object invalid! Obj: {hit.WeaponObjectId} Mapa: {room.mapId}.");
                                                }
                                            }
                                            else
                                            {
                                                Logger.Warning($" (Flag: BoomDamage) [Object] (ObjectInfo) Object invalid! Obj: {hit.WeaponObjectId} Mapa: {room.mapId}.");
                                            }
                                        }
                                        else if (hit.ObjectType == ObjectTypeEnum.UserObject)
                                        {
                                            int ownerSlot = hit.WeaponObjectId >> 4;
                                            int grenadeMapId = hit.WeaponObjectId & 15;
                                            Logger.Warning($" (Flag: BoomDamage) (UserObject) Damage: {hit.WeaponDamage} Player: {ownerSlot} MapObj: {grenadeMapId}");
                                        }
                                        else
                                        {
                                            Logger.Warning($" (Flag: BoomDamage) Unknown ObjectType: (" + hit.ObjectType + "/" + (int)hit.ObjectType + ")");
                                        }
                                        if (HitValid)
                                        {
                                            hits.Add(hit);
                                        }
                                        Logger.Battle($" (Flag: BoomDamage) HitsCount: {hitsCount} HitInfo: {hit.HitInfo} BoomInfo: {hit.BoomInfo} WeaponInfo: {hit.WeaponInfo} WeaponSlot: {hit.WeaponSlot} DeathType: {hit.DeathType} FirePos: {hit.FirePos} HitPos: {hit.HitPos} GrenadesCount: {hit.GrenadesCount}");
                                    }
                                    if (deaths.Count > 0)
                                    {
                                        Death(room, player, 255, ItemId, deaths);
                                    }
                                    sendFlags.WriteC((byte)hits.Count);
                                    for (int i = 0; i < hits.Count; i++)
                                    {
                                        HitDataBoomDamage hit = hits[i];
                                        sendFlags.WriteD(hit.HitInfo);
                                        sendFlags.WriteH(hit.BoomInfo);
                                        sendFlags.WriteH(hit.WeaponInfo);
                                        sendFlags.WriteC(hit.WeaponSlot);
                                        sendFlags.WriteC(hit.DeathType);
                                        sendFlags.WriteHVector(hit.FirePos);
                                        sendFlags.WriteHVector(hit.HitPos);
                                        sendFlags.WriteH(hit.GrenadesCount);
                                    }
                                    hits = null;
                                    deaths = null;
                                }
                                if (action.Flags.HasFlag(EventsEnum.Death)) //GetWeaponForHost
                                {
                                    flagsValue += 0x40000;
                                    byte DeathType = receiveFlags.ReadByte(); //(byte)(HitInfo.DeathType + (HitInfo.ObjId * 16));
                                    byte HitPart = receiveFlags.ReadByte();
                                    ushort PositionX = receiveFlags.ReadUshort();
                                    ushort PositionY = receiveFlags.ReadUshort();
                                    ushort PositionZ = receiveFlags.ReadUshort();
                                    int WeaponId = receiveFlags.ReadInt();

                                    Logger.Battle($" (Flag: Death) DeathType: {DeathType} HitPart: {HitPart} PositionX: {PositionX} PositionY: {PositionY} PositionZ: {PositionZ} WeaponId: {WeaponId}");

                                    sendFlags.WriteC(DeathType);
                                    sendFlags.WriteC(HitPart);
                                    sendFlags.WriteH(PositionX);
                                    sendFlags.WriteH(PositionY);
                                    sendFlags.WriteH(PositionZ);
                                    sendFlags.WriteD(WeaponId);
                                }
                                if (action.Flags.HasFlag(EventsEnum.SufferingDamage)) //Dano Sofrido
                                {
                                    flagsValue += 0x80000;
                                    byte hitEffects = receiveFlags.ReadByte();
                                    byte hitPart = receiveFlags.ReadByte();

                                    Logger.Battle($" (Flag: SufferingDamage) [Modo BOT?] HitEffects: {hitEffects} HitPart: {hitPart}");

                                    sendFlags.WriteC(hitEffects); //hitEffects (&15 = Qm deu o dano | >>4 = Tipo do dano (CHARA_DEATH))
                                    sendFlags.WriteC(hitPart); //hitPart (Número do efeito??) 
                                }
                                if (action.Flags.HasFlag(EventsEnum.PassPortal))
                                {
                                    flagsValue += 0x100000;
                                    ushort portal = receiveFlags.ReadUshort();

                                    Logger.Battle($" (Flag: PassPortal) portal: {portal}");

                                    sendFlags.WriteH(portal);
                                    if (!player.isDead)
                                    {
                                        room.PassPortal(player);
                                    }
                                }
                                if (flagsValue != (uint)action.Flags)
                                {
                                    Logger.Warning($" [BattleHandler] [WriteDataPlayer] Actions: ({(uint)action.Flags}|{(uint)action.Flags - flagsValue})");
                                }
                                byte[] BufferFlags = sendFlags.memory.ToArray();
                                send.GoBack(2);
                                send.WriteH((ushort)(BufferFlags.Length + 9));
                                send.WriteD((uint)action.Flags);
                                send.WriteB(BufferFlags);
                            }
                            if (action.Data.Length == 0 && (action.LengthData - 9 != 0))
                            {
                                break;
                            }
                        }
                        else if (SubHead == P2PSubHeadEnum.GRENADE)
                        {
                            var WeaponInfo = receive.ReadUshort();
                            var WeaponSlotId = receive.ReadByte();
                            var Unk1 = receive.ReadUshort();
                            var ObjectPositionX = receive.ReadUshort();
                            var ObjectPositionY = receive.ReadUshort();
                            var ObjectPositionZ = receive.ReadUshort();
                            var Unk2 = receive.ReadUshort();
                            var Unk3 = receive.ReadUshort();
                            var Unk4 = receive.ReadUshort();
                            var GrenadesCount = receive.ReadUshort();

                            Account playerCache = room.GetPlayerBySlot(slot);
                            SendLogToPlayer(playerCache, $" (P2PSubHead: GRENADE) (WriteDataPlayer) (Slot: {action.Slot}) ForSlot: {j} WeaponInfo: {WeaponInfo} WeaponSlotId: {WeaponSlotId} Unk1: {Unk1} ObjectPositionX: {ObjectPositionX} ObjectPositionY: {ObjectPositionY} ObjectPositionZ: {ObjectPositionZ} Unk2: {Unk2} Unk3: {Unk3} Unk4: {Unk4} GrenadesCount: {GrenadesCount}");

                            send.WriteH(WeaponInfo);
                            send.WriteC(WeaponSlotId);
                            send.WriteH(Unk1);
                            send.WriteH(ObjectPositionX);
                            send.WriteH(ObjectPositionY);
                            send.WriteH(ObjectPositionZ);
                            send.WriteH(Unk2);
                            send.WriteH(Unk3);
                            send.WriteH(Unk4);
                            send.WriteH(GrenadesCount);

                            if (Settings.ClientVersion == "1.15.42")
                            {
                                var Unk5 = receive.ReadB(6); //Mais 3 WriteH ?
                                Logger.Battle($" (P2PSubHead: GRENADE) (WriteDataPlayer) (Slot: {action.Slot}) (ClientVersion == 1.15.42) Unk5: {BitConverter.ToString(Unk5)}");

                                send.WriteB(Unk5); //id do item do mapa?
                            }
                        }
                        else if (SubHead == P2PSubHeadEnum.DROPEDWEAPON)
                        {
                            var WeaponFlag = receive.ReadByte();
                            var PositionX = receive.ReadUshort();
                            var PositionY = receive.ReadUshort();
                            var PositionZ = receive.ReadUshort();
                            var Unk1 = receive.ReadUshort();
                            var Unk2 = receive.ReadUshort();
                            var Unk3 = receive.ReadUshort();
                            var Unk4 = receive.ReadUshort();

                            Account playerCache = room.GetPlayerBySlot(slot);
                            SendLogToPlayer(playerCache, $" (P2PSubHead: DROPEDWEAPON) (WriteDataPlayer) (Slot: {action.Slot}) ForSlot: {j} WeaponFlag: {WeaponFlag} PositionX: {PositionX} PositionY: {PositionY} PositionZ: {PositionZ} Unk1: {Unk1} Unk2: {Unk2} Unk3: {Unk3} Unk4: {Unk4}");

                            send.WriteC(WeaponFlag);
                            send.WriteH(PositionX);
                            send.WriteH(PositionY);
                            send.WriteH(PositionZ);
                            send.WriteH(Unk1);
                            send.WriteH(Unk2);
                            send.WriteH(Unk3);
                            send.WriteH(Unk4);
                        }
                        else if (SubHead == P2PSubHeadEnum.OBJECT_STATIC)
                        {
                            var ObjectLife = receive.ReadUshort();
                            var DestroyedBySlot = receive.ReadByte();

                            Account playerCache = room.GetPlayerBySlot(slot);
                            SendLogToPlayer(playerCache, $" (P2PSubHead: OBJECT_STATIC) (WriteDataPlayer) (Slot: {action.Slot}) ForSlot: {j} ObjectLife: {ObjectLife} DestroyedBySlot: {DestroyedBySlot}");

                            send.WriteH(ObjectLife);
                            send.WriteC(DestroyedBySlot);
                        }
                        else if (SubHead == P2PSubHeadEnum.OBJECT_ANIM)
                        {
                            var ObjectLife = receive.ReadUshort();
                            var AnimationId1 = receive.ReadByte();
                            var AnimationId2 = receive.ReadByte();
                            var SyncDate = receive.ReadFloat();

                            Account playerCache = room.GetPlayerBySlot(slot);
                            SendLogToPlayer(playerCache, $" (P2PSubHead: OBJECT_ANIM) (WriteDataPlayer) (Slot: {action.Slot}) ForSlot: {j} ObjectLife: {ObjectLife} AnimationId1: {AnimationId1} AnimationId2: {AnimationId2} SyncDate: {SyncDate}");

                            send.WriteH(ObjectLife);
                            send.WriteC(AnimationId1);
                            send.WriteC(AnimationId2);
                            send.WriteT(SyncDate); //SpecialUse
                        }
                        else if (SubHead == P2PSubHeadEnum.STAGEINFO_OBJ_STATIC)
                        {
                            var isDestroyed = receive.ReadByte(); //1=true 0=false

                            Account playerCache = room.GetPlayerBySlot(slot);
                            SendLogToPlayer(playerCache, $" (P2PSubHead: STAGEINFO_OBJ_STATIC) (WriteDataPlayer) (Slot: {action.Slot}) ForSlot: {j} isDestroyed: {isDestroyed}");

                            send.WriteC(isDestroyed);
                        }
                        else if (SubHead == P2PSubHeadEnum.STAGEINFO_OBJ_ANIM)
                        {
                            var DestroyState = receive.ReadByte();
                            var ObjectLife = receive.ReadUshort();
                            var SyncDate = receive.ReadFloat(); //SpecialUse
                            var AnimationId1 = receive.ReadByte();
                            var AnimationId2 = receive.ReadByte();

                            Account playerCache = room.GetPlayerBySlot(slot);
                            SendLogToPlayer(playerCache, $" (P2PSubHead: STAGEINFO_OBJ_ANIM) (WriteDataPlayer) (Slot: {action.Slot}) ForSlot: {j} DestroyState: {DestroyState} ObjectLife: {ObjectLife} SyncDate: {SyncDate} AnimationId1: {AnimationId1} AnimationId2: {AnimationId2}");

                            send.WriteC(DestroyState);
                            send.WriteH(ObjectLife);
                            send.WriteT(SyncDate);
                            send.WriteC(AnimationId1);
                            send.WriteC(AnimationId2);
                        }
                        else if (SubHead == P2PSubHeadEnum.CONTROLED_OBJECT)
                        {
                            var Unk1 = receive.ReadB(9);

                            Account playerCache = room.GetPlayerBySlot(slot);
                            SendLogToPlayer(playerCache, $" (P2PSubHead: CONTROLED_OBJECT) (WriteDataPlayer) (Slot: {action.Slot}) ForSlot: {j} Unk1: {BitConverter.ToString(Unk1)}");

                            send.WriteB(Unk1); //160 existe no mapa OUTPOST
                        }
                        else
                        {
                            Logger.Warning($" [Battle] [WriteDataPlayer] Unknown ActionType: ({action.Type}/{(int)action.Type}) Buffer Data: {BitConverter.ToString(data)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($" [Battle] [WriteDataPlayer] Exception: {ex} Buffer Data: {BitConverter.ToString(data)}");
                        break;
                    }
                }
                for (int i = 0; i < objects.Count; i++)
                {
                    ObjectHitInfo HitInfo = objects[i];
                    if (HitInfo.SyncType == 1)
                    {
                        if (HitInfo.ObjSyncId == 0)
                        {
                            send.WriteC((byte)P2PSubHeadEnum.OBJECT_STATIC); //Action Type
                            send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                            send.WriteH(8); //Action Length

                            send.WriteH((ushort)HitInfo.ObjectLife); //ObjectLife
                            send.WriteC((byte)HitInfo.killerId); //DestroyedBySlot (Slot de quem destruiu o objeto).
                        }
                        else
                        {
                            send.WriteC((byte)P2PSubHeadEnum.OBJECT_ANIM); //Action Type
                            send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                            send.WriteH(13); //Action Length

                            send.WriteH((ushort)HitInfo.ObjectLife);
                            send.WriteC((byte)HitInfo.AnimationId1);
                            send.WriteC((byte)HitInfo.AnimationId2);
                            send.WriteT(HitInfo.SpecialUse);
                        }
                    }
                    else if (HitInfo.SyncType == 2)
                    {
                        EventsEnum events = EventsEnum.LifeSync;
                        ushort length = 11;
                        if (HitInfo.ObjectLife == 0)
                        {
                            events |= EventsEnum.Death;
                            length += 12;
                        }
                        send.WriteC((byte)P2PSubHeadEnum.USER); //Action Type
                        send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                        send.WriteH(length); //Action Length

                        send.WriteD((uint)events); //Action Events Flags
                        send.WriteH((ushort)HitInfo.ObjectLife);

                        if (events.HasFlag(EventsEnum.Death))
                        {
                            send.WriteC((byte)(HitInfo.DeathType + (HitInfo.ObjId * 16)));
                            send.WriteC((byte)HitInfo.HitPart);
                            send.WriteH(HitInfo.Position.X.RawValue);
                            send.WriteH(HitInfo.Position.Y.RawValue);
                            send.WriteH(HitInfo.Position.Z.RawValue);
                            send.WriteD(HitInfo.WeaponId);
                        }
                    }
                    else if (HitInfo.SyncType == 3)
                    {
                        if (HitInfo.ObjSyncId == 0)
                        {
                            send.WriteC((byte)P2PSubHeadEnum.STAGEINFO_OBJ_STATIC); //Action Type
                            send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                            send.WriteH(6); //Action Length

                            send.WriteC(HitInfo.ObjectLife == 0); //isDestroyed
                        }
                        else
                        {
                            send.WriteC((byte)P2PSubHeadEnum.STAGEINFO_OBJ_ANIM); //Action Type
                            send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                            send.WriteH(14); //Action Length

                            send.WriteC((byte)HitInfo.DestroyState);
                            send.WriteH((ushort)HitInfo.ObjectLife);
                            send.WriteT(HitInfo.SpecialUse);
                            send.WriteC((byte)HitInfo.AnimationId1);
                            send.WriteC((byte)HitInfo.AnimationId2);
                        }
                    }
                    else if (HitInfo.SyncType == 4)
                    {
                        send.WriteC((byte)P2PSubHeadEnum.STAGEINFO_CHARA); //Action Type
                        send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                        send.WriteH(11); //Action Length

                        send.WriteD((uint)EventsEnum.LifeSync);
                        send.WriteH((ushort)HitInfo.ObjectLife);
                    }
                    else if (HitInfo.SyncType == 5)
                    {
                        send.WriteC((byte)P2PSubHeadEnum.USER); //Action Type
                        send.WriteH((short)HitInfo.ObjId); //Action Slot
                        send.WriteH(11); //Action Length

                        send.WriteD((uint)EventsEnum.SufferingDamage); //Dano Sofrido
                        send.WriteC((byte)(HitInfo.killerId + ((byte)HitInfo.DeathType * 16)));
                        send.WriteC((byte)HitInfo.ObjectLife);
                    }
                }
                return send.memory.ToArray();
            }
        }

        public byte[] WriteDataBot(byte[] data, Slot slot, Room room)
        {
            List<ObjectHitInfo> objects = new List<ObjectHitInfo>();
            BattlePacketReader receive = new BattlePacketReader(data);
            using (BattlePacketWriter send = new BattlePacketWriter())
            {
                for (int j = 0; j < 16; j++)
                {
                    ActionModel action = new ActionModel();
                    try
                    {
                        action.Type = receive.ReadByteOutException(out bool exception);
                        if (exception)
                        {
                            break;
                        }
                        action.Slot = receive.ReadUshort();
                        action.LengthData = receive.ReadUshort();
                        if (action.LengthData == 65535)
                        {
                            break;
                        }
                        send.WriteC(action.Type);
                        send.WriteH(action.Slot);
                        send.WriteH(action.LengthData);

                        P2PSubHeadEnum SubHead = (P2PSubHeadEnum)action.Type;
                        if (SubHead == P2PSubHeadEnum.USER || SubHead == P2PSubHeadEnum.STAGEINFO_CHARA)
                        {
                            action.Flags = (EventsEnum)receive.ReadUint();
                            action.Data = receive.ReadB(action.LengthData - 9);
                            Logger.Battle($" (P2PSubHead: {SubHead}) (WriteDataBot) (Slot: {action.Slot}) ActionFlags: {action.Flags}");
                            Slot player = room.GetSlot(action.Slot);
                            uint flagsValue = 0;
                            BattlePacketReader receiveFlags = new BattlePacketReader(action.Data);
                            using (BattlePacketWriter sendFlags = new BattlePacketWriter())
                            {
                                if (action.Flags.HasFlag(EventsEnum.ActionState))
                                {
                                    flagsValue++;
                                    uint Unknown1 = receiveFlags.ReadUint();

                                    uint code = Unknown1 & 3;
                                    uint code1 = (Unknown1 >> 2) & 1; //all >> 2 & 1
                                    uint code2 = (Unknown1 >> 10) & 0xF; //all >> 10 & 0xF
                                    uint code3 = (Unknown1 >> 14) & 1; //all >> 14 & 1
                                    uint code4 = (Unknown1 >> 15) & 1; //all >> 15 & 1
                                    uint code5 = (Unknown1 >> 16) & 0xF; //all >> 16 & 0xF
                                    uint code6 = (Unknown1 >> 20) & 0xFF; //all >> 20 & 0xFF

                                    Logger.Battle($" (Flag: ActionState) (Slot: {action.Slot}) Unknown1: {Unknown1} code: {code} code1: {code1} code2: {code2} code3: {code3} code4: {code4} code5: {code5} code6: {code6}");

                                    sendFlags.WriteD(Unknown1);
                                }
                                if (action.Flags.HasFlag(EventsEnum.Animation))
                                {
                                    flagsValue += 2;
                                    ushort positionVertical = receiveFlags.ReadUshort();
                                    sendFlags.WriteH(positionVertical);

                                    Logger.Battle($" (Flag: Animation) positionVertical: {positionVertical}");
                                }
                                if (action.Flags.HasFlag(EventsEnum.PosRotation))
                                {
                                    flagsValue += 4;
                                    var RotationX = receiveFlags.ReadUshort();
                                    var RotationY = receiveFlags.ReadUshort();
                                    var RotationZ = receiveFlags.ReadUshort();
                                    var CameraX = receiveFlags.ReadUshort();
                                    var CameraY = receiveFlags.ReadUshort();
                                    var Area = receiveFlags.ReadUshort();

                                    sendFlags.WriteH(RotationX);
                                    sendFlags.WriteH(RotationY);
                                    sendFlags.WriteH(RotationZ);
                                    sendFlags.WriteH(CameraX);
                                    sendFlags.WriteH(CameraY);
                                    sendFlags.WriteH(Area);

                                    Logger.Battle($" (Flag: PosRotation) RotationX: {RotationX} RotationY: {RotationY} RotationZ: {RotationZ} CameraX: {CameraX} CameraY: {CameraY} Area: {Area}");
                                }
                                if (action.Flags.HasFlag(EventsEnum.OnLoadObject))
                                {
                                    flagsValue += 8;
                                    var SpaceflagsValue = receiveFlags.ReadByte();
                                    var ObjectId = receiveFlags.ReadUshort();
                                    CharaMovesEnum SpaceFlags = (CharaMovesEnum)SpaceflagsValue;
                                    Logger.Battle($" (Flag: OnLoadObject) SpaceFlags: {SpaceFlags} ObjectId: {ObjectId}");
                                    ObjectInfo objectInfo = room.GetObject(ObjectId);
                                    if (objectInfo != null)
                                    {
                                        //ObjectId != 65535
                                        //SendLogToPlayer(playerCache, $"[OnLoadObject] ({SpaceFlags}) ObjectId: {ObjectId} Heli: {(ObjectId == 50 ? "RED" : ObjectId == 49 ? "BLUE" : "UNK")} animationId: {objectInfo.animation.Id} NextAnim: {objectInfo.animation.NextAnim} OtherAnim: {objectInfo.animation.OtherAnim} OtherObj: {objectInfo.animation.OtherObj} Duration: {objectInfo.animation.Duration} useDate: {objectInfo.useDate}");
                                        if (SpaceFlags.HasFlag(CharaMovesEnum.STOPPED))
                                        {

                                        }
                                        if (SpaceFlags.HasFlag(CharaMovesEnum.LEFT))
                                        {

                                        }
                                        if (SpaceFlags.HasFlag(CharaMovesEnum.BACK))
                                        {

                                        }
                                        if (SpaceFlags.HasFlag(CharaMovesEnum.RIGHT))
                                        {

                                        }
                                        if (SpaceFlags.HasFlag(CharaMovesEnum.FRONT))
                                        {

                                        }
                                        if (SpaceFlags.HasFlag(CharaMovesEnum.HELI_IN_MOVE))
                                        {
                                            Logger.Warning(" (Flag: OnLoadObject) HELI_IN_MOVE: Date: " + objectInfo.useDate.ToString("yyMMddHHmm"));
                                            if (objectInfo.useDate.ToString("yyMMddHHmm") == "0101010000")
                                            {

                                            }
                                        }
                                        if (SpaceFlags.HasFlag(CharaMovesEnum.HELI_UNKNOWN)) //Quando aperta E para sair do modo atirador do Helicoptero
                                        {

                                        }
                                        if (SpaceFlags.HasFlag(CharaMovesEnum.HELI_LEAVE)) //Quando aperta E para sair do modo atirador do Helicoptero
                                        {

                                        }
                                        if (SpaceFlags.HasFlag(CharaMovesEnum.HELI_STOPPED))
                                        {
                                            //if (anim != null && anim.Id == 0)
                                            //{
                                            //    objectInfo.info.GetAnim(anim.NextAnim, 0, 0, objectInfo);
                                            //}
                                        }
                                        objects.Add(new ObjectHitInfo(3)
                                        {
                                            ObjSyncId = 1,
                                            ObjId = objectInfo.objectId,
                                            ObjectLife = objectInfo.life,
                                            AnimationId1 = 255,
                                            AnimationId2 = objectInfo.animation != null ? objectInfo.animation.Id : 255,
                                            SpecialUse = GetTime(objectInfo.useDate)
                                        });
                                    }
                                    sendFlags.WriteC(SpaceflagsValue);
                                    sendFlags.WriteH(ObjectId);
                                }
                                if (action.Flags.HasFlag(EventsEnum.Unk1))
                                {
                                    flagsValue += 0x10;
                                    byte Unknown1 = receiveFlags.ReadByte();
                                    byte Unknown2 = receiveFlags.ReadByte();

                                    Logger.Battle($" (Flag: Unk1) Unknown1: {Unknown1} Unknown2: {Unknown2}");

                                    sendFlags.WriteC(Unknown1);
                                    sendFlags.WriteC(Unknown2);
                                }
                                if (action.Flags.HasFlag(EventsEnum.RadioChat))
                                {
                                    flagsValue += 0x20;
                                    var RadioId = receiveFlags.ReadByte();
                                    var Area = receiveFlags.ReadByte();

                                    Logger.Battle($" (Flag: RadioChat) RadioId: {RadioId} Area: {Area}");

                                    sendFlags.WriteC(RadioId);
                                    sendFlags.WriteC(Area);
                                }
                                if (action.Flags.HasFlag(EventsEnum.WeaponSync))
                                {
                                    flagsValue += 0x40;
                                    var weaponInfo = receiveFlags.ReadUshort();
                                    var weaponSlotInfo = receiveFlags.ReadByte();
                                    var charaModelId = receiveFlags.ReadByte();

                                    //var WeaponSecondMelee = weaponSlotInfo >> 4;
                                    var WeaponSlot = weaponSlotInfo & 15;
                                    var WeaponNumber = (weaponInfo >> 6) & 1023;
                                    var WeaponClass = weaponInfo & 63;

                                    Logger.Battle($" (Flag: WeaponSync) WeaponInfo: {weaponInfo} WeaponSlotInfo: {weaponSlotInfo} CharaModelId: {charaModelId}");
                                    Logger.Battle($" (Flag: WeaponSync) (OBS: Valores Gerados) WeaponSlot: {WeaponSlot} WeaponNumber: {WeaponNumber} WeaponClass: {WeaponClass}");

                                    sendFlags.WriteH(weaponInfo);
                                    sendFlags.WriteC(weaponSlotInfo);
                                    sendFlags.WriteC(charaModelId); //VALOR > 0 && VALOR < 43

                                    room.SyncInfo(objects, 2);
                                }
                                if (action.Flags.HasFlag(EventsEnum.WeaponRecoil))
                                {
                                    flagsValue += 0x80;
                                    float RecoilHorzAngle = receiveFlags.ReadFloat();
                                    float RecoilHorzMax = receiveFlags.ReadFloat();
                                    float RecoilVertAngle = receiveFlags.ReadFloat();
                                    float RecoilVertMax = receiveFlags.ReadFloat();
                                    float Deviation = receiveFlags.ReadFloat();
                                    ushort WeaponId = receiveFlags.ReadUshort();
                                    byte WeaponSlot = receiveFlags.ReadByte();
                                    byte Unkwnon = receiveFlags.ReadByte(); ///DeviationMax? ou BulletWeight(Peso da bala-QUASE CERTEZA)
                                    byte RecoilHorzCount = receiveFlags.ReadByte();

                                    Logger.Battle($" (Flag: WeaponRecoil) (WeaponId/WeaponSlot): {WeaponId}/{WeaponSlot} Horz(Angle/Max/Count): {RecoilHorzAngle}/{RecoilHorzMax}/{RecoilHorzCount} Vert(Angle/Max): {RecoilVertAngle}/{RecoilVertMax} Deviation: {Deviation} Unk: {Unkwnon}");

                                    sendFlags.WriteT(RecoilHorzAngle);
                                    sendFlags.WriteT(RecoilHorzMax);
                                    sendFlags.WriteT(RecoilVertAngle);
                                    sendFlags.WriteT(RecoilVertMax);
                                    sendFlags.WriteT(Deviation);
                                    sendFlags.WriteH(WeaponId);
                                    sendFlags.WriteC(WeaponSlot);
                                    sendFlags.WriteC(Unkwnon);
                                    sendFlags.WriteC(RecoilHorzCount);
                                }
                                if (action.Flags.HasFlag(EventsEnum.LifeSync)) //Apenas no modo BOT
                                {
                                    flagsValue += 0x100;
                                    ushort ObjectLife = receiveFlags.ReadUshort();
                                    Logger.Battle($" (Flag: LifeSync) ObjectLife: {ObjectLife}");
                                    sendFlags.WriteH(ObjectLife);
                                }
                                if (action.Flags.HasFlag(EventsEnum.TakeWeapon)) //TakeWeapon = Pegar Arma
                                {
                                    flagsValue += 0x800;
                                    byte weaponFlag = receiveFlags.ReadByte();
                                    byte weaponClass = receiveFlags.ReadByte();
                                    ushort weaponId = receiveFlags.ReadUshort();
                                    byte ammoPrin = receiveFlags.ReadByte();
                                    byte ammoDual = receiveFlags.ReadByte();
                                    ushort ammoTotal = receiveFlags.ReadUshort();

                                    sendFlags.WriteC(weaponFlag);
                                    sendFlags.WriteC(weaponClass);
                                    sendFlags.WriteH(weaponId);
                                    if (Settings.UseMaxAmmoInDrop)
                                    {
                                        sendFlags.WriteC(255);
                                        sendFlags.WriteC(ammoDual);
                                        sendFlags.WriteH(10000);
                                    }
                                    else
                                    {
                                        sendFlags.WriteC(ammoPrin);
                                        sendFlags.WriteC(ammoDual);
                                        sendFlags.WriteH(ammoTotal);
                                    }
                                }
                                if (action.Flags.HasFlag(EventsEnum.DropWeapon)) //DropWeapon = Soltar Arma
                                {
                                    flagsValue += 0x1000;
                                    if (!room.IsCase4BotMode)
                                    {
                                        room.DropCounter++;
                                        if (room.DropCounter > Settings.MaxDrop)
                                        {
                                            room.DropCounter = 0;
                                        }
                                    }
                                    byte weaponFlag = receiveFlags.ReadByte();
                                    byte weaponClass = receiveFlags.ReadByte();
                                    ushort weaponId = receiveFlags.ReadUshort();
                                    byte ammoPrin = receiveFlags.ReadByte();
                                    byte ammoDual = receiveFlags.ReadByte();
                                    ushort ammoTotal = receiveFlags.ReadUshort();

                                    sendFlags.WriteC((byte)(weaponFlag + room.DropCounter));
                                    sendFlags.WriteC(weaponClass);
                                    sendFlags.WriteH(weaponId);

                                    if (Settings.UseMaxAmmoInDrop)
                                    {
                                        sendFlags.WriteC(255);
                                        sendFlags.WriteC(ammoDual);
                                        sendFlags.WriteH(10000);
                                    }
                                    else
                                    {
                                        sendFlags.WriteC(ammoPrin);
                                        sendFlags.WriteC(ammoDual);
                                        sendFlags.WriteH(ammoTotal);
                                    }
                                }
                                if (action.Flags.HasFlag(EventsEnum.FireSync)) //FireSync = Sincronização do Tiro (Apenas no modo BOT?)
                                {
                                    flagsValue += 0x2000;
                                    ushort shotId = receiveFlags.ReadUshort();
                                    ushort shotIndex = receiveFlags.ReadUshort();
                                    ushort camX = receiveFlags.ReadUshort();
                                    ushort camY = receiveFlags.ReadUshort();
                                    ushort camZ = receiveFlags.ReadUshort();
                                    int weaponNumber = receiveFlags.ReadInt(); //weaponId

                                    Logger.Battle($" (Flag: FireSync) (Id/Index): {shotId}/{shotIndex} WeaponNumber: {weaponNumber} camX: {camX} camY: {camY} camZ: {camZ}");

                                    sendFlags.WriteH(shotId);
                                    sendFlags.WriteH(shotIndex);
                                    sendFlags.WriteH(camX);
                                    sendFlags.WriteH(camY);
                                    sendFlags.WriteH(camZ);
                                    sendFlags.WriteD(weaponNumber);
                                }
                                if (action.Flags.HasFlag(EventsEnum.AIDamage)) //Artificial Intelligence Damage (MODO BOT, ZUMBI)
                                {
                                    flagsValue += 0x4000;
                                    int hitsCount = receiveFlags.ReadByte();
                                    sendFlags.WriteC((byte)hitsCount);
                                    for (int i = 0; i < hitsCount; i++)
                                    {
                                        uint hitInfo = receiveFlags.ReadUint();
                                        ushort WeaponInfo = receiveFlags.ReadUshort();
                                        byte WeaponSlot = receiveFlags.ReadByte();
                                        ushort Unknown = receiveFlags.ReadUshort();
                                        ushort EixoX = receiveFlags.ReadUshort();
                                        ushort EixoY = receiveFlags.ReadUshort();
                                        ushort EixoZ = receiveFlags.ReadUshort();

                                        ClassTypeEnum WeaponClass = (ClassTypeEnum)(WeaponInfo & 63);
                                        int WeaponId = WeaponInfo >> 6;
                                        int WeaponDamage = (int)(hitInfo >> 21);

                                        WeaponInfo myWeapon = WeaponsXML.GetWeapon(WeaponId, (int)WeaponClass, WeaponSlot);
                                        int PercentualDamage = myWeapon.damage + (myWeapon.damage * 30 / 100);
                                        if (myWeapon.usageType == 0 && WeaponDamage > PercentualDamage) //PRIMARY
                                        {
                                            Logger.Warning($" (Flag: AIDamage) SET HIT [!] (PRIMARY) HitDamage: {WeaponDamage}/{myWeapon.damage} Name: {myWeapon.name} PlayerId: {slot.playerId} EndPoint: {slot.client}");
                                            WeaponDamage = myWeapon.damage;
                                        }
                                        if (myWeapon.usageType == 1 && WeaponDamage > PercentualDamage) //SECONDARY
                                        {
                                            Logger.Warning($" (Flag: AIDamage) SET HIT [!] (SECONDARY) HitDamage: {WeaponDamage}/{myWeapon.damage} Name: {myWeapon.name} PlayerId: {slot.playerId} EndPoint: {slot.client}");
                                            WeaponDamage = myWeapon.damage;
                                        }
                                        if (myWeapon.usageType == 2 && WeaponDamage > myWeapon.damage) //MEELE
                                        {
                                            Logger.Warning($" (Flag: AIDamage) SET HIT [!] (MEELE) HitDamage: {WeaponDamage}/{myWeapon.damage} Name: {myWeapon.name} PlayerId: {slot.playerId} EndPoint: {slot.client}");
                                            WeaponDamage = myWeapon.damage;
                                        }

                                        Logger.Battle($" (Flag: AIDamage) HitsCount: {hitsCount} HitInfo: {hitInfo} WeaponInfo: {WeaponInfo} WeaponSlot: {WeaponSlot} Unknown: {Unknown} EixoX: {EixoX} EixoY: {EixoY} EixoZ: {EixoZ}");

                                        sendFlags.WriteD(hitInfo);
                                        sendFlags.WriteH(WeaponInfo);
                                        sendFlags.WriteC(WeaponSlot);
                                        sendFlags.WriteH(Unknown);
                                        sendFlags.WriteH(EixoX);
                                        sendFlags.WriteH(EixoY);
                                        sendFlags.WriteH(EixoZ);
                                    }
                                }
                                if (action.Flags.HasFlag(EventsEnum.Death))
                                {
                                    flagsValue += 0x40000;
                                    byte DeathType = receiveFlags.ReadByte(); //(byte)(HitInfo.DeathType + (HitInfo.ObjId * 16));
                                    byte HitPart = receiveFlags.ReadByte();
                                    ushort PositionX = receiveFlags.ReadUshort();
                                    ushort PositionY = receiveFlags.ReadUshort();
                                    ushort PositionZ = receiveFlags.ReadUshort();
                                    int WeaponId = receiveFlags.ReadInt();

                                    Logger.Battle($" (Flag: Death) DeathType: {DeathType} HitPart: {HitPart} PositionX: {PositionX} PositionY: {PositionY} PositionZ: {PositionZ} WeaponId: {WeaponId}");

                                    sendFlags.WriteC(DeathType);
                                    sendFlags.WriteC(HitPart);
                                    sendFlags.WriteH(PositionX);
                                    sendFlags.WriteH(PositionY);
                                    sendFlags.WriteH(PositionZ);
                                    sendFlags.WriteD(WeaponId);
                                }
                                if (action.Flags.HasFlag(EventsEnum.SufferingDamage)) //Dano Sofrido
                                {
                                    flagsValue += 0x80000;
                                    byte hitEffects = receiveFlags.ReadByte();
                                    byte hitPart = receiveFlags.ReadByte();

                                    sendFlags.WriteC(hitEffects); //hitEffects (&15 = Qm deu o dano | >>4 = Tipo do dano (CHARA_DEATH))
                                    sendFlags.WriteC(hitPart); //hitPart (Número do efeito??)
                                    Logger.Battle($" (Flag: SufferingDamage) [Modo BOT?] HitEffects: {hitEffects} HitPart: {hitPart}");
                                }
                                if (flagsValue != (uint)action.Flags)
                                {
                                    Logger.Warning($" [BattleHandler] [WriteDataBot] Actions: ({(uint)action.Flags}|{(uint)action.Flags - flagsValue})");
                                }
                                byte[] BufferFlags = sendFlags.memory.ToArray();
                                send.GoBack(2);
                                send.WriteH((ushort)(BufferFlags.Length + 9));
                                send.WriteD((uint)action.Flags);
                                send.WriteB(BufferFlags);
                            }
                            if (action.Data.Length == 0 && (action.LengthData - 9 != 0))
                            {
                                break;
                            }
                        }
                        else if (SubHead == P2PSubHeadEnum.GRENADE)
                        {
                            var WeaponInfo = receive.ReadUshort();
                            var WeaponSlotId = receive.ReadByte();
                            var Unk1 = receive.ReadUshort();
                            var ObjectPositionX = receive.ReadUshort();
                            var ObjectPositionY = receive.ReadUshort();
                            var ObjectPositionZ = receive.ReadUshort();
                            var Unk2 = receive.ReadUshort();
                            var Unk3 = receive.ReadUshort();
                            var Unk4 = receive.ReadUshort();
                            var GrenadesCount = receive.ReadUshort();

                            Logger.Battle($" (P2PSubHead: GRENADE) (WriteDataBot) (Slot: {action.Slot}) WeaponInfo: {WeaponInfo} WeaponSlotId: {WeaponSlotId} Unk1: {Unk1} ObjectPositionX: {ObjectPositionX} ObjectPositionY: {ObjectPositionY} ObjectPositionZ: {ObjectPositionZ} Unk2: {Unk2} Unk3: {Unk3} Unk4: {Unk4} GrenadesCount: {GrenadesCount}");

                            send.WriteH(WeaponInfo);
                            send.WriteC(WeaponSlotId);
                            send.WriteH(Unk1);
                            send.WriteH(ObjectPositionX);
                            send.WriteH(ObjectPositionY);
                            send.WriteH(ObjectPositionZ);
                            send.WriteH(Unk2);
                            send.WriteH(Unk3);
                            send.WriteH(Unk4);
                            send.WriteH(GrenadesCount);

                            if (Settings.ClientVersion == "1.15.42")
                            {
                                var Unk5 = receive.ReadB(6);
                                Logger.Battle($" (P2PSubHead: GRENADE) (WriteDataBot) (Slot: {action.Slot}) (ClientVersion == 1.15.42) Unk5: {BitConverter.ToString(Unk5)}");

                                send.WriteB(Unk5); //id do item do mapa?
                            }
                        }
                        else if (SubHead == P2PSubHeadEnum.DROPEDWEAPON)
                        {
                            var WeaponFlag = receive.ReadByte();
                            var PositionX = receive.ReadUshort();
                            var PositionY = receive.ReadUshort();
                            var PositionZ = receive.ReadUshort();
                            var Unk1 = receive.ReadUshort();
                            var Unk2 = receive.ReadUshort();
                            var Unk3 = receive.ReadUshort();
                            var Unk4 = receive.ReadUshort();

                            Logger.Battle($" (P2PSubHead: DROPEDWEAPON) (WriteDataBot) (Slot: {action.Slot}) WeaponFlag: {WeaponFlag} PositionX: {PositionX} PositionY: {PositionY} PositionZ: {PositionZ} Unk1: {Unk1} Unk2: {Unk2} Unk3: {Unk3} Unk4: {Unk4}");

                            send.WriteC(WeaponFlag);
                            send.WriteH(PositionX);
                            send.WriteH(PositionY);
                            send.WriteH(PositionZ);
                            send.WriteH(Unk1);
                            send.WriteH(Unk2);
                            send.WriteH(Unk3);
                            send.WriteH(Unk4);
                        }
                        else if (SubHead == P2PSubHeadEnum.OBJECT_STATIC)
                        {
                            var ObjectLife = receive.ReadUshort();
                            var DestroyedBySlot = receive.ReadByte();

                            Logger.Battle($" (P2PSubHead: OBJECT_STATIC) (WriteDataBot) (Slot: {action.Slot}) ObjectLife: {ObjectLife} DestroyedBySlot: {DestroyedBySlot}");

                            send.WriteH(ObjectLife);
                            send.WriteC(DestroyedBySlot);
                        }
                        else if (SubHead == P2PSubHeadEnum.OBJECT_ANIM)
                        {
                            var ObjectLife = receive.ReadUshort();
                            var AnimationId1 = receive.ReadByte();
                            var AnimationId2 = receive.ReadByte();
                            var SyncDate = receive.ReadFloat();

                            Logger.Battle($" (P2PSubHead: OBJECT_ANIM) (WriteDataBot) (Slot: {action.Slot}) ObjectLife: {ObjectLife} AnimationId1: {AnimationId1} AnimationId2: {AnimationId2} SyncDate: {SyncDate}");

                            send.WriteH(ObjectLife);
                            send.WriteC(AnimationId1);
                            send.WriteC(AnimationId2);
                            send.WriteT(SyncDate); //SpecialUse
                        }
                        else if (SubHead == P2PSubHeadEnum.STAGEINFO_OBJ_STATIC)
                        {
                            var isDestroyed = receive.ReadByte(); //1=true 0=false

                            Logger.Battle($" (P2PSubHead: STAGEINFO_OBJ_STATIC) (WriteDataBot) (Slot: {action.Slot}) isDestroyed: {isDestroyed}");

                            send.WriteC(isDestroyed);
                        }
                        else if (SubHead == P2PSubHeadEnum.STAGEINFO_OBJ_ANIM)
                        {
                            var DestroyState = receive.ReadByte();
                            var ObjectLife = receive.ReadUshort();
                            var SyncDate = receive.ReadFloat(); //SpecialUse
                            var AnimationId1 = receive.ReadByte();
                            var AnimationId2 = receive.ReadByte();

                            Logger.Battle($" (P2PSubHead: STAGEINFO_OBJ_ANIM) (WriteDataBot) (Slot: {action.Slot}) DestroyState: {DestroyState} ObjectLife: {ObjectLife} SyncDate: {SyncDate} AnimationId1: {AnimationId1} AnimationId2: {AnimationId2}");

                            send.WriteC(DestroyState);
                            send.WriteH(ObjectLife);
                            send.WriteT(SyncDate);
                            send.WriteC(AnimationId1);
                            send.WriteC(AnimationId2);
                        }
                        else if (SubHead == P2PSubHeadEnum.CONTROLED_OBJECT)
                        {
                            var Unk1 = receive.ReadB(9);
                            Logger.Battle($" (P2PSubHead: CONTROLED_OBJECT) (WriteDataBot) (Slot: {action.Slot}) Unk1: {BitConverter.ToString(Unk1)}");

                            send.WriteB(Unk1); //160 existe no mapa OUTPOST
                        }
                        else
                        {
                            Logger.Warning($" [Battle] [WriteDataBot] Unknown ActionType: ({action.Type}/{(int)action.Type}) Buffer Data: {BitConverter.ToString(data)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($" [Battle] [WriteDataBot] Exception: {ex} Buffer Data: {BitConverter.ToString(data)}");
                        break;
                    }
                }
                for (int i = 0; i < objects.Count; i++)
                {
                    ObjectHitInfo HitInfo = objects[i];
                    if (HitInfo.SyncType == 1)
                    {
                        if (HitInfo.ObjSyncId == 0)
                        {
                            send.WriteC((byte)P2PSubHeadEnum.OBJECT_STATIC); //Action Type
                            send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                            send.WriteH(8); //Action Length

                            send.WriteH((ushort)HitInfo.ObjectLife); //ObjectLife
                            send.WriteC((byte)HitInfo.killerId); //DestroyedBySlot (Slot de quem destruiu o objeto).
                        }
                        else
                        {
                            send.WriteC((byte)P2PSubHeadEnum.OBJECT_ANIM); //Action Type
                            send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                            send.WriteH(13); //Action Length

                            send.WriteH((ushort)HitInfo.ObjectLife);
                            send.WriteC((byte)HitInfo.AnimationId1);
                            send.WriteC((byte)HitInfo.AnimationId2);
                            send.WriteT(HitInfo.SpecialUse);
                        }
                    }
                    else if (HitInfo.SyncType == 2)
                    {
                        EventsEnum events = EventsEnum.LifeSync;
                        ushort length = 11;
                        if (HitInfo.ObjectLife == 0)
                        {
                            events |= EventsEnum.Death;
                            length += 12;
                        }
                        send.WriteC((byte)P2PSubHeadEnum.USER); //Action Type
                        send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                        send.WriteH(length); //Action Length

                        send.WriteD((uint)events); //Action Events Flags
                        send.WriteH((ushort)HitInfo.ObjectLife);

                        if (events.HasFlag(EventsEnum.Death))
                        {
                            send.WriteC((byte)(HitInfo.DeathType + (HitInfo.ObjId * 16)));
                            send.WriteC((byte)HitInfo.HitPart);
                            send.WriteH(HitInfo.Position.X.RawValue);
                            send.WriteH(HitInfo.Position.Y.RawValue);
                            send.WriteH(HitInfo.Position.Z.RawValue);
                            send.WriteD(HitInfo.WeaponId);
                        }
                    }
                    else if (HitInfo.SyncType == 3)
                    {
                        if (HitInfo.ObjSyncId == 0)
                        {
                            send.WriteC((byte)P2PSubHeadEnum.STAGEINFO_OBJ_STATIC); //Action Type
                            send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                            send.WriteH(6); //Action Length

                            send.WriteC(HitInfo.ObjectLife == 0); //isDestroyed
                        }
                        else
                        {
                            send.WriteC((byte)P2PSubHeadEnum.STAGEINFO_OBJ_ANIM); //Action Type
                            send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                            send.WriteH(14); //Action Length

                            send.WriteC((byte)HitInfo.DestroyState);
                            send.WriteH((ushort)HitInfo.ObjectLife);
                            send.WriteT(HitInfo.SpecialUse);
                            send.WriteC((byte)HitInfo.AnimationId1);
                            send.WriteC((byte)HitInfo.AnimationId2);
                        }
                    }
                    else if (HitInfo.SyncType == 4)
                    {
                        send.WriteC((byte)P2PSubHeadEnum.STAGEINFO_CHARA); //Action Type
                        send.WriteH((ushort)HitInfo.ObjId); //Action Slot
                        send.WriteH(11); //Action Length

                        send.WriteD((uint)EventsEnum.LifeSync);
                        send.WriteH((ushort)HitInfo.ObjectLife);
                    }
                    else if (HitInfo.SyncType == 5)
                    {
                        send.WriteC((byte)P2PSubHeadEnum.USER); //Action Type
                        send.WriteH((short)HitInfo.ObjId); //Action Slot
                        send.WriteH(11); //Action Length

                        send.WriteD((uint)EventsEnum.SufferingDamage); //Dano Sofrido
                        send.WriteC((byte)(HitInfo.killerId + ((byte)HitInfo.DeathType * 16)));
                        send.WriteC((byte)HitInfo.ObjectLife);
                    }
                }
                return send.memory.ToArray();
            }
        }

        public byte[] WriteDataIA(byte[] data)
        {
            BattlePacketReader receive = new BattlePacketReader(data);
            using (BattlePacketWriter send = new BattlePacketWriter())
            {
                send.WriteT(receive.ReadFloat()); //Time?
                for (int i = 0; i < 16; i++)
                {
                    ActionModel action = new ActionModel();
                    try
                    {
                        action.Type = receive.ReadByteOutException(out bool exception);
                        //Logger.Warning($" WriteData132 FOR {i} EX: {exception}");
                        if (exception)
                        {
                            break;
                        }
                        action.Slot = receive.ReadUshort();
                        action.LengthData = receive.ReadUshort();
                        if (action.LengthData == 65535)
                        {
                            break;
                        }
                        send.WriteC(action.Type);
                        send.WriteH(action.Slot);
                        send.WriteH(action.LengthData);

                        P2PSubHeadEnum SubHead = (P2PSubHeadEnum)action.Type;
                        if (SubHead == P2PSubHeadEnum.USER || SubHead == P2PSubHeadEnum.STAGEINFO_CHARA)
                        {
                            action.Flags = (EventsEnum)receive.ReadUint();
                            action.Data = receive.ReadB(action.LengthData - 9);
                            send.WriteD((uint)action.Flags);
                            send.WriteB(action.Data);
                            if (action.Data.Length == 0 && action.Flags != 0)
                            {
                                break;
                            }
                        }
                        else if (SubHead == P2PSubHeadEnum.GRENADE)
                        {
                            var Unk1 = receive.ReadB(Settings.ClientVersion == "1.15.37" ? 19 : 25);
                            send.WriteB(Unk1);
                        }
                        else if (SubHead == P2PSubHeadEnum.DROPEDWEAPON)
                        {
                            var Unk1 = receive.ReadB(15);
                            send.WriteB(Unk1);
                        }
                        else if (SubHead == P2PSubHeadEnum.OBJECT_STATIC)
                        {
                            var Unk1 = receive.ReadB(3);
                            send.WriteB(Unk1);
                        }
                        else if (SubHead == P2PSubHeadEnum.OBJECT_ANIM)
                        {
                            var Unk1 = receive.ReadB(8);
                            send.WriteB(Unk1);
                        }
                        else if (SubHead == P2PSubHeadEnum.STAGEINFO_OBJ_STATIC)
                        {
                            var IsDestroyed = receive.ReadByte(); //1=true 0=false
                            send.WriteC(IsDestroyed);
                        }
                        else if (SubHead == P2PSubHeadEnum.STAGEINFO_OBJ_ANIM)
                        {
                            var Unk1 = receive.ReadB(9);
                            send.WriteB(Unk1);
                        }
                        else if (SubHead == P2PSubHeadEnum.CONTROLED_OBJECT)
                        {
                            var Unk1 = receive.ReadB(9);
                            send.WriteB(Unk1); //160 existe no mapa OUTPOST
                        }
                        else
                        {
                            Logger.Warning($" [Battle] [WriteData132] Unknown Type: {action.Type} Buffer Data: {BitConverter.ToString(data)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($" [Battle] [WriteData132] Exception: {ex}");
                        break;
                    }
                }
                return send.memory.ToArray();
            }
        }

        public float GetTime(DateTime date)
        {
            return (float)(DateTime.Now - date).TotalSeconds;
        }

        public int CreateItemId(ClassTypeEnum weaponClass, int usage, int number)
        {
            //GET_ITEM_FLAG_CLASS = (Itemflag)(Itemflag / 100000000)
            //GET_ITEM_FLAG_USAGE = (Itemflag)((Itemflag % 100000000) / 1000000)
            //GET_ITEM_FLAG_TYPE = (Itemflag)((Itemflag % 1000000) / 1000)
            //GET_ITEM_FLAG_NUMBER = (Itemflag)(Itemflag % 1000)
            //MAKE_ITEM_FLAG = (class,usage,classtype,number)	((class*100000000)+(usage*1000000)+(classtype*1000)+(number))
            return (GetItemClass(weaponClass) * 100000000) + (usage * 1000000) + ((int)weaponClass * 1000) + number;
        }

        public int GetItemClass(ClassTypeEnum classType)
        {
            switch (classType)
            {
                case ClassTypeEnum.Assault: return 1;
                case ClassTypeEnum.SMG: return 2;
                case ClassTypeEnum.DualSMG: return 2;
                case ClassTypeEnum.Sniper: return 3;
                case ClassTypeEnum.Shotgun: return 4;
                case ClassTypeEnum.DualShotgun: return 4;
                case ClassTypeEnum.MG: return 5;
                case ClassTypeEnum.HandGun: return 6;
                case ClassTypeEnum.DualHandGun: return 6;
                case ClassTypeEnum.CIC: return 6;
                case ClassTypeEnum.Knife: return 7;
                case ClassTypeEnum.DualKnife: return 7;
                case ClassTypeEnum.Knuckle: return 7;
                case ClassTypeEnum.Throwing: return 8;
                case ClassTypeEnum.Item: return 9;
                case ClassTypeEnum.Dino: return 0;
                default: return 1;
            }
        }

        public void Death(Room room, Slot killer, byte objectId, int weaponId, List<DeathServerData> deaths)
        {
            if (room != null && room.round.Timer == null && room.state == RoomStateEnum.Battle && killer.state == SlotStateEnum.BATTLE)
            {
                FragInfos info = new FragInfos
                {
                    killerIdx = (byte)killer.Id,
                    killingType = 0, //1 - piercing | 2 - mass
                    weaponId = weaponId,
                    x = killer.position.X,
                    y = killer.position.Y,
                    z = killer.position.Z,
                    flag = objectId
                };
                bool isSuicide = false;
                for (int i = 0; i < deaths.Count; i++)
                {
                    DeathServerData deathData = deaths[i];
                    byte deathInfo = (byte)(((int)deathData.DeathType * 16) + deathData.slot.Id);
                    int victimId = deathInfo & 15;
                    Slot victim = room.GetSlot(victimId);
                    if (victim != null && victim.state == SlotStateEnum.BATTLE /*&& victim.teamId != killer.teamId*/)
                    {
                        if (info.killerIdx == victimId)
                        {
                            isSuicide = true;
                        }
                        info.frags.Add(new Frag(deathInfo) { flag = 0, victimWeaponClass = (byte)deathData.slot.weaponClass, x = deathData.slot.position.X, y = deathData.slot.position.Y, z = deathData.slot.position.Z });
                    }
                }
                info.killsCount = (byte)info.frags.Count;
                bool isBotMode = room.IsBotMode();
                ushort score = 0;
                ItemClassEnum weaponClass = (ItemClassEnum)Utilities.GetIdStatics(info.weaponId, 1);
                for (int i = 0; i < info.frags.Count; i++)
                {
                    Frag frag = info.frags[i];
                    CharaDeathEnum deathType = (CharaDeathEnum)(frag.hitspotInfo >> 4);
                    if (info.killsCount - (isSuicide ? 1 : 0) > 1)
                    {
                        frag.killFlag |= deathType == CharaDeathEnum.BOOM || deathType == CharaDeathEnum.OBJECT_EXPLOSION || deathType == CharaDeathEnum.POISON || deathType == CharaDeathEnum.HOWL || deathType == CharaDeathEnum.TRAMPLED || weaponClass == ItemClassEnum.SHOTGUN ? KillingMessageEnum.MassKill : KillingMessageEnum.PiercingShot;
                    }
                    else
                    {
                        int type = 0;
                        if (deathType == CharaDeathEnum.HEADSHOT)
                        {
                            type = 4;
                        }
                        else if (deathType == CharaDeathEnum.DEFAULT && weaponClass == ItemClassEnum.KNIFE)
                        {
                            type = 6;
                        }
                        if (type > 0)
                        {
                            int lastState = killer.lastKillState >> 12;
                            if (type == 4)
                            {
                                if (lastState != 4)
                                {
                                    killer.repeatLastState = false;
                                }
                                killer.lastKillState = type << 12 | (killer.killsOnLife + 1);
                                if (killer.repeatLastState)
                                {
                                    frag.killFlag |= (killer.lastKillState & 16383) <= 1 ? KillingMessageEnum.Headshot : KillingMessageEnum.ChainHeadshot;
                                }
                                else
                                {
                                    frag.killFlag |= KillingMessageEnum.Headshot;
                                    killer.repeatLastState = true;
                                }
                            }
                            else if (type == 6)
                            {
                                if (lastState != 6)
                                {
                                    killer.repeatLastState = false;
                                }
                                killer.lastKillState = type << 12 | (killer.killsOnLife + 1);
                                if (killer.repeatLastState && (killer.lastKillState & 16383) > 1)
                                {
                                    frag.killFlag |= KillingMessageEnum.ChainSlugger;
                                }
                                else
                                {
                                    killer.repeatLastState = true;
                                }
                            }
                        }
                        else
                        {
                            killer.lastKillState = 0;
                            killer.repeatLastState = false;
                        }
                    }
                    Slot victimSlot = room.slots[frag.VictimSlot];
                    if (victimSlot.killsOnLife > 3)
                    {
                        frag.killFlag |= KillingMessageEnum.ChainStopper;
                    }
                    if (!((info.weaponId == 19016 || info.weaponId == 19022) && info.killerIdx == frag.VictimSlot) || !victimSlot.specGM)
                    {
                        victimSlot.allDeaths++; //explosao acid
                    }
                    if (killer.teamId != victimSlot.teamId)
                    {
                        score += GetKillScore(frag.killFlag);
                        killer.allKills++;
                        if (killer.deathState == DeadEnum.isAlive)
                        {
                            killer.killsOnLife++;
                        }
                        if (victimSlot.teamId == 0)
                        {
                            room.redDeaths++;
                            room.blueKills++;
                        }
                        else
                        {
                            room.blueDeaths++;
                            room.redKills++;
                        }
                        if (room.mode == RoomTypeEnum.Dino)
                        {
                            if (killer.teamId == 0)
                            {
                                room.redDino += 4;
                            }
                            else
                            {
                                room.blueDino += 4;
                            }
                        }
                    }
                    victimSlot.lastKillState = 0;
                    victimSlot.killsOnLife = 0;
                    victimSlot.repeatLastState = false;
                    victimSlot.passSequence = 0;
                    victimSlot.deathState = DeadEnum.isDead;
                    if (!isBotMode)
                    {
                        room.MissionCompleteBase(room.GetPlayerBySlot(victimSlot), victimSlot, MissionTypeEnum.DEATH, 0);
                    }
                    if (deathType == CharaDeathEnum.HEADSHOT)
                    {
                        killer.headshots++;
                    }
                }
                if (isBotMode)
                {
                    killer.score += (ushort)(killer.killsOnLife + room.IngameAiLevel + score);
                    if (killer.score >= 65535)
                    {
                        killer.score = 65535;
                        Logger.Warning($" [BATTLE] [Death] Foi atingido o limite de pontuação no Modo Challenge. PlayerId: {killer.playerId}");
                    }
                    info.Score = killer.score;
                }
                else
                {
                    killer.score += score;
                    room.MissionCompleteBase(room.GetPlayerBySlot(killer), killer, info, MissionTypeEnum.NA, 0);
                    info.Score = score;
                }
                using (BATTLE_DEATH_PAK packet = new BATTLE_DEATH_PAK(room, info, killer, isBotMode))
                {
                    room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                }
                if ((room.mode == RoomTypeEnum.DeathMatch || room.mode == RoomTypeEnum.HeadHunter || room.mode == RoomTypeEnum.Chaos) && !isBotMode)
                {
                    room.BattleEndKills(isBotMode);
                }
                else if (!killer.specGM && (room.mode == RoomTypeEnum.Destruction || room.mode == RoomTypeEnum.Suppression))
                {
                    room.GetPlayingPlayers(true, out int allRed, out int allBlue, out int redDeaths, out int blueDeaths);
                    int winner = 0;
                    if (redDeaths == allRed && killer.teamId == 0 && isSuicide && !room.PlantedBombC4)
                    {
                        winner = 1;
                        room.blueRounds++;
                        room.BattleEndRound(winner, true);
                    }
                    else if (blueDeaths == allBlue && killer.teamId == 1)
                    {
                        room.redRounds++;
                        room.BattleEndRound(winner, true);
                    }
                    else if (redDeaths == allRed && killer.teamId == 1)
                    {
                        if (!room.PlantedBombC4)
                        {
                            winner = 1;
                            room.blueRounds++;
                        }
                        else if (isSuicide)
                        {
                            room.redRounds++;
                        }
                        room.BattleEndRound(winner, false);
                    }
                    else if (blueDeaths == allBlue && killer.teamId == 0)
                    {
                        if (!isSuicide || !room.PlantedBombC4)
                        {
                            room.redRounds++;
                        }
                        else
                        {
                            winner = 1;
                            room.blueRounds++;
                        }
                        room.BattleEndRound(winner, true);
                    }
                }
            }
        }

        public ushort GetKillScore(KillingMessageEnum killingMessage)
        {
            ushort score = 0;
            if (killingMessage == KillingMessageEnum.MassKill || killingMessage == KillingMessageEnum.PiercingShot)
            {
                score += 6;
            }
            else if (killingMessage == KillingMessageEnum.ChainStopper)
            {
                score += 8;
            }
            else if (killingMessage == KillingMessageEnum.Headshot)
            {
                score += 10;
            }
            else if (killingMessage == KillingMessageEnum.ChainHeadshot)
            {
                score += 14;
            }
            else if (killingMessage == KillingMessageEnum.ChainSlugger)
            {
                score += 6;
            }
            else if (killingMessage == KillingMessageEnum.ObjectDefense)
            {
                score += 7;
            }
            else if (killingMessage != KillingMessageEnum.Suicide)
            {
                score += 5;
            }
            return score;
        }

        public void Sabotage(Room room, Slot slot, ushort damage, int barNumber)
        {
            if (room.round.Timer == null && room.state == RoomStateEnum.Battle && !room.swapRound)
            {
                int times = 0;
                if (barNumber == 1)
                {
                    slot.damageBar1 += damage;
                    times += slot.damageBar1 / 600;
                }
                else if (barNumber == 2)
                {
                    slot.damageBar2 += damage;
                    times += slot.damageBar2 / 600;
                }
                slot.earnedXP = times;
                if (room.mode == RoomTypeEnum.Sabotage)
                {
                    using (BATTLE_MISSION_GENERATOR_INFO_PAK packet = new BATTLE_MISSION_GENERATOR_INFO_PAK(room))
                    {
                        room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                    }
                    if (room.Bar1 == 0)
                    {
                        room.swapRound = true;
                        room.blueRounds++;
                        room.BattleEndRound(1, RoundEndTypeEnum.Normal);
                    }
                    else if (room.Bar2 == 0)
                    {
                        room.swapRound = true;
                        room.redRounds++;
                        room.BattleEndRound(0, RoundEndTypeEnum.Normal);
                    }
                }
                else if (room.mode == RoomTypeEnum.Defense)
                {
                    using (BATTLE_MISSION_DEFENCE_INFO_PAK packet = new BATTLE_MISSION_DEFENCE_INFO_PAK(room))
                    {
                        room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                    }
                    if (room.Bar1 == 0 && room.Bar2 == 0)
                    {
                        room.swapRound = true;
                        room.redRounds++;
                        room.BattleEndRound(0, RoundEndTypeEnum.Normal);
                    }
                }
            }
        }

        public byte[] Decrypt(byte[] data, int shift)
        {
            try
            {
                int length = data.Length;
                byte last = data[length - 1];
                for (int i = length - 1; i > 0; i--)
                {
                    data[i] = (byte)(data[i - 1] << (8 - shift) | data[i] >> shift);
                }
                data[0] = (byte)(last << (8 - shift) | data[0] >> shift);
                return data;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return new byte[0];
        }

        public byte[] Encrypt(byte[] data, int shift)
        {
            try
            {
                int length = data.Length - 1;
                byte first = data[0];
                for (int i = 0; i < length; i++)
                {
                    data[i] = (byte)(data[i + 1] >> (8 - shift) | (data[i] << shift));
                }
                data[length] = (byte)(first >> (8 - shift) | (data[length] << shift));
                return data;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return new byte[0];
        }
    }
}