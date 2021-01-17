using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_GET_ROOMLIST_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                DateTime now = DateTime.Now;
                if (player == null || (now - player.lastRoomList).TotalSeconds < 1)
                {
                    return;
                }
                Channel channel = player.GetChannel();
                if (channel != null)
                {
                    player.lastRoomList = now;
                    client.PacketGetRoomList = true;
                    channel.RemoveEmptyRooms();
                    List<Room> rooms = channel.rooms;
                    List<Account> waiting = channel.GetWaitPlayers();
                    int Rpages = (int)Math.Ceiling(rooms.Count / 15d);
                    int Apages = (int)Math.Ceiling(waiting.Count / 10d);
                    if (player.lastRoomPage >= Rpages)
                    {
                        player.lastRoomPage = 0;
                    }
                    if (player.lastPlayerPage >= Apages)
                    {
                        player.lastPlayerPage = 0;
                    }
                    int roomsCount = 0, playersCount = 0;
                    byte[] roomsArray = GetRoomListData(player, ref roomsCount, rooms);
                    byte[] waitingArray = GetPlayerListData(player.lastPlayerPage, ref playersCount, waiting);
                    client.SendPacket(new LOBBY_GET_ROOMLIST_PAK(rooms.Count, waiting.Count, player.lastRoomPage++, player.lastPlayerPage++, roomsCount, playersCount, roomsArray, waitingArray));
                    rooms = null;
                    waiting = null;
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
        private byte[] GetRoomListData(Account player, ref int count, List<Room> list)
        {
            using (PacketWriter send = new PacketWriter())
            {
                for (int i = player.lastRoomPage * 15; i < list.Count; i++)
                {         
                    Room room = list[i];
                    if (MapsXML.CheckId(room.mapId))
                    {
                        byte restrictions = 0;
                        send.WriteD(room.roomId);
                        if (room.KickedPlayersVote.Contains(player.playerId))
                        {
                            send.WriteS("[Batalha inacessível]", 23);
                        }
                        else
                        {
                            send.WriteS(room.roomName, 23);
                        }
                        send.WriteH((ushort)room.mapId);
                        send.WriteC(room.stage4vs4);
                        send.WriteC((byte)room.mode);
                        send.WriteC((byte)room.state);
                        send.WriteC(room.GetAllPlayersCount());
                        send.WriteC(room.GetSlotCount());
                        send.WriteC(room.ping);
                        send.WriteC(room.weaponsFlag);
                        if (room.randomMap > 0)
                        {
                            restrictions += 2;
                        }
                        if (room.password.Length > 0)
                        {
                            restrictions += 4;
                        }
                        if (room.limit > 0 && room.state > RoomStateEnum.Ready)
                        {
                            restrictions += 128;
                        }
                        send.WriteC(restrictions);
                        send.WriteC((byte)room.modeSpecial);
                    }
                    if (count++ == 15)
                    {
                        break;
                    }
                }
                return send.memorystream.ToArray();
            }
        }
        private byte[] GetPlayerListData(int page, ref int count, List<Account> list)
        {
            using (PacketWriter send = new PacketWriter())
            {
                for (int i = page * 10; i < list.Count; i++)
                {
                    Account player = list[i];
                    if (player.nickname.Length >= Settings.NickMinLength && player.nickname.Length < Settings.NickMaxLength)
                    {
                        Clan clan = ClanManager.GetClan(player.clanId);
                        send.WriteD(player.GetSessionId());
                        send.WriteD(clan.logo);
                        send.WriteS(clan.name, 17);
                        send.WriteH((short)player.GetRank());
                        send.WriteS(player.nickname, 33);
                        send.WriteC(player.nickcolor);
                        send.WriteC(player.country);
                    }
                    else
                    {
                        Logger.Warning($" [GAME] [{GetType().Name}] Não foi possivel exibir o jogador no lobby devido seu nickname: {player.nickname} PlayerId: {player.playerId}");
                    }
                    if (count++ == 10)
                    {
                        break;
                    }
                }
                return send.memorystream.ToArray();
            }
        }
    }
}