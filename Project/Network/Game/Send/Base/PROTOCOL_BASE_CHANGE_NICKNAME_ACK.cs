namespace PointBlank.Game
{
    public class PROTOCOL_AUTH_CHANGE_NICKNAME_ACK : GamePacketWriter
    {
        private string nickname;
        public PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(string nickname)
        {
            this.nickname = nickname;
        }

        public override void Write()
        {
            WriteH(300);
            WriteC((byte)nickname.Length);
            WriteS(nickname, nickname.Length);
        }
    }
}