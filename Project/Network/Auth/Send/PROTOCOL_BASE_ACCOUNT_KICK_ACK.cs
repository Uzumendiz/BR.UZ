namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_ACCOUNT_KICK_ACK : GamePacketWriter
    {
        private byte type;
        public PROTOCOL_BASE_ACCOUNT_KICK_ACK(byte type)
        {
            this.type = type;
        }

        public override void Write()
        {
            WriteH(513);
            WriteC(type);
        }
    }
}