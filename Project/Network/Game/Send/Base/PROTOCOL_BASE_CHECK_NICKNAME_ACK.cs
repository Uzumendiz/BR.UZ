namespace PointBlank.Game
{
    public class AUTH_CHECK_NICKNAME_PAK : GamePacketWriter
    {
        private uint error;
        public AUTH_CHECK_NICKNAME_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(549);
            WriteD(error);
        }
    }
}