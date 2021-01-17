using System;

namespace PointBlank.Game
{
    public class CLAN_WAR_PARTY_CONTEXT_PAK : GamePacketWriter
    {
        private int matchCount;
        public CLAN_WAR_PARTY_CONTEXT_PAK(int count)
        {
            matchCount = count;
        }

        public override void Write()
        {
            WriteH(1539);
            WriteC((byte)matchCount);
            WriteC(13);
            WriteC((byte)Math.Ceiling(matchCount / 13d));
        }
    }
}