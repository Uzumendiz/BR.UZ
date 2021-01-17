namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_WEB_CASH_ACK : GamePacketWriter
    {
        private int erro, gold, cash;
        public PROTOCOL_BASE_WEB_CASH_ACK(int erro, int gold = 0, int cash = 0)
        {
            this.erro = erro;
            this.gold = gold;
            this.cash = cash;
        }

        public override void Write()
        {
            WriteH(545);
            WriteD(erro);
            if (erro >= 0)
            {
                WriteD(gold);
                WriteD(cash);
            }
        }
    }
}