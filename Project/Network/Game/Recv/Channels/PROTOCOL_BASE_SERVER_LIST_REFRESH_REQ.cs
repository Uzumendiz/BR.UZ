using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_SERVER_LIST_REFRESH_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                DateTime now = DateTime.Now;
                if ((now - client.LastServerListRefresh).Seconds >= 1)
                {
                    client.SendPacket(new PROTOCOL_BASE_SERVER_LIST_REFRESH_ACK());
                    client.LastServerListRefresh = now;
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}