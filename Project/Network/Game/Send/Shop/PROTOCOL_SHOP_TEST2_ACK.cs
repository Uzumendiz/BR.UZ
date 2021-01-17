namespace PointBlank.Game
{
    public class SHOP_TEST2_PAK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(567); //Não existe
            WriteD(0);
            WriteD(0);
            WriteD(0);
            WriteD(44); //356
        }
    }
}