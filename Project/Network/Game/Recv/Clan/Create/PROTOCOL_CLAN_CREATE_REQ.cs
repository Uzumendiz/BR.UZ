using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_CREATE_REQ : GamePacketReader
    {
        private int NameLength, InfoLength, AzitLength;
        private string clanName, clanInfo, clanAzit;
        public override void ReadImplement()
        {
            NameLength = ReadByte();
            InfoLength = ReadByte();
            AzitLength = ReadByte();
            clanName = ReadString(NameLength);
            clanInfo = ReadString(InfoLength);
            clanAzit = ReadString(AzitLength);
            ReadInt();
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
                Clan clan = new Clan
                {
                    name = clanName,
                    informations = clanInfo,
                    logo = 0,
                    ownerId = player.playerId,
                    creationDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"))
                };
                if (player.clanId > 0 || player.GetRequestClanId() > 0)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CREATE_0x8000105C_PAK);
                }
                else if ((player.gold - Settings.ClanCreateGold) < 0 || Settings.ClanCreateRank > player.rankId)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CREATE_0x8000104A_PAK);
                }
                else if (ClanManager.clans.Count > Settings.MaxClanActive)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CREATE_0x80001055_PAK);
                }
                if ((player.gold - Settings.ClanCreateGold) < 0)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CREATE_0x80001048_PAK);
                }
                else if (!ClanManager.IsClanNameExist(clan.name).Result && clan.CreateClan().Result && player.UpdateAccountGold(player.gold - Settings.ClanCreateGold) && player.ExecuteQuery($"UPDATE accounts SET clan_authority='1', clan_date='{clan.creationDate}', clan_id='{clan.id}' WHERE id='{player.playerId}'"))
                {
                    clan.BestPlayers.SetDefault();
                    player.clanDate = clan.creationDate;
                    player.clanId = clan.id;
                    player.clanAuthority = ClanAuthorityEnum.Master;
                    ClanManager.AddClan(clan);
                    player.gold -= Settings.ClanCreateGold;
                    client.SendPacket(new CLAN_CREATE_PAK(0, clan, player));
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CREATE_0x8000105A_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
            /*
             * 80001055 - STBL_IDX_CLAN_MESSAGE_FAIL_CREATING_OVERFLOW
             * 8000105C - STBL_IDX_CLAN_MESSAGE_FAIL_CREATING_ALREADY
             * 8000105A - STBL_IDX_CLAN_MESSAGE_FAIL_CREATING_OVERLAPPING
             * 80001048 - STBL_IDX_CLAN_MESSAGE_FAIL_CREATING
             * Padrão: STBL_IDX_CLAN_MESSAGE_FAIL_CREATING_ADMIN
             */
        }
    }
}