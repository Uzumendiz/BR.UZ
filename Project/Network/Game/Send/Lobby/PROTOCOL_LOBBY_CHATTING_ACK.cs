namespace PointBlank.Game
{
    public class LOBBY_CHATTING_PAK : GamePacketWriter
    {
        private string sender, message;
        private int sessionId;
        private int nameColor;
        private bool GMColor;
        public LOBBY_CHATTING_PAK(Account player, string message, bool GMCmd = false)
        {
            if (!GMCmd)
            {
                nameColor = player.nickcolor;
                GMColor = player.UseChatGM();
            }
            else
            {
                GMColor = true;
            }
            sender = player.nickname;
            sessionId = player.GetSessionId();
            this.message = message;
        }
        public LOBBY_CHATTING_PAK(string sender, int sessionId, int nameColor, bool GMColor, string message)
        {
            this.sender = sender;
            this.sessionId = sessionId;
            this.nameColor = nameColor;
            this.GMColor = GMColor;
            this.message = message;
        }
        public override void Write()
        {
            WriteH(3093);
            WriteD(sessionId);
            WriteC((byte)(sender.Length + 1));
            WriteS(sender, sender.Length + 1);
            WriteC((byte)nameColor);
            WriteC(GMColor);
            WriteH((ushort)(message.Length + 1));
            WriteS(message, message.Length + 1);
        }
    }
}