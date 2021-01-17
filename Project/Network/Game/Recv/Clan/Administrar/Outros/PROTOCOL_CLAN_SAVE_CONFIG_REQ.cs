using System;

namespace PointBlank.Game
{
    /* (Administrar outros) Salvas as configurações do clã.
     * Autoridade de Editar Noticias/Informaçoes, Expulsar Membros, Convidar, Aceitar Cadastros
     * Limites de Rank, Idade para alistamentos.
     * 
     * Configurações de autoridade de staff.
     * autoridade: 8=Autoridade de administração de posts.
     * autoridade: 4=Autoridade de convidar ao clã.
     * autoridade: 2=Autoridade de expulsar membro.
     * autoridade: 1=Autoridade de administrar cadastros.
     * 
     * Configurar o limite de cadastro.
     * 
     * Ranks
     * rankId: 4 = + Sargento
     * rankId: 17 = + 2º Tenente 1
     * rankId: 31 = + Major 1
     * rankId: 46 = + General de Brigada
     * 
     * Idade Acima (limitAgeBigger)
     * Livre = 0
     * 15 Anos
     * 20 Anos
     * 30 Anos
     * 
     * 
     * Idade Abaixo (limitAgeSmaller)
     * Livre = 0
     * 15 Anos
     * 20 Anos
     * 30 Anos
     */
    public class PROTOCOL_CLAN_SAVE_CONFIG_REQ : GamePacketReader
    {
        private byte authorityConfig;
        private int limitRankId, limitAgeBigger, limitAgeSmaller;
        public override void ReadImplement()
        {
            authorityConfig = ReadByte();
            limitRankId = ReadByte();
            limitAgeBigger = ReadByte();
            limitAgeSmaller = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || authorityConfig > 15 || (limitRankId != 4 && limitRankId != 17 && limitRankId != 31 && limitRankId != 46) || (limitAgeBigger != 0 && limitAgeBigger != 15 && limitAgeBigger != 20 && limitAgeBigger != 30) || (limitAgeSmaller != 0 && limitAgeSmaller != 15 && limitAgeSmaller != 20 && limitAgeSmaller != 30))
                {
                    return;
                }
                Clan clan = ClanManager.GetClan(player.clanId);
                if (clan.id > 0 && (clan.ownerId == player.playerId) && clan.UpdateClanInfo(authorityConfig, limitRankId, limitAgeBigger, limitAgeSmaller))
                {
                    clan.authorityConfig = (ClanAuthorityConfigEnum)authorityConfig;
                    clan.limitRankId = limitRankId;
                    clan.limitAgeBigger = limitAgeBigger;
                    clan.limitAgeSmaller = limitAgeSmaller;
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_SAVEINFO3_SUCCESS_ACK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_SAVEINFO3_ERROR_ACK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}