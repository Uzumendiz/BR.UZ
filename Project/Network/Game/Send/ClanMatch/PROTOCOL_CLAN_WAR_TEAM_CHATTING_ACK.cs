namespace PointBlank.Game
{
    public class CLAN_WAR_TEAM_CHATTING_PAK : GamePacketWriter
    {
        private int type, bantime;
        private string message, sender;
        public CLAN_WAR_TEAM_CHATTING_PAK(string sender, string text)
        {
            this.sender = sender;
            message = text;
        }
        public CLAN_WAR_TEAM_CHATTING_PAK(int type, int bantime)
        {
            this.type = type;
            this.bantime = bantime;
        }
        public override void Write()
        {
            WriteH(1577);
            WriteC((byte)type);
            if (type == 0)
            {
                WriteC((byte)(sender.Length + 1));
                WriteS(sender, sender.Length + 1);
                WriteC((byte)message.Length);
                WriteS(message, message.Length);
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