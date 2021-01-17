namespace PointBlank.Game
{
    public class BATTLE_MISSION_BOMB_UNINSTALL_PAK : GamePacketWriter
    {
        private int slotId;
        public BATTLE_MISSION_BOMB_UNINSTALL_PAK(int slotId)
        {
            this.slotId = slotId;
        }

        public override void Write()
        {
            WriteH(3359);
            WriteD(slotId);
        }
    }
}