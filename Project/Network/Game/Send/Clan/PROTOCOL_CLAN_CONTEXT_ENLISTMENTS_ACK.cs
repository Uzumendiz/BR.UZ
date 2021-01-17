using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_CONTEXT_ENLISTMENTS_ACK : GamePacketWriter
    {
        private uint errorOrInvites;
        public PROTOCOL_CLAN_CONTEXT_ENLISTMENTS_ACK(uint errorOrInvites)
        {
            this.errorOrInvites = errorOrInvites;
        }

        public override void Write()
        {
            WriteH(1321);
            WriteD(errorOrInvites);
            if (errorOrInvites == 0)
            {
                WriteC((byte)errorOrInvites);
                WriteC(13);
                WriteC((byte)Math.Ceiling(errorOrInvites / 13d));
                WriteD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
            }
        }
    }
}