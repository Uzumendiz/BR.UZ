namespace PointBlank.Game
{
    public class CLAN_CHATTING_PAK : GamePacketWriter
    {
        private string text;
        private int type, bantime;
        private string sender;
        private bool isGM;
        public CLAN_CHATTING_PAK(string sender, bool isGM, string text)
        {
            this.sender = sender;
            this.isGM = isGM;
            this.text = text;
        }
        public CLAN_CHATTING_PAK(int type, int bantime)
        {
            this.type = type;
            this.bantime = bantime;
        }
        public override void Write()
        {
            WriteH(1359);
            WriteC((byte)type);
            if (type == 0)
            {
                WriteC((byte)(sender.Length + 1));
                WriteS(sender, sender.Length + 1);
                WriteC(isGM);
                WriteC((byte)(text.Length + 1));
                WriteS(text, text.Length + 1);
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