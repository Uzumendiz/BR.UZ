namespace PointBlank.Game
{
    public class BASE_TITLE_GET_PAK : GamePacketWriter
    {
        private int slotsCount;
        private uint error;
        public BASE_TITLE_GET_PAK(uint error, int slotsCount)
        {
            this.error = error;
            this.slotsCount = slotsCount;
        }

        public override void Write()
        {
            WriteH(2620);
            WriteD(error);
            WriteD(slotsCount);
        }
    }
}