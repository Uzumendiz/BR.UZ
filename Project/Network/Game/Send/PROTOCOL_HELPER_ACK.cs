namespace PointBlank.Game
{
    public class HELPER_PAK : GamePacketWriter
    {
        private ushort packet;
        public HELPER_PAK(ushort packet)
        {
            this.packet = packet;
        }

        public override void Write()
        {
            WriteH(packet);
            WriteD(0);
            WriteC(1);
        }
    }
} //3879 - 4v4error?
  //553 = RECEBER PRESENTE
  //2630 - zera minhas info e pede 2561
  //2685 - log do usuário é salvo