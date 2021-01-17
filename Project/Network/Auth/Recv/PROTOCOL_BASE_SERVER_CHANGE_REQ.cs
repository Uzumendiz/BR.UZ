using System;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_SERVER_CHANGE_REQ : AuthPacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                if ((DateTime.Now - client.SessionDate).TotalSeconds < 2)
                {
                    Logger.Attacks($" [Auth] (PROTOCOL_BASE_SERVER_CHANGE_REQ) Connection destroyed on suspicion of modified client. IPAddress: {client.GetIPAddress()}");
                    client.Close(0, true);
                    return;
                }
                client.SendCompletePacket(PackageDataManager.BASE_SERVER_CHANGE_GAME_PAK);
                client.Close(0, false);
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}