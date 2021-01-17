namespace PointBlank.Game
{
    public class BASE_2612_PAK : GamePacketWriter
    {
        private Account player;
        private Clan clan;
        public BASE_2612_PAK(Account player)
        {
            this.player = player;
            clan = ClanManager.GetClan(player.clanId);
        }

        public override void Write()
        {
            WriteH(2612);
            WriteS(player.nickname, 33);
            WriteD(player.exp);
            WriteD(player.rankId);
            WriteD(player.rankId);
            WriteD(player.gold);
            WriteD(player.cash);
            WriteD(clan.id);
            WriteD((int)player.clanAuthority);
            WriteQ(0);
            WriteC(player.pccafe);
            WriteC(player.tourneyLevel);
            WriteC(player.nickcolor);
            WriteS(clan.name, 17);
            WriteC(clan.rank);
            WriteC(clan.GetClanUnit());
            WriteD(clan.logo);
            WriteC(clan.nameColor);
            WriteD(10000);
            WriteC(0);
            WriteD(0);
            WriteD(player.lastRankUpDate); //109 BYTES
        }
    }
}