namespace PointBlank.Game
{
    public class CLAN_CREATE_REQUIREMENTS_PAK : GamePacketWriter
    {

        public override void Write()
        {
            WriteH(1417);
            WriteC((byte)Settings.ClanCreateRank);
            WriteD(Settings.ClanCreateGold);
        }
    }
}