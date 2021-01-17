namespace PointBlank.Game
{
    public class CLAN_CHANGE_POINTS_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(1410);
        }
    }
}