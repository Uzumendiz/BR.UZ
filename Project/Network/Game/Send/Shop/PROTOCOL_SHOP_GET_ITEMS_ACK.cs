namespace PointBlank.Game
{
    public class SHOP_GET_ITEMS_PAK : GamePacketWriter
    {
        private int total;
        private ShopData data;
        public SHOP_GET_ITEMS_PAK(ShopData data, int total)
        {
            this.data = data;
            this.total = total;
        }
        public override void Write()
        {
            WriteH(525); //1111 itens por página
            WriteD(total);
            WriteD(data.ItemsCount);
            WriteD(data.Offset);
            WriteB(data.Buffer);
            WriteD(44);
        }
    }
}