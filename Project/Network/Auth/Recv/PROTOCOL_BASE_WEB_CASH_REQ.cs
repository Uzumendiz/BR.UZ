using System;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_WEB_CASH_REQ : AuthPacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.checkUserWebCash)
                {
                    return;
                }
                player.checkUserWebCash = true;
                client.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}