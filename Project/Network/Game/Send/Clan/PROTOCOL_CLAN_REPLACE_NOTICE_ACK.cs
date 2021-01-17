namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_REPLACE_NOTICE_ACK : GamePacketWriter
    {
        private uint error;
        public PROTOCOL_CLAN_REPLACE_NOTICE_ACK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(1363);
            WriteD(error);
        }
    }
}