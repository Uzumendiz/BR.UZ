using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_REQUEST_DENIAL_REQ : GamePacketReader
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
                int result = 0;
                Clan clan = ClanManager.GetClan(player.clanId);
                if (clan.id > 0 && (player.clanAuthority >= ClanAuthorityEnum.Master && player.clanAuthority <= ClanAuthorityEnum.Auxiliar || clan.ownerId == player.playerId))
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        Account member = AccountManager.GetAccount(players[i], 0);
                        if (member != null)
                        {
                            if (member.DeleteInvite(clan.id))
                            {
                                if (member.GetMessagesCount() < 100)
                                {
                                    Message message = new Message(15)
                                    {
                                        senderName = clan.name,
                                        senderId = player.playerId,
                                        clanId = clan.id,
                                        type = 4,
                                        state = 1,
                                        noteEnum = NoteMessageClanEnum.InviteDenial
                                    };
                                    if (message != null && member.InsertMessage(message) && member.isOnline)
                                    {
                                        //Envia mensagem para o jogador, notificando que foi rejeitado o alistamento.
                                        member.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(message));
                                    }
                                }
                                result++;
                            }
                        }
                    }
                }
                client.SendPacket(new CLAN_REQUEST_DENIAL_PAK(result));
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