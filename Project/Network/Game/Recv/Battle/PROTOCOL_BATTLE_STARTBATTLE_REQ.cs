using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_STARTBATTLE_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room == null)
                {
                    Logger.Warning($" [Game] [{GetType().Name}] Room is null.");
                    client.Close(0, false);
                    return;
                }
                if (!room.IsPreparing())
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_0x8000100B_ACK);
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_BATTLE_STARTBATTLE_ACK);
                    room.ChangeSlotState(player.slotId, SlotStateEnum.NORMAL, true);
                    return;
                }
                bool isBotMode = room.IsBotMode();
                Slot slot = room.GetSlot(player.slotId);
                if (slot == null || slot.state != SlotStateEnum.PRESTART)
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_0x8000100B_ACK);
                    client.SendPacket(new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(player, 0));
                    room.ChangeSlotState(slot, SlotStateEnum.NORMAL, true);
                    room.BattleEndPlayersCount(isBotMode);
                    return;
                }
                room.ChangeSlotState(slot, SlotStateEnum.BATTLE_READY, true);
                slot.StopTiming();
                if (isBotMode)
                {
                    client.SendPacket(new PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_ACK(room));
                }
                client.SendPacket(new PROTOCOL_ROOM_CHANGE_INFO_ACK(room));
                int blueBATTLE_READY = 0;
                int redBATTLE_READY = 0;
                int redLOAD = 0;
                int blueLOAD = 0;
                int totalREADY = 0;
                lock (room.slots)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        Slot slotR = room.slots[i];
                        if (slotR.state >= SlotStateEnum.READY)
                        {
                            totalREADY++;
                            if (slotR.state >= SlotStateEnum.LOAD)
                            {
                                if (slotR.teamId == 0)
                                {
                                    redLOAD++;
                                }
                                else
                                {
                                    blueLOAD++;
                                }
                                if (slotR.state >= SlotStateEnum.BATTLE_READY)
                                {
                                    if (slotR.teamId == 0)
                                    {
                                        redBATTLE_READY++;
                                    }
                                    else
                                    {
                                        blueBATTLE_READY++;
                                    }
                                }
                            }
                        }
                    }
                }
                if (room.GetChannel(out Channel channel) && channel.type == 4 && totalREADY == (redBATTLE_READY + blueBATTLE_READY))
                {
                    Logger.Warning($" [PROTOCOL_BATTLE_STARTBATTLE_REQ] Iniciando partida de ClanFronto com {totalREADY} jogadores. TR: {redBATTLE_READY} CT: {blueBATTLE_READY}");
                    room.SpawnReadyPlayers(isBotMode);
                }
                else if (room.state == RoomStateEnum.Battle || (room.slots[room.leaderSlot].state >= SlotStateEnum.BATTLE_READY && isBotMode && ((room.leaderSlot % 2 == 0 && redBATTLE_READY > redLOAD / 2) || (room.leaderSlot % 2 == 1 && blueBATTLE_READY > blueLOAD / 2))) || (room.slots[room.leaderSlot].state >= SlotStateEnum.BATTLE_READY && blueBATTLE_READY > blueLOAD / 2 && redBATTLE_READY > redLOAD / 2))
                {
                    room.SpawnReadyPlayers(isBotMode);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}