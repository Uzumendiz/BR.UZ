using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_DEMOTE_KICK_REQ : GamePacketReader
    {
        private List<long> players = new List<long>();
        private uint result;
        public override void ReadImplement()
        {
            try
            {
                int countPlayers = ReadByte();
                for (int i = 0; i < countPlayers; i++)
                {
                    long playerId = ReadLong();
                    players.Add(playerId);
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
                if (player == null)
                {
                    return;
                }
                Clan clan = ClanManager.GetClan(player.clanId);
                if (player.clanId == 0 || clan.id == 0 || !(player.clanAuthority >= ClanAuthorityEnum.Master && player.clanAuthority <= ClanAuthorityEnum.Auxiliar || clan.ownerId == player.playerId))
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_DEPORTATION_2147487833_PAK);
                    return;
                }
                List<Account> clanPlayers = clan.GetPlayers(-1, true);
                for (int i = 0; i < players.Count; i++)
                {
                    Account member = AccountManager.GetAccount(players[i], 0);
                    if (member != null && member.clanId == clan.id && member.match == null && member.ExecuteQuery($"UPDATE accounts SET clan_id='0', clan_authority='0', clan_fights='0', clan_wins='0' WHERE id='{member.playerId}'"))
                    {
                        using (PROTOCOL_CLAN_MEMBER_LEAVE_ACK packet = new PROTOCOL_CLAN_MEMBER_LEAVE_ACK(member.playerId))
                        {
                            member.SendPacketForPlayers(packet, clanPlayers, member.playerId);
                        }
                        member.clanId = 0;
                        member.clanAuthority = ClanAuthorityEnum.None;
                        lock (member.clanPlayers)
                        {
                            member.clanPlayers.Clear();
                        }
                        if (member.GetMessagesCount() < 100)
                        {
                            Message message = new Message(15)
                            {
                                senderName = clan.name,
                                senderId = player.playerId,
                                clanId = clan.id,
                                type = 4,
                                state = 1,
                                noteEnum = NoteMessageClanEnum.Deportation
                            };
                            if (message != null && member.InsertMessage(message) && member.isOnline)
                            {
                                //Envia mensagem para o jogador que foi expulso do clã, notificando a expulsão.
                                member.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                            }
                        }
                        if (member.isOnline)
                        {
                            member.SendCompletePacket(PackageDataManager.CLAN_PRIVILEGES_KICK_PAK);
                        }
                        result++;
                        clanPlayers.Remove(member);
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.CLAN_DEPORTATION_2147487833_PAK);
                        break;
                    }
                }
                client.SendPacket(new CLAN_DEPORTATION_PAK(result));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
            finally
            {
                players = null;
            }
        }
    }
}