using PointBlank.Game;

namespace PointBlank
{
    public class CMD_PLAYERINFO_REQ : PacketCommand
    {
        private string command;
        private byte type;
        public CMD_PLAYERINFO_REQ(string command, byte type)
        {
            this.command = command;
            this.type = type;
        }

        public override void RunImplement()
        {
            string[] values = command.Substring(5).Split('=');
            string nickname = values[0];
            int valor = int.Parse(values[1]);
            Account player = AccountManager.GetAccount(nickname, 0);
            if (player != null)
            {
                if (type == 1)
                {
                    long cashCalculated = player.cash + valor;
                    if (cashCalculated > 999999999)
                    {
                        response = "Não é possivel adicionar esse valor de cash para este jogador no momento.";
                    }
                    else
                    {
                        int cashValid = (int)cashCalculated;
                        if (player.UpdateAccountCash(cashValid))
                        {
                            player.cash += cashValid;
                            player.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
                            response = $"O jogador {player.nickname} recebeu {valor} de cash.";
                        }
                        else
                        {
                            response = "Não foi possivel atualizar o cash para este jogador.";
                        }
                    }
                }
                else if (type == 2)
                {
                    long goldCalculated = player.gold + valor;
                    if (goldCalculated > 999999999)
                    {
                        response = "Não é possivel adicionar esse valor de gold para este jogador no momento.";
                    }
                    else
                    {
                        int goldValid = (int)goldCalculated;
                        if (player.UpdateAccountGold(goldValid))
                        {
                            player.gold += goldValid;
                            player.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
                            response = $"O jogador {player.nickname} recebeu {valor} de gold.";
                        }
                        else
                        {
                            response = "Não foi possivel atualizar o gold para este jogador.";
                        }
                    }
                }
            }
            else
            {
                response = $"O jogador {nickname} é inexistente.";
            }
        }
    }
}