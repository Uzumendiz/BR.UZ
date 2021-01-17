using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_CHANGE_INFO_REQ : GamePacketReader
    {
        private string informations;
        public override void ReadImplement()
        {
            informations = ReadString(ReadByte());
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
                if (player.clanId > 0 && clan.id > 0 && clan.informations != informations && informations.Length <= 241 && (clan.ownerId == player.playerId || player.clanAuthority >= ClanAuthorityEnum.Master && player.clanAuthority <= ClanAuthorityEnum.Auxiliar))
                {
                    if (player.ExecuteQuery($"UPDATE clan_data SET clan_info='{informations}' WHERE clan_id='{clan.id}'"))
                    {
                        clan.informations = informations;
                        client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_REPLACE_INTRO_SUCCESS_ACK);
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_REPLACE_INTRO_2147487860_ACK);
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_REPLACE_INTRO_2147487835_ACK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}