namespace PointBlank.Api
{
    public class API_AUTH_ACK : ApiPacketWriter
    {
        public override void Write()
        {
            WriteH(1);
            WriteD(AuthManager.SocketSessions.Count);
            WriteD(GameManager.SocketSessions.Count);
            WriteD(AccountManager.accounts.Count);
            WriteD(ClanManager.clans.Count);
            WriteD(Application.recordOnline);
            WriteQ(long.Parse(Application.StartDate.ToString("yyyyMMddHHmmss")));
        }
    }
}