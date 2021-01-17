namespace PointBlank.Game
{
    public class CLAN_WAR_MATCH_REQUEST_BATTLE_PAK : GamePacketWriter
    {
        public Match mt;
        public Account p;
        public CLAN_WAR_MATCH_REQUEST_BATTLE_PAK(Match match, Account p)
        {
            this.mt = match;
            this.p = p;
        }

        public override void Write()
        {
            WriteH(1555);
            WriteH((short)mt.matchId);
            WriteH((ushort)mt.GetServerInfo());
            WriteH((ushort)mt.GetServerInfo());
            WriteC((byte)mt.state);
            WriteC((byte)mt.friendId);
            WriteC((byte)mt.formação);
            WriteC((byte)mt.GetCountPlayers());
            WriteD(mt.leader);
            WriteC(0);
            WriteD(mt.clan.id);
            WriteC(mt.clan.rank);
            WriteD(mt.clan.logo);
            WriteS(mt.clan.name, 17);
            WriteT(mt.clan.pontos);
            WriteC(mt.clan.nameColor);
            if (p != null)
            {
                WriteC((byte)p.rankId);
                WriteS(p.nickname, 33);
                WriteQ(p.playerId);
                WriteC((byte)mt.slots[mt.leader].state);
            }
            else
                WriteB(new byte[43]);
        }
    }
}