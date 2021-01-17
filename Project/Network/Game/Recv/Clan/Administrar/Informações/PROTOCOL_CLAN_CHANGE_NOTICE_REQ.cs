using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_CHANGE_NOTICE_REQ : GamePacketReader
    {
        private string notice;
        public override void ReadImplement()
        {
            notice = ReadString(ReadByte());
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null)
                {
                    return;
                }
                Clan clan = ClanManager.GetClan(player.clanId);
                if (player.clanId > 0 && clan.id > 0 && clan.notice != notice && notice.Length <= 241 && (clan.ownerId == player.playerId || player.clanAuthority >= ClanAuthorityEnum.Master && player.clanAuthority <= ClanAuthorityEnum.Auxiliar))
                {
                    if (player.ExecuteQuery($"UPDATE clan_data SET clan_news='{notice}' WHERE clan_id='{clan.id}'"))
                    {
                        clan.notice = notice;
                        client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_REPLACE_NOTICE_SUCCESS_ACK);
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_REPLACE_NOTICE_2147487859_ACK);
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_REPLACE_NOTICE_2147487835_ACK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}