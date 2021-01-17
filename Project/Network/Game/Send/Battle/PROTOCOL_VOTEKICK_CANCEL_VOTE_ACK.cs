namespace PointBlank.Game
{
    public class VOTEKICK_CANCEL_VOTE_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(3405);
        }
    }
}