namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_CREATE_NICKNAME_ACK : GamePacketWriter
    {
        private uint error;
        public PROTOCOL_LOBBY_CREATE_NICKNAME_ACK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(3102);
            WriteD(error);
        }
    }
}