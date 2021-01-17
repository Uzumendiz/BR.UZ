namespace PointBlank.Game
{
    public class BASE_USER_ENTER_PAK : GamePacketWriter
    {
        private uint error;
        public BASE_USER_ENTER_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(2580);
            WriteD(error);
        }
    }
}