namespace PointBlank.Game
{
    public class INVENTORY_ENTER_PAK : GamePacketWriter
    {
        private int date;
        public INVENTORY_ENTER_PAK(int date)
        {
            this.date = date;
        }
        public override void Write()
        {
            WriteH(3586);
            WriteD(date);
        }
    }
}