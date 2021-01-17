namespace PointBlank.Game
{
    public class AUTH_SEND_WHISPER_PAK : GamePacketWriter
    {
        private string name, message;
        private uint error;
        private int type, bantime;
        public AUTH_SEND_WHISPER_PAK(string name, string message, uint error)
        {
            this.name = name;
            this.message = message;
            this.error = error;
        }
        public AUTH_SEND_WHISPER_PAK(int type, int bantime)
        {
            this.type = type;
            this.bantime = bantime;
        }
        public override void Write()
        {
            WriteH(291);
            WriteC((byte)type);
            if (type == 0)
            {
                WriteD(error);
                WriteS(name, 33);
                if (error == 0)
                {
                    WriteH((ushort)(message.Length + 1));
                    WriteS(message, message.Length + 1);
                }
            }
            else
            {
                WriteD(bantime);
            }
            /*
             * 1=STR_MESSAGE_BLOCK_ING
             * 2=STR_MESSAGE_BLOCK_START
             */
        }
    }
}