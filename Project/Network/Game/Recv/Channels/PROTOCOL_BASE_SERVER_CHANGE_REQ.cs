using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_SERVER_CHANGE_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                if (!client.ConnectionIsClosed)
                {
                    client.SendCompletePacket(PackageDataManager.BASE_SERVER_CHANGE_GAME_PAK);
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}