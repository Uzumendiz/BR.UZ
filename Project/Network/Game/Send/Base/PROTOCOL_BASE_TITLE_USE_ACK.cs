namespace PointBlank.Game
{
    public class BASE_TITLE_USE_PAK : GamePacketWriter
    {
        private uint error;
        public BASE_TITLE_USE_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(2622);
            WriteD(error);
        }
    }
}