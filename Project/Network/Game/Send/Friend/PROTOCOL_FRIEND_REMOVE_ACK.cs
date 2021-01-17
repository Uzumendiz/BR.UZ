namespace PointBlank.Game
{
    public class FRIEND_REMOVE_PAK : GamePacketWriter
    {
        private uint error;
        public FRIEND_REMOVE_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(285);
            WriteD(error);
        }
    }
}