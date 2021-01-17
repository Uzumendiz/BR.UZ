namespace PointBlank.Game
{
    public class EVENT_VISIT_REWARD_PAK : GamePacketWriter
    {
        private uint error;
        public EVENT_VISIT_REWARD_PAK(EventErrorEnum error)
        {
            this.error = (uint)error;
        }

        public override void Write()
        {
            WriteH(2664);
            WriteD(error);
        }
    }
}