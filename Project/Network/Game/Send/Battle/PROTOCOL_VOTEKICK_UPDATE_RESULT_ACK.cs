namespace PointBlank.Game
{
    public class VOTEKICK_UPDATE_RESULT_PAK : GamePacketWriter
    {
        private uint error;
        public VOTEKICK_UPDATE_RESULT_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(3401);
            WriteD(error);
        }
    }
}