using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_CHAT_1390_REQ : GamePacketReader
    {
        private ChattingTypeEnum type;
        private string text;
        public override void ReadImplement()
        {
            type = (ChattingTypeEnum)ReadShort();
            text = ReadString(ReadShort());
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                DateTime now = DateTime.Now;
                if (player == null || player.clanId <= 0 || type != ChattingTypeEnum.Clan_Member_Page || text.Length > 60 || player.nickname.Length < Settings.NickMinLength || player.nickname.Length > Settings.NickMaxLength || (now - player.lastChatting).TotalSeconds < 1)
                {
                    return;
                }
                if (StringFilter.CheckFilterChat(text))
                {
                    client.SendPacket(new CLAN_CHAT_1390_PAK("Server", true, "Não é possivel digitar palavras inapropriadas."));
                    return;
                }
                Clan clan = ClanManager.GetClan(player.clanId);
                if (clan.id == 0)
                {
                    return;
                }
                List<Account> players = player.GetClanPlayers(-1);
                using (CLAN_CHAT_1390_PAK packet = new CLAN_CHAT_1390_PAK(player.nickname, player.UseChatGM(), text))
                {
                    player.SendPacketForPlayers(packet, players);
                }
                player.lastChatting = now;
                players = null;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}