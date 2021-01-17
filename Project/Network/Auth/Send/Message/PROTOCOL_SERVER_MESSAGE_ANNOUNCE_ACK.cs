namespace PointBlank.Auth
{
    public class PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK : GamePacketWriter
    {
        private string message;
        /// <summary>
        /// Envia uma mensagem global ao jogador. (GM)
        /// </summary>
        /// <param name="message"></param>
        public PROTOCOL_SERVER_MESSAGE_ANNOUNCE_ACK(string message)
        {
            this.message = message;
        }

        public override void Write()
        {
            WriteH(2055);
            WriteD(2); //Tipo da notícia [NOTICE_TYPE_NORMAL - 1 || NOTICE_TYPE_EMERGENCY - 2]
            WriteH((ushort)message.Length);
            WriteS(message);
        }
    }
}