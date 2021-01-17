namespace PointBlank.Api
{
    public class API_BATTLE_TESTE_RESULT_ACK : ApiPacketWriter
    {
        private byte result;
        public API_BATTLE_TESTE_RESULT_ACK(byte result)
        {
            this.result = result;
        }
        public override void Write()
        {
            WriteH(20);
            WriteC(result);
        }
    }
}