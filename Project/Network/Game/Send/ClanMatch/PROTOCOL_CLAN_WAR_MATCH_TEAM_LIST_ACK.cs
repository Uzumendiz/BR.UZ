using System.Collections.Generic;

namespace PointBlank.Game
{
    public class CLAN_WAR_MATCH_TEAM_LIST_PAK : GamePacketWriter
    {
        private List<Match> matchs;
        private int myMatchIdx, _page, MatchCount;
        public CLAN_WAR_MATCH_TEAM_LIST_PAK(int page, List<Match> matchs, int matchId)
        {
            _page = page;
            myMatchIdx = matchId;
            MatchCount = (matchs.Count - 1);
            this.matchs = matchs;
        }

        public override void Write()
        {
            WriteH(1545);
            WriteH((ushort)MatchCount);//Quantidade de clãs na lista
            if (MatchCount == 0)
                return;
            WriteH(1);
            WriteH(0);
            WriteC((byte)MatchCount); //Quantidade de itens da lista a ser lida
            for (int i = 0; i < matchs.Count; i++)
            {
                Match m = matchs[i];
                if (m.matchId == myMatchIdx)
                    continue;
                WriteH((short)m.matchId);
                WriteH((short)m.GetServerInfo());
                WriteH((short)m.GetServerInfo());
                WriteC((byte)m.state);
                WriteC((byte)m.friendId);
                WriteC((byte)m.formação);
                WriteC((byte)m.GetCountPlayers());
                WriteD(m.leader);
                WriteC(0);
                WriteD(m.clan.id);
                WriteC(m.clan.rank);
                WriteD(m.clan.logo);
                WriteS(m.clan.name, 17);
                WriteT(m.clan.pontos);
                WriteC(m.clan.nameColor);
                Account p = m.GetLeader();
                if (p != null)
                {
                    WriteC(p.rankId);
                    WriteS(p.nickname, 33);
                    WriteQ(p.playerId);
                    WriteC((byte)m.slots[m.leader].state);
                }
                else
                {
                    WriteB(new byte[43]);
                }
            }
        }
    }
}