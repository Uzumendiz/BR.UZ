namespace PointBlank.Game
{
    public class BATTLE_4VS4_ERROR_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(3879);
        }
    }
}