using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_DELETE_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
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
                if (clan.id > 0 && clan.ownerId == player.playerId && player.clanAuthority == ClanAuthorityEnum.Master && player.ExecuteQuery($"DELETE FROM clan_data WHERE clan_id='{clan.id}'") && player.ExecuteQuery($"UPDATE accounts SET clan_id='0', clan_authority='0', clan_fights='0', clan_wins='0', clan_date='0' WHERE id='{player.playerId}'") && ClanManager.RemoveClan(clan))
                {
                    player.clanId = 0;
                    player.clanAuthority = ClanAuthorityEnum.None;
                    ClanManager.RemoveClan(clan);
                    client.SendCompletePacket(PackageDataManager.CLAN_CLOSE_SUCCESS_PAK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CLOSE_2147487850_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}