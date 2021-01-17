using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_GET_CONTEXT_ENLISTMENTS_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.clanId <= 0)
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_GET_CONTEXT_ENLISTMENTS_ERROR_ACK);
                    return;
                }
                int RequestCount = player.GetInvitesCount(player.clanId);
                client.SendPacket(new PROTOCOL_CLAN_CONTEXT_ENLISTMENTS_ACK((uint)RequestCount));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}