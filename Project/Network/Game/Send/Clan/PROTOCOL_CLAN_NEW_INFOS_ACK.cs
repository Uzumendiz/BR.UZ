namespace PointBlank.Game
{
    public class CLAN_NEW_INFOS_PAK : GamePacketWriter
    {
        private Clan clan;
        private Account p;
        private int players;
        public CLAN_NEW_INFOS_PAK(Clan c, Account owner, int clanPlayers)
        {
            clan = c;
            p = owner;
            players = clanPlayers;
        }
        public CLAN_NEW_INFOS_PAK(Clan c, int clanPlayers)
        {
            clan = c;
            p = AccountManager.GetAccount(clan.ownerId, 0);
            players = clanPlayers;
        }
        public override void Write()
        {
            WriteH(1328);
            WriteD(clan.id);
            WriteS(clan.name, 17);
            WriteC(clan.rank);
            WriteC((byte)players);
            WriteC((byte)clan.maxPlayers);
            WriteD(clan.creationDate);
            WriteD(clan.logo);
            WriteC(clan.nameColor);
            WriteC(clan.GetClanUnit(players));
            WriteD(clan.exp);
            WriteD(0);
            WriteQ(clan.ownerId);
            WriteS(p != null ? p.nickname : "", 33);
            WriteC((byte)(p != null ? p.rankId : 0));
            WriteS(clan.informations, 255);
            WriteS("Temp", 21);
            WriteC((byte)clan.limitRankId);
            WriteC((byte)clan.limitAgeBigger);
            WriteC((byte)clan.limitAgeSmaller);
            WriteC((byte)clan.authorityConfig);
            WriteS(clan.notice, 255);
            WriteD(clan.partidas);
            WriteD(clan.vitorias);
            WriteD(clan.derrotas);
            WriteD(clan.partidas);
            WriteD(clan.vitorias);
            WriteD(clan.derrotas);
            //MELHORES MEMBROS DO CLÃ
            WriteQ(clan.BestPlayers.Exp.PlayerId); //XP Adquirida (Total)
            WriteQ(clan.BestPlayers.Exp.PlayerId); //XP Adquirida (Temporada)
            WriteQ(clan.BestPlayers.Wins.PlayerId); //Vitória (Total)
            WriteQ(clan.BestPlayers.Wins.PlayerId); //Vitória (Temporada)
            WriteQ(clan.BestPlayers.Kills.PlayerId); //Kills (Total)
            WriteQ(clan.BestPlayers.Kills.PlayerId); //Kills (Temporada)
            WriteQ(clan.BestPlayers.Headshot.PlayerId); //Headshots (Total)
            WriteQ(clan.BestPlayers.Headshot.PlayerId); //Headshots (Temporada)
            WriteQ(clan.BestPlayers.Participation.PlayerId); //Participação (Total)
            WriteQ(clan.BestPlayers.Participation.PlayerId); //Participação (Temporada)
            WriteT(clan.pontos);
        }
    }
}