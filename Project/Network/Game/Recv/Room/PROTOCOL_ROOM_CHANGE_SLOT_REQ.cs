using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CHANGE_SLOT_REQ : GamePacketReader
    {
        private int teamIdx;
        public override void ReadImplement()
        {
            teamIdx = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                DateTime now = DateTime.Now;
                if (teamIdx >= 0 && teamIdx < 2 && room != null && (now - player.lastSlotChange).TotalSeconds >= 0.7 && !room.changingSlots)
                {
                    Slot slot = room.GetSlot(player.slotId);
                    if (slot != null && teamIdx != slot.teamId && slot.state == SlotStateEnum.NORMAL)
                    {
                        lock (room.slots)
                        {
                            room.changingSlots = true;
                            List<SlotChange> changeList = new List<SlotChange>();
                            room.SwitchNewSlot(changeList, player, slot, teamIdx);
                            if (changeList.Count > 0)
                            {
                                using (PROTOCOL_ROOM_CHANGE_SLOTS_ACK packet = new PROTOCOL_ROOM_CHANGE_SLOTS_ACK(changeList, room.leaderSlot, 0))
                                {
                                    room.SendPacketToPlayers(packet);
                                }
                            }
                            room.changingSlots = false;
                        }
                        player.lastSlotChange = now;
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