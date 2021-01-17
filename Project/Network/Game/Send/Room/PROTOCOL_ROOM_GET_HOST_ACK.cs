namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_GET_HOST_ACK : GamePacketWriter
    {
        private uint slotOrError;
        public PROTOCOL_ROOM_GET_HOST_ACK(uint error)
        {
            slotOrError = error;
        }
        public PROTOCOL_ROOM_GET_HOST_ACK(int slot)
        {
            slotOrError = (uint)slot;
        }
        public override void Write()
        {
            WriteH(3867);
            WriteD(slotOrError);
        }
    }
}