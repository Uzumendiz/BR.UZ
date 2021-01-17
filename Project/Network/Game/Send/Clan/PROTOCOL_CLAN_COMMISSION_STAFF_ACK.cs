namespace PointBlank.Game
{
    public class CLAN_COMMISSION_STAFF_PAK : GamePacketWriter
    {
        private uint result;
        public CLAN_COMMISSION_STAFF_PAK(uint result)
        {
            this.result = result;
        }

        public override void Write()
        {
            WriteH(1341);
            WriteD(result);
        }
    }
}