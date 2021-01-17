namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_LIST_ACK : GamePacketWriter
    {
        private int page;
        private byte count;
        private byte[] array;
        public PROTOCOL_CLAN_LIST_ACK(int page, byte count, byte[] array)
        {
            this.page = page;
            this.count = count;
            this.array = array;
        }
        public override void Write()
        {
            WriteH(1446);
            WriteD(page);
            WriteC(count);
            WriteB(array);
        }
    }
}