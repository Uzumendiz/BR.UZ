using PointBlank.Game;

namespace PointBlank
{
    public class CMD_HELP_REQ : PacketCommand
    {
        private byte type;
        public CMD_HELP_REQ(byte type)
        {
            this.type = type;
        }

        public override void RunImplement()
        {
            if (InGame(administrador))
            {
                response = "Não é possível exibir a lista de comandos durante a partida.";
            }
            else
            {
                if (type == 1) //MOD
                {
                    string comandos = "Lista de comandos para todas as autoridades.\n";

                    comandos += "\n" + "Expulsar jogador do servidor: [.kick (Nickname)]";
                    comandos += "\n" + "Expulsar todos do servidor: [.kickall]";
                    comandos += "\n" + "Expulsar jogadores AFK:  [.afkkick]";
                    comandos += "\n" + "Jogadores online:  [.online]";
                    comandos += "\n" + "Mensagem para geral: [.g1 (Mensagem)]";
                    comandos += "\n" + "Mensagem somente para players da sala:  [.g2 (Mensagem)]";
                    comandos += "\n" + "Adicionar Cash:  [.cash (Nickname)]";
                    comandos += "\n" + "Adicionar Gold:  [.gold (Nickname)]";
                    comandos += "\n" + "Alterar meu rank:  [.rank (0-54)]";
                    comandos += "\n" + "Alterar meu nick:  [.nick (Nickname)]";
                    comandos += "\n" + "Adicionar item: [.additem (ID)]";
                    comandos += "\n" + "Ativar/Desativar cor do nick (GM): [.gmcolor]";
                    comandos += "\n" + "Ativar/Desativar antikick: [.antikick]";
                    comandos += "\n" + "Finalizar uma partida: [.end]";

                    administrador.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(comandos));
                    response = " [Help] Lista de comandos para todas as autoridades.";
                }
            }
        }

        private bool InGame(Account player)
        {
            Room room = player.room;
            if (room != null && room.GetSlot(player.slotId, out Slot slot) && slot.state >= SlotStateEnum.LOAD)
            {
                return true;
            }
            return false;
        }
    }
}
