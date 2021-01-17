using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_MESSAGE_INVITE_REQ : GamePacketReader
    {
        private int type;
        private object objectValue;
        public override void ReadImplement()
        {
            type = ReadByte(); //0 - AMIGOS (Q) || 1 - SALA (D) || 2 - LOBBY (D)
            if (type == 0)
            {
                objectValue = ReadLong();
            }
            else if (type == 1)
            {
                objectValue = ReadInt();
            }
            else if (type == 2)
            {
                objectValue = ReadInt();
            }
            else
            {
                int value = ReadInt();
                Logger.Warning($" [PROTOCOL_CLAN_MESSAGE_INVITE_REQ] Type: {type} Value: {value}");
            }
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                DateTime now = DateTime.Now;
                if (player == null || player.clanId <= 0 || (now - player.lastClanInvite).TotalSeconds < 1)
                {
                    return;
                }
                if (type == 0)
                {
                    long playerId = (long)objectValue;
                    Account playerInvited = AccountManager.GetAccount(playerId, -1);
                    if (playerInvited == null)
                    {
                        client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_0x80000000_PAK);
                        return;
                    }
                    if (playerInvited.GetMessagesCount() >= 100)
                    {
                        client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_0x80000000_PAK);
                        return;
                    }
                    Clan clan = ClanManager.GetClan(player.clanId);
                    Message message = new Message(15)
                    {
                        senderName = clan.name,
                        clanId = clan.id,
                        senderId = player.playerId,
                        type = 5,
                        state = 1,
                        noteEnum = NoteMessageClanEnum.Invite
                    };
                    if (message != null && playerInvited.InsertMessage(message))
                    {
                        playerInvited.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                    }
                    client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_SUCCESS_PAK);
                }
                else if (type == 1)
                {
                    Room room = player.room;
                    if (room == null)
                    {
                        client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_0x80000000_PAK);
                        return;
                    }
                    int slotId = (int)objectValue;
                    Account playerInvited = room.GetPlayerBySlot(slotId);
                    if (playerInvited == null || !playerInvited.isOnline)
                    {
                        client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_0x80000000_PAK);
                        return;
                    }
                    if (playerInvited.GetMessagesCount() >= 100)
                    {
                        client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_0x80000000_PAK);
                        return;
                    }
                    Clan clan = ClanManager.GetClan(player.clanId);
                    Message message = new Message(15)
                    {
                        senderName = clan.name,
                        clanId = clan.id,
                        senderId = player.playerId,
                        type = 5,
                        state = 1,
                        noteEnum = NoteMessageClanEnum.Invite
                    };
                    if (message != null && playerInvited.InsertMessage(message))
                    {
                        playerInvited.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                    }
                    client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_SUCCESS_PAK);
                }
                else if (type == 2)
                {
                    Channel channel = player.GetChannel();
                    if (channel == null)
                    {
                        client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_0x80000000_PAK);
                        return;
                    }
                    int sessionId = (int)objectValue;
                    PlayerSession ps = channel.GetPlayer(sessionId);
                    long pId = ps != null ? ps.playerId : -1;
                    if (pId != -1 && pId != player.playerId)
                    {
                        Account playerInvited = AccountManager.GetAccount(pId, true);
                        if (playerInvited == null || !playerInvited.isOnline)
                        {
                            client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_0x80000000_PAK);
                            return;
                        }
                        if (playerInvited.GetMessagesCount() >= 100)
                        {
                            client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_0x80000000_PAK);
                            return;
                        }
                        Clan clan = ClanManager.GetClan(player.clanId);
                        Message message = new Message(15)
                        {
                            senderName = clan.name,
                            clanId = clan.id,
                            senderId = player.playerId,
                            type = 5,
                            state = 1,
                            noteEnum = NoteMessageClanEnum.Invite
                        };
                        if (message != null && playerInvited.InsertMessage(message))
                        {
                            playerInvited.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                        }
                        client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_SUCCESS_PAK);
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.CLAN_MESSAGE_INVITE_0x80000000_PAK);
                    }
                }
                player.lastClanInvite = now;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}