namespace PointBlank.Api
{
    public class API_LOGIN_ADMIN_RESULT_ACK : ApiPacketWriter
    {
        private byte result;
        public API_LOGIN_ADMIN_RESULT_ACK(byte result)
        {
            this.result = result;
        }
        public override void Write()
        {
            WriteH(9);
            WriteC(result);
        }
    }
}