namespace PointBlank.Game
{
    public class CLAN_WAR_CREATE_TEAM_PAK : GamePacketWriter
    {
        private uint _erro;
        private Match m;
        public CLAN_WAR_CREATE_TEAM_PAK(uint erro, Match mt = null)
        {
            _erro = erro;
            m = mt;
        }
        public override void Write()
        {
            WriteH(1547);
            WriteD(_erro);
            if (_erro == 0)
            {
                WriteH((short)m.matchId);
                WriteH((short)m.GetServerInfo());
                WriteH((short)m.GetServerInfo());
                WriteC((byte)m.state);
                WriteC((byte)m.friendId);
                WriteC((byte)m.formação);
                WriteC((byte)m.GetCountPlayers());
                WriteD(m.leader);
                WriteC(0);
            }
        }
    }
}