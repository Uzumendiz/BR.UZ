using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_PROFILE_ENTER_REQ : GamePacketReader
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
                if (player == null || (now - player.lastProfileEnter).TotalSeconds < 1)
                {
                    return;
                }
                Room room = player.room;
                if (room != null)
                {
                    room.ChangeSlotState(player.slotId, SlotStateEnum.INFO, false);
                    room.StopCountDown(player.slotId);
                    room.UpdateSlotsInfo();
                }
                client.SendCompletePacket(PackageDataManager.BASE_PROFILE_ENTER_PAK);
                player.lastProfileEnter = now;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}