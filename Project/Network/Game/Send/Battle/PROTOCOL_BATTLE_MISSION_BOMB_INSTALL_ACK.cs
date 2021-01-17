namespace PointBlank.Game
{
    public class BATTLE_MISSION_BOMB_INSTALL_PAK : GamePacketWriter
    {
        private int slotId;
        private float x, y, z;
        private byte zone;
        public BATTLE_MISSION_BOMB_INSTALL_PAK(int slotId, byte zone, float x, float y, float z)
        {
            this.slotId = slotId;
            this.zone = zone;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override void Write()
        {
            WriteH(3357);
            WriteD(slotId);
            WriteC(zone);
            WriteH(42);
            WriteT(x);
            WriteT(y);
            WriteT(z);
        }
    }
}