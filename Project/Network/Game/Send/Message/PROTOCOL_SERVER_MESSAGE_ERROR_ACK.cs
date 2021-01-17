namespace PointBlank.Game
{
    public class SERVER_MESSAGE_ERROR_PAK : GamePacketWriter
    {
        private uint error;
        public SERVER_MESSAGE_ERROR_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(2054);
            WriteD(error);
            /*
             * 0x800010AD STBL_IDX_EP_GAME_EXIT_HACKUSER
             * 0x800010AE STBL_IDX_EP_GAMEGUARD_ERROR (Ocorreu um problema no HackShield)
             * 0x800010AF STBL_IDX_EP_GAME_EXIT_ASSERT_E
             * 0x800010B0 STBL_IDX_EP_GAME_EXIT_ASSERT_E
             */
        }
    }
}