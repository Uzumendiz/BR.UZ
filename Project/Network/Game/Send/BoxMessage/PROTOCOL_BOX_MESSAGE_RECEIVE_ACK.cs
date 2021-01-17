namespace PointBlank.Game
{
    public class BOX_MESSAGE_RECEIVE_PAK : GamePacketWriter
    {
        private Message message;
        public BOX_MESSAGE_RECEIVE_PAK(Message message)
        {
            this.message = message;
        }

        public override void Write()
        {
            WriteH(427);
            WriteD(message.objectId);
            WriteQ(message.senderId);
            WriteC(message.type);
            WriteC((byte)message.state);
            WriteC(message.DaysRemaining);
            WriteD(message.clanId);
            WriteC((byte)(message.senderName.Length + 1));
            WriteC((byte)(message.type == 5 || message.type == 4 && message.noteEnum != 0 ? 0 : (message.text.Length + 1)));
            WriteS(message.senderName, message.senderName.Length + 1);
            if (message.type == 5 || message.type == 4)
            {
                if (message.noteEnum >= NoteMessageClanEnum.JoinAccept && message.noteEnum <= NoteMessageClanEnum.Secession)
                {
                    WriteC((byte)(message.text.Length + 1));
                    WriteC((byte)message.noteEnum);
                    WriteS(message.text, message.text.Length + 1);
                }
                else if (message.noteEnum == 0)
                {
                    WriteS(message.text, message.text.Length + 1);
                }
                else
                {
                    WriteC(2);
                    WriteH((ushort)message.noteEnum);
                }
            }
            else
            {
                WriteS(message.text, message.text.Length + 1);
            }
        }
    }
}