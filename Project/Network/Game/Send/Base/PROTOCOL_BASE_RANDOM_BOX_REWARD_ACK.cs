namespace PointBlank.Game
{
    public class AUTH_RANDOM_BOX_REWARD_PAK : GamePacketWriter
    {
        private int cupomId;
        private byte index;
        public AUTH_RANDOM_BOX_REWARD_PAK(int cupomId, byte index)
        {
            this.cupomId = cupomId;
            this.index = index;
        }

        public override void Write()
        {
            WriteH(551);
            WriteD(cupomId);
            WriteC(index);
        }
    }
}