namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_INVITE_RETURN_ACK : GamePacketWriter
    {
        private uint error;
        public PROTOCOL_ROOM_INVITE_RETURN_ACK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(3885);
            WriteD(error);
        }
    }
}