namespace PointBlank.Api
{
    public class API_LOGIN_ADMIN_RESULT_ACK : ApiPacketWriter
    {
        private Account account;
        private byte result;
        public API_LOGIN_ADMIN_RESULT_ACK(Account account, byte result)
        {
            this.account = account;
            this.result = result;
        }
        public override void Write()
        {
            WriteH(9);
            WriteC(result);
            if (result == 1) //1= Success
            {
                WriteC((byte)account.nickname.Length);
                WriteS(account.nickname, account.nickname.Length);
                WriteC((byte)account.access);
                WriteC(account.pccafe);
                WriteC(account.rankId);
                WriteD(account.cash);
                WriteD(account.gold);
                WriteC(account.isOnline);
                WriteD(account.channelId);
            }
        }
    }
}