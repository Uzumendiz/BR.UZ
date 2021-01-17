using System.Collections.Generic;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_MESSAGES_ACK : GamePacketWriter
    {
        private byte pageIdx;
        private List<Message> messages = new List<Message>();
        public PROTOCOL_BASE_USER_MESSAGES_ACK(byte pageIdx, List<Message> messages)
        {
            this.pageIdx = pageIdx;
            int count = 0;
            for (int i = pageIdx * 25; i < messages.Count; i++)
            {
                this.messages.Add(messages[i]);
                if (++count == 25)
                {
                    break;
                }
            }
        }

        public override void Write()
        {
            WriteH(421);
            WriteC(pageIdx); //PageIdx
            WriteC((byte)messages.Count); //Max=25
            for (int i = 0; i < messages.Count; i++)
            {
                Message msg = messages[i];
                WriteD(msg.objectId);
                WriteQ(msg.senderId);
                WriteC(msg.type);
                WriteC((byte)msg.state);
                WriteC(msg.DaysRemaining);
                WriteD(msg.clanId);
            }
            for (int i = 0; i < messages.Count; i++)
            {
                Message message = messages[i];
                WriteC((byte)(message.senderName.Length + 1));
                WriteC((byte)(message.type == 5 || message.type == 4 && message.noteEnum != 0 ? 0 : (message.text.Length + 1)));
                WriteS(message.senderName, message.senderName.Length + 1);
                if (message.type == 5 || message.type == 4)
                {
                    if (message.noteEnum >= NoteMessageClanEnum.JoinAccept && message.noteEnum <= NoteMessageClanEnum.Secession)
                    {
                        WriteC((byte)(message.text.Length + 1));
                        WriteC((byte)message.noteEnum);
                        WriteS(message.text, message.text.Length);
                    }
                    else if (message.noteEnum == 0)
                    {
                        WriteS(message.text, message.text.Length + 1);
                    }
                    else
                    {
                        WriteC(2);
                        WriteH((short)message.noteEnum);
                    }
                }
                else
                {
                    WriteS(message.text, message.text.Length + 1);
                }
            }
            messages = null;
        }
    }
}