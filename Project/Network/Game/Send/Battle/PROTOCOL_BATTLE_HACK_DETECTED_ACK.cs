namespace PointBlank.Game
{
    public class BATTLE_HACK_DETECTED_PAK : GamePacketWriter
    {
        private int slotId;
        public BATTLE_HACK_DETECTED_PAK(int slotId)
        {
            this.slotId = slotId;
        }

        public override void Write()
        {
            WriteH(3413);
            WriteC((byte)slotId); //Slot do hacker
            WriteC(1); //?
            WriteD(1); //?
        }
    }
}