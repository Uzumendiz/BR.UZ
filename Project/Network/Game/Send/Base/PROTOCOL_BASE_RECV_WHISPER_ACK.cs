namespace PointBlank.Game
{
    public class AUTH_RECV_WHISPER_PAK : GamePacketWriter
    {
        private string sender, message;
        private bool chatGM;
        public AUTH_RECV_WHISPER_PAK(string sender, string message, bool chatGM)
        {
            this.sender = sender;
            this.message = message;
            this.chatGM = chatGM;
        }

        public override void Write()
        {
            WriteH(294);
            WriteS(sender, 33);
            WriteC(chatGM);
            WriteH((ushort)(message.Length + 1));
            WriteS(message, message.Length + 1);
        }
    }
}