namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_LOGIN_ACK : GamePacketWriter
    {
        private uint result;
        private string login;
        private long playerId;
        public PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum result, string login, long playerId)
        {
            this.result = (uint)result;
            this.login = login;
            this.playerId = playerId;
        }
        public PROTOCOL_BASE_LOGIN_ACK(int result, string login, long playerId)
        {
            this.result = (uint)result;
            this.login = login;
            this.playerId = playerId;
        }
        public override void Write()
        {
            WriteH(2564);
            WriteD(result);
            WriteC(0);
            WriteQ(playerId);
            WriteC((byte)login.Length);
            WriteS(login, login.Length);
            WriteC(0); //(Max = 127/128)
            WriteC(0); //(Max = 49/50)
        }
    }
}