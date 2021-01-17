using System;
using System.Threading;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_PROFILE_LEAVE_REQ : GamePacketReader
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
                if (player == null || (now - player.lastProfileLeave).TotalSeconds < 1)
                {
                    return;
                }
                Room room = player.room;
                if (room != null)
                {
                    room.ChangeSlotState(player.slotId, SlotStateEnum.NORMAL, true);
                }
                client.SendCompletePacket(PackageDataManager.BASE_PROFILE_LEAVE_PAK);
                player.lastProfileLeave = now;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}