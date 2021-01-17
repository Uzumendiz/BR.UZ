namespace PointBlank.Game
{
    public class PROTOCOL_BASE_WEB_CASH_ACK : GamePacketWriter
    {
        private int error, gold, cash;
        public PROTOCOL_BASE_WEB_CASH_ACK(int error, int gold = 0, int cash = 0)
        {
            this.error = error;
            this.gold = gold;
            this.cash = cash;
        }

        public override void Write()
        {
            WriteH(545);
            WriteD(error);
            WriteD(gold);
            WriteD(cash);
        }
    }
}