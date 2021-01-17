using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_CHANNEL_LIST_REQ : GamePacketReader
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
                if (player != null && (now - player.lastChannelList).TotalSeconds >= 1)
                {
                    client.SendPacket(new BASE_CHANNEL_LIST_PAK());
                    player.lastChannelList = now;
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}