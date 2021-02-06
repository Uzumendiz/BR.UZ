namespace PointBlank.Api
{
    public class API_ATTACK_NOTIFY_ACK : ApiPacketWriter
    {
        private string text;
        public API_ATTACK_NOTIFY_ACK(string text)
        {
            this.text = text;
        }

        public override void Write()
        {
            WriteH(16);
            WriteC((byte)text.Length);
            WriteS(text, text.Length);
        }
    }
}