using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_LIST_CONTEXT_ACK : GamePacketWriter
    {
        private int clansCount;
        public PROTOCOL_CLAN_LIST_CONTEXT_ACK(int clansCount)
        {
            this.clansCount = clansCount;
        }

        public override void Write()
        {
            WriteH(1452);
            WriteD(clansCount);
            WriteC(170);
            WriteH((ushort)Math.Ceiling(clansCount / 170d));
            WriteD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
        }
    }
}