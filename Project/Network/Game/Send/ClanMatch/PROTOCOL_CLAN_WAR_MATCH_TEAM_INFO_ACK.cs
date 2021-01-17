namespace PointBlank.Game
{
    public class CLAN_WAR_MATCH_TEAM_INFO_PAK : GamePacketWriter
    {
        private uint _erro;
        private Clan c;
        private Account leader;
        public CLAN_WAR_MATCH_TEAM_INFO_PAK(uint erro, Clan c)
        {
            _erro = erro;
            this.c = c;
            if (this.c != null)
            {
                leader = AccountManager.GetAccount(this.c.ownerId, 0);
                if (leader == null) _erro = 0x80000000;
            }
        }
        public CLAN_WAR_MATCH_TEAM_INFO_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1570);
            WriteD(_erro);
            if (_erro == 0)
            {
                byte players = c.GetClanPlayers();
                WriteD(c.id);
                WriteS(c.name, 17);
                WriteC(c.rank);
                WriteC(players);
                WriteC((byte)c.maxPlayers);
                WriteD(c.creationDate);
                WriteD(c.logo);
                WriteC(c.nameColor);
                WriteC(c.GetClanUnit(players));
                WriteD(c.exp);
                WriteD(0);
                WriteQ(c.ownerId);
                WriteS(leader.nickname, 33);
                WriteC(leader.rankId);
                WriteS("", 255);
            }//727 bytes
        }
    }
}