using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_MSG_FOR_PLAYERS_REQ : GamePacketReader
    {
        private int type;
        private string text;
        public override void ReadImplement()
        {
            type = ReadByte(); 
            text = ReadString(ReadByte());
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (text.Length > 120 || player == null)
                {
                    return;
                }
                Clan clan = ClanManager.GetClan(player.clanId);
                int playersLoaded = 0;
                if (clan.id > 0 && clan.ownerId == player.playerId)
                {
                    List<Account> players = clan.GetPlayers(player.playerId, true);
                    for (int i = 0; i < players.Count; i++)
                    {
                        Account member = players[i];
                        if ((type == 0 || member.clanAuthority == ClanAuthorityEnum.Master && type == 1 || member.clanAuthority == ClanAuthorityEnum.Auxiliar && type == 2) && member.GetMessagesCount() < 100)
                        {
                            playersLoaded++;
                            Message message = new Message(15)
                            {
                                senderName = clan.name,
                                senderId = player.playerId,
                                clanId = clan.id,
                                type = 4,
                                text = text,
                                state = 1
                            };
                            if (message != null && member.InsertMessage(message) && member.isOnline)
                            {
                                member.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                            }
                        }
                    }
                }
                client.SendPacket(new CLAN_MSG_FOR_PLAYERS_PAK(playersLoaded));
                if (playersLoaded > 0)
                {
                    client.SendCompletePacket(PackageDataManager.BOX_MESSAGE_SEND_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}