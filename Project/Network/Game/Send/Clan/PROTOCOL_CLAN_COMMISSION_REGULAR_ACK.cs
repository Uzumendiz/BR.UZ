namespace PointBlank.Game
{
    public class CLAN_COMMISSION_REGULAR_PAK : GamePacketWriter
    {
        private uint result;
        public CLAN_COMMISSION_REGULAR_PAK(uint result)
        {
            this.result = result;
        }

        public override void Write()
        {
            WriteH(1344);
            WriteD(result);
        }
    }
}