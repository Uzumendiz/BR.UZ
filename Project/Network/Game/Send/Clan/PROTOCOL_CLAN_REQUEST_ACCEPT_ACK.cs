namespace PointBlank.Game
{
    public class CLAN_REQUEST_ACCEPT_PAK : GamePacketWriter
    {
        private int result;
        public CLAN_REQUEST_ACCEPT_PAK(int result)
        {
            this.result = result;
        }

        public override void Write()
        {
            WriteH(1327);
            WriteD(result);
        }
    }
}