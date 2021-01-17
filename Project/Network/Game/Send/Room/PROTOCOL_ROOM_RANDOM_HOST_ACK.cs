namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_RANDOM_HOST_ACK : GamePacketWriter
    {
        private uint slotOrError;
        public PROTOCOL_ROOM_RANDOM_HOST_ACK(uint error)
        {
            slotOrError = error;
        }
        public PROTOCOL_ROOM_RANDOM_HOST_ACK(int slotId)
        {
            slotOrError = (uint)slotId;
        }
        public override void Write()
        {
            WriteH(3869);
            WriteD(slotOrError);
        }
    }
}