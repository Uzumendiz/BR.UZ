using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_WEB_CASH_REQ : GamePacketReader
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
                client.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}