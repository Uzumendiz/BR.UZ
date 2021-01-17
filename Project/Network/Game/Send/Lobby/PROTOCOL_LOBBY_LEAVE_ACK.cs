namespace PointBlank.Game
{
    public class LOBBY_LEAVE_PAK : GamePacketWriter
    {
        private uint error;
        public LOBBY_LEAVE_PAK(uint error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(3084);
            WriteD(error);
        }
    }
}