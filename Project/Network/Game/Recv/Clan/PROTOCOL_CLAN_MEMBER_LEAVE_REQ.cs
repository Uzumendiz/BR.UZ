using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    /*
     * Ação: requisitado quando um membro abandona o clã.
     */
    public class PROTOCOL_CLAN_MEMBER_LEAVE_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.clanId <= 0)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_MEMBER_LEAVE_2147487835_ACK);
                    return;
                }
                Clan clan = ClanManager.GetClan(player.clanId);
                if (clan.id == 0 || clan.ownerId == player.playerId)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_MEMBER_LEAVE_2147487838_ACK);
                    return;
                }
                if (player.ExecuteQuery($"UPDATE accounts SET clan_id='0', clan_authority='0', clan_fights='0', clan_wins='0', clan_date='0' WHERE id='{player.playerId}'"))
                {
                    List<Account> players = player.GetClanPlayers(player.playerId);
                    using (PROTOCOL_CLAN_MEMBER_LEAVE_ACK packet = new PROTOCOL_CLAN_MEMBER_LEAVE_ACK(player.playerId))
                    {
                        player.SendPacketForPlayers(packet, players);
                    }
                    players = null;
                    Account ownerClan = AccountManager.GetAccount(clan.ownerId, 0);
                    if (ownerClan != null)
                    {
                        if (ownerClan.GetMessagesCount() < 100)
                        {
                            Message message = new Message(15)
                            {
                                senderName = clan.name,
                                senderId = player.playerId,
                                clanId = clan.id,
                                type = 4,
                                text = player.nickname,
                                state = 1,
                                noteEnum = NoteMessageClanEnum.Secession
                            };
                            if (message != null && ownerClan.InsertMessage(message) && ownerClan.isOnline)
                            {
                                //Envia mensagem para o dono do clã, notificando que o jogador saiu do clã.
                                ownerClan.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                            }
                        }
                    }
                    player.clanId = 0;
                    player.clanAuthority = ClanAuthorityEnum.None;
                    client.SendCompletePacket(PackageDataManager.CLAN_MEMBER_LEAVE_SUCCESS_ACK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_MEMBER_LEAVE_0x8000106B_ACK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}