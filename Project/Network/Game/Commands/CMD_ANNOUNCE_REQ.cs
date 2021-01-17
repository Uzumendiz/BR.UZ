using PointBlank.Game;

namespace PointBlank
{
    public class CMD_ANNOUNCE_REQ : PacketCommand
    {
        private string command;
        private byte type;
        public CMD_ANNOUNCE_REQ(string command, byte type)
        {
            this.command = command;
            this.type = type;
        }

        public override void RunImplement()
        {
            string message = command.Substring(3);
            if (message.Length >= 1024)
            {
                response = $"Não é possivel mandar uma mensagem muito grande.";
                return;
            }
            int count = 0;
            if (type == 1) //All
            {
                using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(message))
                {
                    count = GameManager.SendPacketToAllClients(packet);
                }
                response = $"Mensagem enviada a {count} jogadores do servidor.";
            }
            else if (type == 2) //Room
            {
                if (room != null)
                {
                    using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(message))
                    {
                        count = room.SendMessageToPlayers(packet);
                    }
                    response = $"Mensagem enviada a {count} jogadores da sala {room.roomId + 1}.";
                }
                else
                {
                    response = "Você precisa estar presente em uma sala.";
                }
            }
        }
    }
}