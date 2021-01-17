using System;

namespace PointBlank.Auth
{
    /*
     * =========================================
     * Blood: SANGUE E ACERTO DE DISPARO 
     * =========================================
     * 0=None
     * 1=Exibir sangue
     * 2=Exibir Acerto de disparo
     * 3=Exibir Sangue + Acerto de disparo
     * =========================================
     * 
     * Sight: MIRA Tipos de mira em ordem da esqueda para direita (0,1,2,3)
     * 
     * =========================================
     * config: INTERFACE
     * =========================================
     * 0=None + Mini-mapa: rotação por padrão
     * 1=Indicar inimigos na mira 
     * 2=Indicar aliados na mira
     * 4=Efeito de texto
     * 8=Mini-mapa: Fixed
     * 16=Janela de aviso de missão
     * 32=Função de histórico de 1vs1
     * =========================================
     * 
     * AudioEnable: 0=Nenhum 7=Todos 1=Musicas 2=Efeitos Sonoros 4=Mensagem de rádio
     * 
     * Inverter Mouse: 0=None 1=Actived
     * Sensibilidade Max: 120 Min: 10
     * Angulo de Visão: (fov) Max: 80 Min: 35
     * 
     * ChatPrivate
     * 0=Permitir Todos
     * 1=Sussuro=Bloquear Todos
     * 16=Chat Bloquear Todos 
     * 
     * MessageInvitation 
     * 0= Permitir Todos
     * 16=Bloquear todos 
     * 32=Permitir amigos e clã
     * 
     * Macro
     * 0=ALT 1-5 (Todos) 31(TIME)
     * 1=ALT1 TIME
     * 2=ALT2 TIME
     * 4=ALT3 TIME
     * 8=ALT4 TIME
     * 16=ALT6 TIME
     */
    public class PROTOCOL_BASE_CONFIG_SAVE_REQ : AuthPacketReader
    {
        private int type;
        private PlayerConfig configs = new PlayerConfig();
        public override void ReadImplement()
        {
            try
            {
                type = ReadInt();
                if ((type & 1) == 1)
                {
                    configs.blood = ReadShort(); //(SANGUE E ACERTO DE DISPARO) 0=Não exibir sangue e Não exibir acerto de disparo 1=Exibir sangue 2=Exibir Acerto de disparo 3=Exibir sangue e Acerto de disparo
                    configs.sight = ReadByte(); //(MIRA) Tipos de mira em ordem da esqueda pra direita (0,1,2,3)
                    configs.hand = ReadByte(); //(MÃO) Lado da mão utilizada: 0=Destro 1=Canhoto
                    configs.config = ReadInt(); //(INTERFACE) 0=Nenhum Selecionado e Rotação do mini-mapa 1=Indicar inimigos na mira 2=Indicar aliados na mira
                    configs.audioEnable = ReadByte();
                    ReadB(5);
                    configs.audio = ReadByte(); //Efeitos Sonoros
                    configs.music = ReadByte(); //Musicas
                    configs.fov = ReadShort();
                    configs.sensibilidade = ReadByte();
                    configs.invertedMouse = ReadByte();
                    ReadByte();
                    ReadByte();
                    configs.messageInvitation = ReadByte();
                    configs.chatPrivate = ReadByte();
                    configs.macros = ReadByte();
                    ReadByte();
                    ReadByte();
                    ReadByte();
                }
                if ((type & 2) == 2)
                {
                    ReadB(5);
                    byte[] keysBuffer = ReadB(215);
                    configs.keys = keysBuffer;
                }
                if ((type & 4) == 4)
                {
                    configs.macro_1 = ReadString(ReadByte());
                    configs.macro_2 = ReadString(ReadByte());
                    configs.macro_3 = ReadString(ReadByte());
                    configs.macro_4 = ReadString(ReadByte());
                    configs.macro_5 = ReadString(ReadByte());
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || (player.configs == null && !player.InsertConfig()) || player.checkUserConfigsSave)
                {
                    return;
                }
                if (configs.blood > 3 || configs.sight > 3 || configs.hand > 1 || configs.config > 63 || configs.audioEnable > 7 || configs.audio > 100 || configs.music > 100 || configs.fov < 35 || configs.fov > 80 || configs.sensibilidade < 10 || configs.sensibilidade > 120 || configs.invertedMouse > 1 || (configs.messageInvitation != 0 && configs.messageInvitation != 16 && configs.messageInvitation != 32) || (configs.chatPrivate != 0 && configs.chatPrivate != 1 && configs.chatPrivate != 16 && configs.chatPrivate != 17) || configs.macros > 31 || configs.macro_1.Length > 58 || configs.macro_2.Length > 58 || configs.macro_3.Length > 58 || configs.macro_4.Length > 58 || configs.macro_5.Length > 58)
                {
                    Logger.Warning($" [AUTH] [{GetType().Name}] Dados das configurações recebidas estão incorretos. PlayerId: {player.playerId}");
                    return;
                }
                using (DBQuery query = new DBQuery())
                {
                    if ((type & 1) == 1)
                    {
                        player.UpdateConfigs(query, configs);
                    }
                    if ((type & 2) == 2)
                    {
                        query.AddQuery("keys", configs.keys);
                    }
                    if ((type & 4) == 4)
                    {
                        player.UpdateMacros(query, configs, type);
                    }
                    Utilities.UpdateDB("player_configs", "owner_id", player.playerId, query.GetTables(), query.GetValues());
                }
                player.configs = configs;
                player.checkUserConfigsSave = true;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}