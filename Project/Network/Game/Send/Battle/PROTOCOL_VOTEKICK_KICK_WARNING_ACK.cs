namespace PointBlank.Game
{
    public class VOTEKICK_KICK_WARNING_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(3409);
        }
    }
}