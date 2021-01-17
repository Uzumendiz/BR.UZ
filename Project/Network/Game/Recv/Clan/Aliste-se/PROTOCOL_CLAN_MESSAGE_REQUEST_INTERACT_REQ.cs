using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_MESSAGE_REQUEST_INTERACT_REQ : GamePacketReader
    {
        private int clanId, type;
        public override void ReadImplement()
        {
            clanId = ReadInt();
            ReadInt();
            type = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.nickname.Length == 0)
                {
                    return;
                }
                Clan clan = ClanManager.GetClan(clanId);
                if (clan.id == 0)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_REQUEST_ACCEPT_2147487835_PAK);
                    return;
                }
                if (player.clanId > 0)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_REQUEST_ACCEPT_2147487832_PAK);
                    return;
                }
                List<Account> clanPlayers = clan.GetPlayers(-1, true);
                if (clan.maxPlayers <= clanPlayers.Count)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_REQUEST_ACCEPT_2147487830_PAK);
                }
                else if (type == 0 || type == 1)
                {
                    Account owner = AccountManager.GetAccount(clan.ownerId, 0);
                    if (owner != null)
                    {
                        if (owner.GetMessagesCount() < 100)
                        {
                            Message message = new Message(15)
                            {
                                senderName = clan.name,
                                senderId = player.playerId,
                                clanId = clan.id,
                                type = 4,
                                text = player.nickname,
                                state = 1,
                                noteEnum = type == 0 ? NoteMessageClanEnum.JoinDenial : NoteMessageClanEnum.JoinAccept
                            };
                            if (message != null && owner.InsertMessage(message) && owner.isOnline)
                            {
                                owner.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                            }
                        }
                        if (type == 1)
                        {
                            int date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                            if (player.ExecuteQuery($"UPDATE accounts SET clan_id='{clan.id}', clan_authority='3', clan_date='{date}' WHERE id='{player.playerId}'"))
                            {
                                using (CLAN_MEMBER_INFO_INSERT_PAK packet = new CLAN_MEMBER_INFO_INSERT_PAK(player))
                                {
                                    player.SendPacketForPlayers(packet, clanPlayers);
                                }
                                player.clanId = clan.id;
                                player.clanDate = date;
                                player.clanAuthority = ClanAuthorityEnum.Membro;
                                client.SendPacket(new CLAN_GET_CLAN_MEMBERS_PAK(clanPlayers));
                                Room room = player.room;
                                if (room != null)
                                {
                                    room.SendPacketToPlayers(new PROTOCOL_ROOM_GET_SLOTONEINFO_ACK(player, clan));
                                }
                                client.SendPacket(new CLAN_NEW_INFOS_PAK(clan, owner, clanPlayers.Count + 1));
                            }
                            else
                            {
                                client.SendCompletePacket(PackageDataManager.BOX_MESSAGE_SEND_ERROR_0x80000000_PAK);
                            }
                        }
                        client.SendCompletePacket(PackageDataManager.BOX_MESSAGE_SEND_SUCCESS_PAK);
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.BOX_MESSAGE_SEND_ERROR_0x80000000_PAK);
                    }
                }
                clanPlayers = null;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}