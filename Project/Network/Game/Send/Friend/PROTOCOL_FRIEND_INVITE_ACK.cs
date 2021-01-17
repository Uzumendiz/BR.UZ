namespace PointBlank.Game
{
    public class FRIEND_INVITE_PAK : GamePacketWriter
    {
        private uint error;
        public FRIEND_INVITE_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(283);
            WriteD(error);
            /*
             * 0x80001038 STR_TBL_GUI_BASE_FAIL_REQUEST_FRIEND_BY_LIMIT
             * 0x80001038 STR_TBL_GUI_BASE_NO_MORE_GET_FRIEND_STATE
             * 0x80001041 STR_TBL_GUI_BASE_ALREADY_REGIST_FRIEND_LIST
             * 
             */
        }
    }
}