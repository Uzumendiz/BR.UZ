namespace PointBlank.Game
{
    public class VOTEKICK_CHECK_PAK : GamePacketWriter
    {
        private uint error;
        public VOTEKICK_CHECK_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(3397);
            WriteD(error);
        }
    }
}