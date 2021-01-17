namespace PointBlank.Game
{
    public class BASE_USER_EXIT_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(2655);
        }
    }
}