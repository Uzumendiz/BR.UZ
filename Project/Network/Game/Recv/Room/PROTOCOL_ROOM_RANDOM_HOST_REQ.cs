using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_RANDOM_HOST_REQ : GamePacketReader
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
                if (room != null && room.leaderSlot == player.slotId && room.state == RoomStateEnum.Ready)
                {
                    List<Slot> slots = new List<Slot>();
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
                        int idx = new Random().Next(slots.Count);
                        Slot result = slots[idx];
                        if (room.GetPlayerBySlot(result) != null)
                        {
                            client.SendPacket(new PROTOCOL_ROOM_NEW_HOST_ACK((uint)result.Id));
                            room.UpdateRoomInfo();
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.ROOM_NEW_HOST_ERROR_PAK);
                        }
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.ROOM_NEW_HOST_ERROR_PAK);
                    }
                    slots = null;
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.ROOM_NEW_HOST_ERROR_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}