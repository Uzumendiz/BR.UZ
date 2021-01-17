namespace PointBlank
{
    public class RankModel
    {
        public byte rankId;
        public int onNextLevel, onGoldUp, onAllExp;
        public int brooch, insignia, medal, blueorder;
        public RankModel(byte rankId, int onNextLevel, int onGoldUp, int onAllExp, int brooch, int insignia, int medal, int blueorder)
        {
            this.rankId = rankId;
            this.onNextLevel = onNextLevel;
            this.onGoldUp = onGoldUp;
            this.onAllExp = onAllExp;
            this.brooch = brooch;
            this.insignia = insignia;
            this.medal = medal;
            this.blueorder = blueorder;
        }
        public RankModel(byte rankId, int onNextLevel, int onGoldUp, int onAllExp)
        {
            this.rankId = rankId;
            this.onNextLevel = onNextLevel;
            this.onGoldUp = onGoldUp;
            this.onAllExp = onAllExp;
        }
    }
}