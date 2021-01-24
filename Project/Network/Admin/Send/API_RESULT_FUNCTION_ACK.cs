namespace PointBlank.Api
{
    public class API_RESULT_FUNCTION_ACK : ApiPacketWriter
    {
        private int error;
        private byte type;
        private string response;
        public API_RESULT_FUNCTION_ACK(int error, byte type, string response)
        {
            this.error = error;
            this.type = type;
            this.response = response;
        }

        public override void Write()
        {
            WriteH(5);
            WriteD(error);
            WriteC(type);
            WriteC((byte)response.Length);
            WriteS(response, response.Length);
        }
    }
}
