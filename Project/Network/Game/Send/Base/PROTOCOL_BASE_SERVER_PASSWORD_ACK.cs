namespace PointBlank.Game
{
    public class BASE_SERVER_PASSWORD_PAK : GamePacketWriter
    {
        private uint error;
        public BASE_SERVER_PASSWORD_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(2645);
            WriteD(error);
        }
    }
}