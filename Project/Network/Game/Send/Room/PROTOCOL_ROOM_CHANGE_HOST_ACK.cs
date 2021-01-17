namespace PointBlank.Game
{
    public class ROOM_CHANGE_HOST_PAK : GamePacketWriter
    {
        private uint slotOrError;
        public ROOM_CHANGE_HOST_PAK(uint error)
        {
            slotOrError = error;
        }
        public ROOM_CHANGE_HOST_PAK(int slotId)
        {
            slotOrError = (uint)slotId;
        }
        public override void Write()
        {
            WriteH(3871);
            WriteD(slotOrError);
        }
    }
}