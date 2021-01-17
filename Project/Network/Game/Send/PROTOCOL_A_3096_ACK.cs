namespace PointBlank.Game
{
    public class A_3096_PAK : GamePacketWriter
    {
        private int XPEarned, GPEarned;
        public A_3096_PAK(int xp_earned, int gp_earned)
        {
            XPEarned = xp_earned;
            GPEarned = gp_earned;
        }

        public override void Write()
        {
            WriteH(3097);
            WriteD(XPEarned);
            WriteD(GPEarned);
        }
    }
}