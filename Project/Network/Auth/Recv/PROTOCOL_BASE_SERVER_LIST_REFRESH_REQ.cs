using System;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_SERVER_LIST_REFRESH_REQ : AuthPacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                if ((DateTime.Now - client.LastServerListRefresh).Seconds >= 1)
                {
                    client.SendPacket(new PROTOCOL_BASE_SERVER_LIST_REFRESH_ACK());
                    client.LastServerListRefresh = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}