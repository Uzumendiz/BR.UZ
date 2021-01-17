using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_USER_EXIT_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }
        public override void RunImplement()
        {
            try
            {
                client.SendCompletePacket(PackageDataManager.BASE_EXIT_GAME_PAK);
                client.Close(1000);
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}