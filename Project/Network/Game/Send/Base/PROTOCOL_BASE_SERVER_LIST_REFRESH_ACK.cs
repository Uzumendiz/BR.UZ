namespace PointBlank.Game
{
    public class PROTOCOL_BASE_SERVER_LIST_REFRESH_ACK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(2643);
            WriteB(ServersManager.ServerRefreshBytes);
            WriteD(ServersManager.GetPlayers());
        }
    }
}