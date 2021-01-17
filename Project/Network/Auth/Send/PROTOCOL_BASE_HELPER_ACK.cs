namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_HELPER_ACK : GamePacketWriter
    {
        private short _packet;
        public PROTOCOL_BASE_HELPER_ACK(short packet)
        {
            _packet = packet;
        }

        public override void Write()
        {
            WriteH(_packet);
            WriteD(0); //id do cupom ativo
        }
    }
} //3879 - 4v4error?
  //553 = RECEBER PRESENTE
  //2630 - zera minhas info e pede 2561
  //2685 - log do usuário é salvo