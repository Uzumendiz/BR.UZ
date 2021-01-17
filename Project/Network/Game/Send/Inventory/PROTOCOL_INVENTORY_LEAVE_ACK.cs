namespace PointBlank.Game
{
    public class INVENTORY_LEAVE_PAK : GamePacketWriter
    {
        private int error, type;
        public INVENTORY_LEAVE_PAK(int error, int type = 0)
        {
            this.error = error;
            this.type = type;
        }

        public override void Write()
        {
            WriteH(3590);
            WriteD(error);
            if (error < 0)
            {
                WriteD(type);
            }
        }
    }
}