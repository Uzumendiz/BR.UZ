namespace PointBlank.Game
{
    public class BASE_QUEST_COMPLETE_PAK : GamePacketWriter
    {
        private byte missionId;
        private byte value;
        public BASE_QUEST_COMPLETE_PAK(byte progress, Card card)
        {
            missionId = (byte)card.missionBasicId;
            if (card.missionLimit == progress)
            {
                missionId += 240;
            }
            value = progress;
        }

        public override void Write()
        {
            WriteH(2600);
            WriteC(missionId);
            WriteC(value);
        }
    }
}