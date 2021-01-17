namespace PointBlank.Game
{
    public class FRIEND_ACCEPT_PAK : GamePacketWriter
    {
        private uint error;
        public FRIEND_ACCEPT_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(281);
            WriteD(error);
        }
    }
}