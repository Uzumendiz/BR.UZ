namespace PointBlank.Game
{
    public class BASE_TITLE_DETACH_PAK : GamePacketWriter
    {
        private uint error;
        public BASE_TITLE_DETACH_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(2624);
            WriteD(error);
        }
    }
}