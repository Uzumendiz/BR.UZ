namespace PointBlank.Game
{
    public class INVENTORY_ITEM_EXCLUDE_PAK : GamePacketWriter
    {
        private long objectId;
        private uint error;
        public INVENTORY_ITEM_EXCLUDE_PAK(uint error, long objectId = 0)
        {
            this.error = error;
            this.objectId = objectId;
        }
        public override void Write()
        {
            WriteH(543);
            WriteD(error);
            if (error == 1)
            {
                WriteQ(objectId);
            }
        }
    }
}