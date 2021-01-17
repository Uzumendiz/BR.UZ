namespace PointBlank.Game
{
    public class CLAN_WAR_JOIN_TEAM_PAK : GamePacketWriter
    {
        private Match m;
        private uint _erro;
        public CLAN_WAR_JOIN_TEAM_PAK(uint erro, Match m = null)
        {
            _erro = erro;
            this.m = m;
        }
        public override void Write()
        {
            WriteH(1549);
            WriteD(_erro);
            if (_erro == 0)
            {
                WriteH((short)m.matchId);
                WriteH((ushort)m.GetServerInfo());
                WriteH((ushort)m.GetServerInfo());
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
                for (int i = 0; i < m.formação; i++)
                {
                    SlotMatch s = m.slots[i];
                    Account pS = m.GetPlayerBySlot(s);
                    if (pS != null)
                    {
                        WriteC(pS.rankId);
                        WriteS(pS.nickname, 33);
                        WriteQ(pS.playerId);
                        WriteC((byte)s.state);
                    }
                    else
                        WriteB(new byte[43]);
                }
            }
        }
    }
}