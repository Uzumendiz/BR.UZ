using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_PROMOTE_MASTER_REQ : GamePacketReader
    {
        private long playerId;
        public override void ReadImplement()
        {
            playerId = ReadLong();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.clanAuthority != ClanAuthorityEnum.Master)
                {
                    client.SendPacket(new CLAN_COMMISSION_MASTER_PAK(0x80000000));
                    return;
                }
                Account member = AccountManager.GetAccount(playerId, 0);
                if (member == null || member.clanId != player.clanId)
                {
                    client.SendPacket(new CLAN_COMMISSION_MASTER_PAK(0x80000000));
                }
                else if (member.rankId >= Settings.MinRankMasterClan)
                {
                    Clan clan = ClanManager.GetClan(player.clanId);
                    if (player.clanId > 0 && clan.id > 0 && clan.ownerId == player.playerId && member.clanAuthority == ClanAuthorityEnum.Auxiliar && player.ExecuteQuery($"UPDATE clan_data SET owner_id='{playerId}' WHERE clan_id='{player.clanId}'") &&
                        member.ExecuteQuery($"UPDATE accounts SET clan_authority='1' WHERE id='{playerId}'") &&
                        player.ExecuteQuery($"UPDATE accounts SET clan_authority='2' WHERE id='{player.playerId}'"))
                    {
                        member.clanAuthority = ClanAuthorityEnum.Master;
                        player.clanAuthority = ClanAuthorityEnum.Auxiliar;
                        clan.ownerId = playerId;
                        if (member.GetMessagesCount() < 100)
                        {
                            Message message = new Message(15)
                            {
                                senderName = clan.name,
                                senderId = player.playerId,
                                clanId = clan.id,
                                type = 4,
                                state = 1,
                                noteEnum = NoteMessageClanEnum.Master
                            };
                            if (message != null && member.InsertMessage(message) && member.isOnline)
                            {
                                //Envia mensagem para o jogador promovido para master, notificando do seu novo cargo.
                                member.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                            }
                        }
                        if (member.isOnline)
                        {
                            member.SendCompletePacket(PackageDataManager.CLAN_PRIVILEGES_MASTER_PAK);
                        }
                        client.SendPacket(new CLAN_COMMISSION_MASTER_PAK(0));
                    }
                    else
                    {
                        client.SendPacket(new CLAN_COMMISSION_MASTER_PAK(2147487744));
                    }
                }
                else
                {
                    client.SendPacket(new CLAN_COMMISSION_MASTER_PAK(2147487928));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}