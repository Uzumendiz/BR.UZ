namespace PointBlank.Game
{
    public class SHOP_GET_GOODS_PAK : GamePacketWriter
    {
        private int total;
        private ShopData data;
        public SHOP_GET_GOODS_PAK(ShopData data, int total)
        {
            this.data = data; 
            this.total = total;
        }
        public override void Write()
        {
            WriteH(523); //592 itens por página
            WriteD(total);
            WriteD(data.ItemsCount);
            WriteD(data.Offset);
            WriteB(data.Buffer);
            WriteD(44); //356
        }
    }
}