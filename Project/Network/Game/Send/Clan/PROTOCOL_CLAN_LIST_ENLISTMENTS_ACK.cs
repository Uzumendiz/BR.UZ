namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_LIST_ENLISTMENTS_ACK : GamePacketWriter
    {
        private int error, page, count;
        private byte[] array;
        public PROTOCOL_CLAN_LIST_ENLISTMENTS_ACK(int error, int count, int page, byte[] array)
        {
            this.error = error;
            this.count = count;
            this.page = page;
            this.array = array;
        }
        public PROTOCOL_CLAN_LIST_ENLISTMENTS_ACK(int error)
        {
            this.error = error;
        }
        public override void Write()
        {
            WriteH(1323);
            WriteD(error);
            if (error >= 0)
            {
                WriteC((byte)page);
                WriteC((byte)count);
                WriteB(array);
            }
        }
    }
}