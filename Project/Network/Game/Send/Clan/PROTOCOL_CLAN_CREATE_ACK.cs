namespace PointBlank.Game
{
    public class CLAN_CREATE_PAK : GamePacketWriter
    {
        private Account _p;
        private Clan clan;
        private uint _erro;
        public CLAN_CREATE_PAK(uint erro, Clan clan, Account player)
        {
            _erro = erro;
            this.clan = clan;
            _p = player;
        }

        public override void Write()
        {
            WriteH(1311);
            WriteD(_erro);
            if (_erro == 0)
            {
                WriteD(clan.id);
                WriteS(clan.name, 17);
                WriteC(clan.rank);
                WriteC(clan.GetClanPlayers());
                WriteC((byte)clan.maxPlayers);
                WriteD(clan.creationDate);
                WriteD(clan.logo);
                WriteB(new byte[10]);
                WriteQ(clan.ownerId);
                WriteS(_p.nickname, 33);
                WriteC(_p.rankId);
                WriteS(clan.informations, 255);
                WriteS("Url", 21);
                WriteC((byte)clan.limitRankId);
                WriteC((byte)clan.limitAgeBigger);
                WriteC((byte)clan.limitAgeSmaller);
                WriteC((byte)clan.authorityConfig);
                WriteS("", 255);
                WriteB(new byte[104]);
                WriteT(clan.pontos);
                WriteD(_p.gold);
            }
        }
    }
}