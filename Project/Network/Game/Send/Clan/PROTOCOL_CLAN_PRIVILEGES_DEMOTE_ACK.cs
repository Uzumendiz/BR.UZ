namespace PointBlank.Game
{
    public class CLAN_PRIVILEGES_DEMOTE_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(1345);
        }
    }
}