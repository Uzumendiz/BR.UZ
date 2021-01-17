using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_SERVER_PASSWORD_REQ : GamePacketReader
    {
        private string password;
        public override void ReadImplement()
        {
            password = ReadString(ReadByte());
        }

        public override void RunImplement()
        {
            try
            {
                if (password != Settings.ServerPassword)
                {
                    client.SendCompletePacket(PackageDataManager.BASE_SERVER_PASSW_ERROR_PAK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BASE_SERVER_PASSW_SUCCESS_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}