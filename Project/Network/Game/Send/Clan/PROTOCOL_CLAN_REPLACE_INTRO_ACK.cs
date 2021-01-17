namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_REPLACE_INTRO_ACK : GamePacketWriter
    {
        private uint error;
        public PROTOCOL_CLAN_REPLACE_INTRO_ACK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(1365);
            WriteD(error);
        }
    }
}