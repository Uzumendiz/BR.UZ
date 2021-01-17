using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_SENDPING_REQ : GamePacketReader
    {
        private byte[] PingSlots;
        private int ReadyPlayersCount;
        public override void ReadImplement()
        {
            PingSlots = ReadB(16);
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room != null && room.slots[player.slotId].state >= SlotStateEnum.BATTLE_READY)
                {
                    if (room.state == RoomStateEnum.Battle)
                    {
                        room.ping = PingSlots[room.leaderSlot];
                    }
                    using (BATTLE_SENDPING_PAK packet = new BATTLE_SENDPING_PAK(PingSlots))
                    {
                        List<Account> players = room.GetAllPlayers(SlotStateEnum.READY, 1);
                        if (players.Count == 0)
                        {
                            return;
                        }
                        byte[] data = packet.GetCompleteBytes("BATTLE_SENDPING_REQ");
                        for (int i = 0; i < players.Count; i++)
                        {
                            Account pR = players[i];
                            if (room.slots[pR.slotId].state >= SlotStateEnum.BATTLE_READY)
                            {
                                pR.SendCompletePacket(data);
                            }
                            else
                            {
                                ReadyPlayersCount++;
                            }
                        }
                    }
                    if (ReadyPlayersCount == 0)
                    {
                        room.SpawnReadyPlayers();
                    }
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}