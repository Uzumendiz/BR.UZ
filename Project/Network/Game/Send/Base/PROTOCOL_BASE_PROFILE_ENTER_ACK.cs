namespace PointBlank.Game
{
    public class BASE_PROFILE_ENTER_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(3863);
        }
    }
}