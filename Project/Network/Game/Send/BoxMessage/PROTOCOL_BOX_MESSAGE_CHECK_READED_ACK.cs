using System.Collections.Generic;

namespace PointBlank.Game
{
    public class BOX_MESSAGE_CHECK_READED_PAK : GamePacketWriter
    {
        private List<int> messages;
        public BOX_MESSAGE_CHECK_READED_PAK(List<int> messages)
        {
            this.messages = messages;
        }

        public override void Write()
        {
            WriteH(423);
            WriteC((byte)messages.Count);
            for (int i = 0; i < messages.Count; i++)
            {
                WriteD(messages[i]);
            }
        }
    }
}