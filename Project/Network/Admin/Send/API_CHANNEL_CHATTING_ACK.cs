namespace PointBlank.Api
{
    public class API_CHANNEL_CHATTING_ACK : ApiPacketWriter
    {
        private Account sender;
        private string text;
        public API_CHANNEL_CHATTING_ACK(Account sender, string text)
        {
            this.sender = sender;
            this.text = text;
        }

        public override void Write()
        {
            WriteH(12);
            WriteD(sender.GetSessionId());
            WriteD(sender.channelId);
            WriteC((byte)(sender.nickname.Length + 1));
            WriteS(sender.nickname, sender.nickname.Length + 1);
            WriteH((ushort)(text.Length + 1));
            WriteS(text, text.Length + 1);
        }
    }
}