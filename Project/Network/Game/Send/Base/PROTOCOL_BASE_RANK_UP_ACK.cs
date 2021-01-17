namespace PointBlank.Game
{
    public class PROTOCOL_BASE_RANK_UP_ACK : GamePacketWriter
    {
        private int rankId;
        private int allExp;
        public PROTOCOL_BASE_RANK_UP_ACK(int rankId, int allExp)
        {
            this.rankId = rankId;
            this.allExp = allExp;
        }

        public override void Write()
        {
            WriteH(2614);
            WriteD(rankId);
            WriteD(rankId);
            WriteD(allExp); //EXP
        }
    }
}