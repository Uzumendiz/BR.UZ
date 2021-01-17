using System.Net;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_SERVER_LIST_ACK : GamePacketWriter
    {
        private int SessionId;
        private ushort SessionSeed;
        private IPAddress Ip;
        private ushort Port;
        public PROTOCOL_BASE_SERVER_LIST_ACK(int SessionId, ushort SessionSeed, IPAddress Ip, int Port)
        {
            this.SessionId = SessionId;
            this.SessionSeed = SessionSeed;
            this.Ip = Ip;
            this.Port = (ushort)Port;
        }

        public override void Write()
        {
            WriteH(2049);
            WriteD(SessionId);
            WriteIP(Ip);
            WriteH(Port); //29890
            WriteH(SessionSeed);
            WriteB(ServersManager.GameServerListBytes);
            WriteD(ServersManager.GetPlayers());
        }
    }
}