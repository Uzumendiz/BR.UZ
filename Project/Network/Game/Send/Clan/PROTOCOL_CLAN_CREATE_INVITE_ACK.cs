namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_CREATE_INVITE_ACK : GamePacketWriter
    {
        private int clanId;
        private uint error;
        public PROTOCOL_CLAN_CREATE_INVITE_ACK(uint error, int clanId)
        {
            this.error = error;
            this.clanId = clanId;
        }

        public override void Write()
        {
            WriteH(1317);
            WriteD(error);
            if (error == 0)
            {
                WriteD(clanId);
            }
        }
    }
}