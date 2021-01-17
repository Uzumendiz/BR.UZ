namespace PointBlank.Game
{
    public class BATTLE_PLAYED_TIME_PAK : GamePacketWriter
    {
        private int type;
        private PlayTimeModel eventPlayTime;
        public BATTLE_PLAYED_TIME_PAK(int type, PlayTimeModel eventPlayTime)
        {
            this.type = type;
            this.eventPlayTime = eventPlayTime;
        }

        public override void Write()
        {
            WriteH(3911);
            WriteC((byte)type);
            WriteS(eventPlayTime.title, 30);
            WriteD(eventPlayTime.goodReward1);
            WriteD(eventPlayTime.goodReward2);
            WriteQ(eventPlayTime.time);
        }
    }
}