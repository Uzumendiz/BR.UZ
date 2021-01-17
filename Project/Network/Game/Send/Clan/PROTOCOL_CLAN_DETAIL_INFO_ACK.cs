namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_DETAIL_INFO_ACK : GamePacketWriter
    {
        private Clan clan;
        private Account owner;
        private int error;
        public PROTOCOL_CLAN_DETAIL_INFO_ACK(Clan clan, Account owner, int error)
        {
            this.clan = clan;
            this.owner = owner;
            this.error = error;
        }

        public override void Write()
        {
            WriteH(1305);
            WriteD(error);
            WriteD(clan.id);
            WriteS(clan.name, 17);
            WriteC(clan.rank);
            WriteC(clan.GetClanPlayers());
            WriteC((byte)clan.maxPlayers);
            WriteD(clan.creationDate);
            WriteD(clan.logo);
            WriteC(clan.nameColor);
            WriteC(clan.GetClanUnit());
            WriteD(clan.exp);
            WriteD(10); //?
            WriteQ(clan.ownerId);
            if (owner != null)
            {
                WriteS(owner.nickname, 33);
                WriteC(owner.rankId);
            }
            else
            {
                WriteS("Indefinido", 33);
                WriteC(0); //RankId
            }
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