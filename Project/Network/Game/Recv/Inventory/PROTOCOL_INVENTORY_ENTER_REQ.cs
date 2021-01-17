using System;

namespace PointBlank.Game
{
    public class PROTOCOL_INVENTORY_ENTER_REQ : GamePacketReader
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
                if (player == null || (now - player.lastInventoryEnter).TotalSeconds < 1)
                {
                    return;
                }
                Room room = player.room;
                if (room != null)
                {
                    room.ChangeSlotState(player.slotId, SlotStateEnum.INVENTORY, false);
                    room.StopCountDown(player.slotId);
                    room.UpdateSlotsInfo();
                }
                player.lastInventoryEnter = now;
                client.SendPacket(new INVENTORY_ENTER_PAK(int.Parse(player.lastInventoryEnter.ToString("yyMMddHHmm"))));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}