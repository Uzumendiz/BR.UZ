namespace PointBlank.Game
{
    public class BATTLE_RESPAWN_FOR_AI_PAK : GamePacketWriter
    {
        private int slotId;
        public BATTLE_RESPAWN_FOR_AI_PAK(int slotId)
        {
            this.slotId = slotId;
        }

        public override void Write()
        {
            WriteH(3379);
            WriteD(slotId);
        }
    }
}