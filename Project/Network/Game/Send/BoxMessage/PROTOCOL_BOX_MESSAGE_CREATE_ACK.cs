namespace PointBlank.Game
{
    public class BOX_MESSAGE_CREATE_PAK : GamePacketWriter
    {
        private uint error;
        public BOX_MESSAGE_CREATE_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(418);
            WriteD(error);
        }
    }
}