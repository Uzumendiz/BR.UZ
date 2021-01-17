using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CHANGE_TEAM_REQ : GamePacketReader
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
                if (room != null && room.leaderSlot == player.slotId && room.state == RoomStateEnum.Ready && (DateTime.Now - room.lastChangeTeam).TotalSeconds >= 1.5 && !room.changingSlots)
                {
                    List<SlotChange> changeList = new List<SlotChange>();
                    lock (room.slots)
                    {
                        room.changingSlots = true;
                        foreach (int slotIdx in room.RED_TEAM)
                        {
                            int NewId = slotIdx + 1;
                            if (slotIdx == room.leaderSlot)
                            {
                                room.leaderSlot = NewId;
                            }
                            else if (NewId == room.leaderSlot)
                            {
                                room.leaderSlot = slotIdx;
                            }
                            room.SwitchSlots(changeList, NewId, slotIdx, true);
                        }
                        if (changeList.Count > 0)
                        {
                            using (PROTOCOL_ROOM_CHANGE_SLOTS_ACK packet = new PROTOCOL_ROOM_CHANGE_SLOTS_ACK(changeList, room.leaderSlot, 2))
                            {
                                byte[] data = packet.GetCompleteBytes("ROOM_CHANGE_TEAM_REQ");
                                List<Account> list = room.GetAllPlayers();
                                for (int i = 0; i < list.Count; i++)
                                {
                                    Account account = list[i];
                                    account.slotId = room.GetNewSlotId(account.slotId);
                                    account.SendCompletePacket(data);
                                }
                            }
                        }
                        room.changingSlots = false;
                    }
                    room.lastChangeTeam = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}