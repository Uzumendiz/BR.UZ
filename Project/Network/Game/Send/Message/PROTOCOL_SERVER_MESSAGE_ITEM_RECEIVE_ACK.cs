namespace PointBlank.Game
{
    public class SERVER_MESSAGE_ITEM_RECEIVE_PAK : GamePacketWriter
    {
        private uint error;
        public SERVER_MESSAGE_ITEM_RECEIVE_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(2692);
            WriteD(error);
        }
    }
}