using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_REQUEST_ACCEPT_REQ : GamePacketReader
    {
        private List<long> players = new List<long>();
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
                if (clan.id > 0 && (player.clanAuthority >= ClanAuthorityEnum.Master && player.clanAuthority <= ClanAuthorityEnum.Auxiliar || player.playerId == clan.ownerId))
                {
                    List<Account> clanPlayers = clan.GetPlayers(-1, true);
                    if (clanPlayers.Count >= clan.maxPlayers)
                    {
                        client.SendPacket(new CLAN_REQUEST_ACCEPT_PAK(-1));
                        return;
                    }
                    int result = 0;
                    for (int i = 0; i < players.Count; i++)
                    {
                        Account member = AccountManager.GetAccount(players[i], 0);
                        if (member != null && clanPlayers.Count < clan.maxPlayers && member.clanId == 0 && member.GetRequestClanId() > 0)
                        {
                            using (CLAN_MEMBER_INFO_INSERT_PAK packet = new CLAN_MEMBER_INFO_INSERT_PAK(member))
                            {
                                member.SendPacketForPlayers(packet, clanPlayers);
                            }
                            member.clanId = player.clanId;
                            member.clanDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                            member.clanAuthority = ClanAuthorityEnum.Membro;

                            member.ExecuteQuery($"UPDATE accounts SET clan_id='{clan.id}', clan_authority='{member.clanAuthority}', clan_date='{member.clanDate}', clan_id='{member.clanId}' WHERE id='{member.playerId}'");

                            member.DeleteInvite(player.clanId);
                            if (member.isOnline)
                            {
                                member.SendPacket(new CLAN_GET_CLAN_MEMBERS_PAK(clanPlayers));
                                Room room = member.room;
                                if (room != null)
                                {
                                    room.SendPacketToPlayers(new PROTOCOL_ROOM_GET_SLOTONEINFO_ACK(member, clan));
                                }
                                member.SendPacket(new CLAN_NEW_INFOS_PAK(clan, clanPlayers.Count + 1));
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
                                    noteEnum = NoteMessageClanEnum.InviteAccept
                                };
                                if (message != null && member.InsertMessage(message) && member.isOnline)
                                {
                                    //Envia mensagem para o jogador, notificando que foi aceito no clã.
                                    member.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                                }
                            }
                            result++;
                            clanPlayers.Add(member);
                        }
                    }
                    client.SendPacket(new CLAN_REQUEST_ACCEPT_PAK(result));
                    clanPlayers = null;
                }
                else
                {
                    client.SendPacket(new CLAN_REQUEST_ACCEPT_PAK(-1));
                }
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