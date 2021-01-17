namespace PointBlank.Game
{
    public class SERVER_MESSAGE_ANNOUNCE_PAK : GamePacketWriter
    {
        private string text;
        public SERVER_MESSAGE_ANNOUNCE_PAK(string text)
        {
            this.text = text;
        }

        public override void Write()
        {
            WriteH(2055);
            WriteD(2); //Tipo da notícia [NOTICE_TYPE_NORMAL - 1 || NOTICE_TYPE_EMERGENCY - 2]
            WriteH((ushort)text.Length);
            WriteS(text);
        }
    }
}