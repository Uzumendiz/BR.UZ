using System.Collections.Generic;

namespace PointBlank.Game
{
    public class CLAN_WAR_PARTY_LIST_PAK : GamePacketWriter
    {
        private List<Match> _c;
        private int _erro;
        public CLAN_WAR_PARTY_LIST_PAK(int erro, List<Match> c)
        {
            _erro = erro;
            _c = c;
        }

        public override void Write()
        {
            WriteH(1541);
            WriteC((byte)(_erro == 0 ? _c.Count : _erro)); //Total de itens
            if (_erro > 0 || _c.Count == 0)
                return;
            WriteC(1); //Página atual?
            WriteC(0);
            WriteC((byte)_c.Count); //Total de itens atual
            foreach (Match m in _c)
            {
                WriteH((short)m.matchId);
                WriteH((ushort)m.GetServerInfo());
                WriteH((ushort)m.GetServerInfo());
                WriteC((byte)m.state);
                WriteC((byte)m.friendId);
                WriteC((byte)m.formação);
                WriteC((byte)m.GetCountPlayers());
                WriteC(0);//1
                WriteD(m.leader);
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