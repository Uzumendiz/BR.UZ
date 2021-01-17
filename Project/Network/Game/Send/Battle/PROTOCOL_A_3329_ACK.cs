namespace PointBlank.Game
{
    public class A_3329_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(3330);
            WriteD(0);
        }
    }
}