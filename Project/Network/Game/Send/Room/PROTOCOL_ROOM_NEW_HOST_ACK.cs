namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_NEW_HOST_ACK : GamePacketWriter
    {
        private uint slotId;
        public PROTOCOL_ROOM_NEW_HOST_ACK(uint slotId)
        {
            this.slotId = slotId;
        }

        public override void Write()
        {
            WriteH(3873);
            WriteD(slotId);
        }
    }
}