using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_CHANNEL_ENTER_REQ : GamePacketReader
    {
        private int channelId;
        public override void ReadImplement()
        {
            channelId = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.channelId >= 0)
                {
                    return;
                }
                Channel channel = ServersManager.GetChannel(channelId);
                if (channel != null)
                {
                    if (ChannelRequirementCheck(player, channel))
                    {
                        client.SendCompletePacket(PackageDataManager.BASE_CHANNEL_ENTER_0x80000202_PAK);
                    }
                    else if (channel.players.Count >= Settings.MaxPlayersChannel)
                    {
                        client.SendCompletePacket(PackageDataManager.BASE_CHANNEL_ENTER_0x80000201_PAK);
                    }
                    else
                    {
                        player.channelId = channelId;
                        client.SendPacket(new BASE_CHANNEL_ENTER_PAK(player.channelId, channel.announce));
                        player.status.UpdateChannel((byte)player.channelId);
                        player.UpdateCacheInfo();
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BASE_CHANNEL_ENTER_0x80000000_PAK);
                    client.Close(3000);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
            /*
             * 0x80000201 STBL_IDX_EP_SERVER_USER_FULL_C
             * 0x80000202 - De acordo com o ChannelType
             * 0x80000203 STBL_IDX_EP_SERVER_NOT_SATISFY_MTS
             * 0x80000204 STR_UI_GOTO_GWARNET_CHANNEL_ERROR
             * 0x80000205 STR_UI_GOTO_AZERBAIJAN_CHANNEL_ERROR
             * 0x80000206 STR_POPUP_MOBILE_CERTIFICATION_ERROR
             * 0x80000207 STR_UI_GOTO_TURKISH_CHANNEL_ERROR
             * 0x80000208 STR_UI_GOTO_MENA_CHANNEL_ERROR
             */
        }

        private bool ChannelRequirementCheck(Account player, Channel channel)
        {
            if (player.IsGM() || player.HaveAcessLevel())
            {
                return false;
            }
            if (channel.type == 4 && player.clanId == 0) //Canal de clã | Precisa de clã (Menos GM)
            {
                return true;
            }
            else if (channel.type == 3 && player.statistics.GetKDRatio() > 40) //Canal iniciante1 | KD abaixo de 40%
            {
                return true;
            }
            else if (channel.type == 2 && player.rankId >= 4) //Canal iniciante2 | Entre Novato-Cabo
            {
                return true;
            }
            else if (channel.type == 5 && player.rankId <= 25) //Canal avançado | Entre capitão 1-hero
            {
                return true;
            }
            else if (channel.type == -1 || channel.type == 9) //Canal Bloqueado
            {
                return true;
            }
            return false;
        }
    }
}