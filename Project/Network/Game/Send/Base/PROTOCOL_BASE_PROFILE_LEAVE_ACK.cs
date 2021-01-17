namespace PointBlank.Game
{
    public class BASE_PROFILE_LEAVE_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(3865);
        }
    }
}