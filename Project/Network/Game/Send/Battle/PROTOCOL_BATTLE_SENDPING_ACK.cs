namespace PointBlank.Game
{
    public class BATTLE_SENDPING_PAK : GamePacketWriter
    {
        private byte[] slots;
        public BATTLE_SENDPING_PAK(byte[] slots)
        {
            this.slots = slots;
        }

        public override void Write()
        {
            WriteH(3345);
            WriteB(slots);
        }
    }
}