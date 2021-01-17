namespace PointBlank.Game
{
    public class AUTH_JACKPOT_NOTICE_PAK : GamePacketWriter
    {
        private string winner;
        private int cupomId;
        private byte index;
        public AUTH_JACKPOT_NOTICE_PAK(string winner, int cupomId, byte index)
        {
            this.winner = winner;
            this.cupomId = cupomId;
            this.index = index;
        }

        public override void Write()
        {
            WriteH(557);
            WriteC((byte)(winner.Length + 1));
            WriteS(winner, winner.Length + 1);
            WriteD(cupomId);
            WriteC(index);
        }
    }
}