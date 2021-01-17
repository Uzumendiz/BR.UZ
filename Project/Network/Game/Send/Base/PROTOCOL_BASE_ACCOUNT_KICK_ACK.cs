namespace PointBlank.Game
{
    public class AUTH_ACCOUNT_KICK_PAK : GamePacketWriter
    {
        private byte type;
        public AUTH_ACCOUNT_KICK_PAK(byte type)
        {
            this.type = type;
        }

        public override void Write()
        {
            WriteH(513);
            WriteC(type); //0 (O jogo será finalizado por solicitação do servidor) || 1 (Conexão simultanêa) || 2 (Jogo finalizado em instantes [GM]) || 3
        }
    }
}