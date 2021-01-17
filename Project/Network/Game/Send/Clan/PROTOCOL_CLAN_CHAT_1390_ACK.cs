namespace PointBlank.Game
{
    public class CLAN_CHAT_1390_PAK : GamePacketWriter
    {
        private string sender, message;
        private int type, bantime;
        private bool isGM;
        public CLAN_CHAT_1390_PAK(string sender, bool isGM, string message)
        {
            this.sender = sender;
            this.message = message;
            this.isGM = isGM;
        }
        public CLAN_CHAT_1390_PAK(int type, int bantime)
        {
            this.type = type;
            this.bantime = bantime;
        }
        public override void Write()
        {
            WriteH(1391);
            WriteC((byte)type);
            if (type == 0)
            {
                WriteC((byte)(sender.Length + 1));
                WriteS(sender, sender.Length + 1);
                WriteC(isGM);
                WriteC((byte)(message.Length + 1));
                WriteS(message, message.Length + 1);
            }
            else
                WriteD(bantime);
            /*
             * 1=STR_MESSAGE_BLOCK_ING
             * 2=STR_MESSAGE_BLOCK_START
             */
        }
    }
}