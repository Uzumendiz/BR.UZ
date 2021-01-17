using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_RANDOM_HOST2_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                List<Slot> slots = new List<Slot>();
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room != null && room.leaderSlot == player.slotId && room.state == RoomStateEnum.Ready)
                {
                    lock (room.slots)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            Slot slot = room.slots[i];
                            if (slot.playerId > 0 && i != room.leaderSlot)
                            {
                                slots.Add(slot);
                            }
                        }
                    }

                    if (slots.Count > 0)
                    {
                        Slot slot = slots[new Random().Next(slots.Count)];
                        Account playerRandom = room.GetPlayerBySlot(slot);
                        if (playerRandom != null)
                        {
                            room.SetNewLeader(slot.Id, 0, room.leaderSlot, false);
                            using (PROTOCOL_ROOM_RANDOM_HOST_ACK packet = new PROTOCOL_ROOM_RANDOM_HOST_ACK(slot.Id))
                            {
                                room.SendPacketToPlayers(packet);
                            }
                            room.UpdateSlotsInfo();
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.ROOM_RANDOM_HOST_ERROR_PAK);
                        }
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.ROOM_RANDOM_HOST_ERROR_PAK);
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