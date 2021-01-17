namespace PointBlank.Game
{
    public class SHOP_GET_REPAIR_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(559);
            WriteD(0); //COUNT
            WriteD(0); //COUNT
            WriteD(0); //Offset

            //writeD(100003188); //ItemId?
            //writeD(59); //GoldRepairPrice?
            //writeD(0); //CashRepairPrice?
            //writeD(100); //Max durability?
            /*
             * ID DO ITEM - WRITED
             * UNK?? - WRITED (Geralmente varia) Ou é esse que tem valor
             * UNK?? - WRITED (Geralmente varia) Repair Type (0 = ? | 1= GOLD | 2=CASH)
             * COUNT - WRITED (100)
             * 
             * 
             */

            WriteD(44); //356
        }
    }
}