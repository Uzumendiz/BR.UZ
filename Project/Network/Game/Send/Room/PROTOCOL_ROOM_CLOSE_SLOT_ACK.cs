namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CLOSE_SLOT_ACK : GamePacketWriter
    {
        private uint error;
        public PROTOCOL_ROOM_CLOSE_SLOT_ACK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(3850);
            WriteD(error);
        }
    }
}