namespace PointBlank.Game
{
    public class CLAN_WAR_REGIST_MERCENARY_PAK : GamePacketWriter
    {
        private Match m;
        public CLAN_WAR_REGIST_MERCENARY_PAK(Match m)
        {
            this.m = m;
        }

        public override void Write()
        {
            WriteH(1552);
            WriteH((short)m.GetServerInfo());
            WriteC((byte)m.state);
            WriteC((byte)m.friendId);
            WriteC((byte)m.formação);
            WriteC((byte)m.GetCountPlayers());
            WriteD(m.leader);
            WriteC(0);
            foreach (SlotMatch s in m.slots)
            {
                Account p = m.GetPlayerBySlot(s);
                if (p != null)
                {
                    WriteC(p.rankId);
                    WriteS(p.nickname, 33);
                    WriteQ(s.playerId);
                    WriteC((byte)s.state);
                }
                else WriteB(new byte[43]);
            }
        }
    }
}