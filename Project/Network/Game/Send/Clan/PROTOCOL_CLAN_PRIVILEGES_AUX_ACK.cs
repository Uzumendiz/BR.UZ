namespace PointBlank.Game
{
    public class CLAN_PRIVILEGES_AUX_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(1342);
        }
    }
}