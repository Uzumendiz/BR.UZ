namespace PointBlank.Game
{
    public class FRIEND_INVITE_FOR_ROOM_PAK : GamePacketWriter
    {
        private uint error;
        public FRIEND_INVITE_FOR_ROOM_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(276);
            WriteD(error);
            /*
             * 2147495938 STR_TBL_NETWORK_FAIL_INVITED_USER
             * 2147495939 STR_TBL_NETWORK_FAIL_INVITED_USER_IN_CLAN_MATCH
             */
        }
    }
}