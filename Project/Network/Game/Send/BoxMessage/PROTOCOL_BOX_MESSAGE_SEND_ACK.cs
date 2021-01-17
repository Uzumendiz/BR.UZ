namespace PointBlank.Game
{
    public class BOX_MESSAGE_SEND_PAK : GamePacketWriter
    {
        private uint error;
        public BOX_MESSAGE_SEND_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(441);
            WriteD(error);
            //0x8000107E STR_TBL_NETWORK_NOT_FIND_USER
            //0x8000107F STR_TBL_NETWORK_FAIL_SEND_MESSAGE_RECEIVER_FULL
            //0x80001080 STR_TBL_NETWORK_DONT_SEND_MYSELF_MESSAGE
            //0x80001081 STR_TBL_NETWORK_FULL_SEND_MESSAGE_PER_DAY
        }
    }
}