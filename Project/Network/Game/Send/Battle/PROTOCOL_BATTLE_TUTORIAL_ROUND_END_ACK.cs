namespace PointBlank.Game
{
    public class BATTLE_TUTORIAL_ROUND_END_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(3395);
            WriteC(3);
            WriteD(110);
        }
    }
}