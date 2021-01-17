namespace PointBlank.Api
{
    public class API_RESULT_FUNCTION_ACK : ApiPacketWriter
    {
        private string response;
        public API_RESULT_FUNCTION_ACK(string response)
        {
            this.response = response;
        }

        public override void Write()
        {
            WriteH(5);
            WriteC((byte)response.Length);
            WriteS(response, response.Length);
        }
    }
}
