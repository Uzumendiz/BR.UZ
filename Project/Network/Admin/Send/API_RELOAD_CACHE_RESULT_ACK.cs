namespace PointBlank.Api
{
    public class API_RELOAD_CACHE_RESULT_ACK : ApiPacketWriter
    {
        private string message;
        public API_RELOAD_CACHE_RESULT_ACK(string message)
        {
            this.message = message;
        }
        public override void Write()
        {
            WriteH(9);
            WriteD(message.Length);
            WriteS(message, message.Length);
        }
    }
}