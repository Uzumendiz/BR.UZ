using System;

namespace PointBlank.Game
{
    /*
     * Sair da pagina de clã.
     */
    public class PROTOCOL_CLAN_LEAVE_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null)
                {
                    return;
                }
                Room room = player.room;
                if (room != null)
                {
                    room.ChangeSlotState(player.slotId, SlotStateEnum.NORMAL, true);
                }
                client.SendCompletePacket(PackageDataManager.CLAN_CLIENT_LEAVE_PAK);
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}