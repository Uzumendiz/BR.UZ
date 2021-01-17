namespace PointBlank.Game
{
    public class CLAN_PRIVILEGES_MASTER_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(1339);
        }
    }
}