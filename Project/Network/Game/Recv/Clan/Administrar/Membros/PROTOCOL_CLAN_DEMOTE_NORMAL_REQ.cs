using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_DEMOTE_NORMAL_REQ : GamePacketReader
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
                    client.SendCompletePacket(PackageDataManager.CLAN_COMMISSION_REGULAR_2147487833_PAK);
                    return;
                }
                for (int i = 0; i < players.Count; i++)
                {
                    Account member = AccountManager.GetAccount(players[i], 0);
                    if (member != null && member.clanId == clan.id && member.clanAuthority == ClanAuthorityEnum.Auxiliar && member.ExecuteQuery($"UPDATE accounts SET clan_authority='3' WHERE id='{member.playerId}'"))
                    {
                        member.clanAuthority = ClanAuthorityEnum.Membro;
                        if (member.GetMessagesCount() < 100)
                        {
                            Message message = new Message(15)
                            {
                                senderName = clan.name,
                                senderId = player.playerId,
                                clanId = clan.id,
                                type = 4,
                                state = 1,
                                noteEnum = NoteMessageClanEnum.Regular
                            };
                            if (message != null && member.InsertMessage(message) && member.isOnline)
                            {
                                //Envia mensagem para o jogador que foi demitido do seu cargo, notificando a demissão.
                                member.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                            }
                        }
                        if (member.isOnline)
                        {
                            member.SendCompletePacket(PackageDataManager.CLAN_PRIVILEGES_DEMOTE_PAK);
                        }
                        result++;
                    }
                }
                client.SendPacket(new CLAN_COMMISSION_REGULAR_PAK(result));
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