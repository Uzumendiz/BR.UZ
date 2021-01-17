using System;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_SOURCE_REQ : AuthPacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player != null && !player.checkSourceInfo)
                {
                    player.checkSourceInfo = true;
                    client.SendCompletePacket(PackageDataManager.A_2678_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}