namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_SAVEINFO3_ACK : GamePacketWriter
    {
        private uint error;
        public PROTOCOL_CLAN_SAVEINFO3_ACK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(1373);
            WriteD(error);
        }
    }
}