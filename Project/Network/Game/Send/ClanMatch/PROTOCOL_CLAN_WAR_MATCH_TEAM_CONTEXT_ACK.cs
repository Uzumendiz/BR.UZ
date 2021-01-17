using System;

namespace PointBlank.Game
{
    public class CLAN_WAR_MATCH_TEAM_CONTEXT_PAK : GamePacketWriter
    {
        private int count;
        public CLAN_WAR_MATCH_TEAM_CONTEXT_PAK(int count)
        {
            this.count = count;
        }

        public override void Write()
        {
            WriteH(1543);
            WriteH((short)count);
            WriteC(13);
            WriteH((short)Math.Ceiling(count / 13d));
        }
    }
}