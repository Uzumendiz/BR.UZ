using PointBlank.Game;

namespace PointBlank
{
    public class CMD_KICK_REQ : PacketCommand
    {
        private string command;
        private byte type;
        public CMD_KICK_REQ(string command, byte type)
        {
            this.command = command;
            this.type = type;
        }

        public override void RunImplement()
        {
            if (type == 1)
            {
                string playerName = command.Substring(5);
                Account victim = AccountManager.GetAccount(playerName, 0);
                if (victim == null)
                {
                    response = "Não foi possivel encontrar o jogador.";
                }
                else if (victim.access > administrador.access)
                {
                    response = "Você não tem permissão para desconectar este jogador.";
                }
                else if (victim.playerId == administrador.playerId)
                {
                    response = "Você não pode se desconectar.";
                }
                else if (victim.client != null)
                {
                    victim.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2));
                    victim.Close(1000, true);
                    response = $"O jogador {victim.nickname} foi desconectado.";
                }
                else
                {
                    response = $"O jogador {victim.nickname} não está online.";
                }
            }
            else if (type == 2)
            {
                int count = 0;
                using (AUTH_ACCOUNT_KICK_PAK packet = new AUTH_ACCOUNT_KICK_PAK(0))
                {
                    if (GameManager.SocketSessions.Count > 0)
                    {
                        byte[] data = packet.GetCompleteBytes("KickAllPlayers");
                        foreach (GameClient client in GameManager.SocketSessions.Values)
                        {
                            Account player = client.SessionPlayer;
                            if (player != null && player.isOnline && player.access <= AccessLevelEnum.TransmissionChampionships)
                            {
                                player.SendCompletePacket(data);
                                player.Close(1000, true);
                                count++;
                            }
                        }
                    }
                }
                response = $"Você desconectou {count} jogadores do servidor.";
            }
            else if (type == 3)
            {
                int count = GameManager.KickActiveClient();
                response = $"Foram desconectados {count} jogadores por inatividade.";
            }
        }
    }
}