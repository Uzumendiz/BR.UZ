namespace PointBlank.Auth
{
    public class PROTOCOL_SERVER_MESSAGE_EVENT_RANKUP_ACK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(2616);
            //Interação bugada. Mensagem re-cria toda hora.
            //Bônus padrão no .exe: 50.000 GOLD.
        }
    }
}