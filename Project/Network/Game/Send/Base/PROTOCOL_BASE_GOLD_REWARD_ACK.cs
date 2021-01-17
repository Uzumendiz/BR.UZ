namespace PointBlank.Game
{
    public class AUTH_GOLD_REWARD_PAK : GamePacketWriter
    {
        private int gold, goldIncrease, type;
        public AUTH_GOLD_REWARD_PAK(int goldIncrease, int gold, int type)
        {
            this.goldIncrease = goldIncrease;
            this.gold = gold;
            this.type = type;
        }

        public override void Write()
        {
            WriteH(561);
            WriteD(goldIncrease);
            WriteD(gold);
            WriteD(type); //Faz aparecer STR_POPUP_GET_POINT
        }
    }
}