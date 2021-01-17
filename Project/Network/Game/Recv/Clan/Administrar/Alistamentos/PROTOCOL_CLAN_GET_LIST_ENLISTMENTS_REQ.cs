using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    /* Requer a lista de jogadores que se alistaram para ser membros do clã.
     * Enviar por paginas a lista de jogadores e suas informações.
     */
    public class PROTOCOL_CLAN_GET_LIST_ENLISTMENTS_REQ : GamePacketReader
    {
        private int page;
        public override void ReadImplement()
        {
            page = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.clanId <= 0)
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_LIST_ENLISTMENTS_ERROR_ACK);
                    return;
                }
                List<ClanInvite> clanInvites = player.GetClanRequestEnlistment();
                using (PacketWriter writer = new PacketWriter())
                {
                    int count = 0;
                    for (int i = page * 13; i < clanInvites.Count; i++)
                    {
                        ClanInvite invite = clanInvites[i];
                        writer.WriteQ(invite.playerId);
                        Account playerInvited = AccountManager.GetAccount(invite.playerId, 0);
                        if (playerInvited != null)
                        {
                            writer.WriteS(playerInvited.nickname, 33);
                            writer.WriteC(playerInvited.rankId);
                        }
                        else
                        {
                            writer.WriteB(new byte[34]);
                        }
                        writer.WriteD(invite.inviteDate);
                        if (count++ == 13)
                        {
                            break;
                        }
                    }
                    client.SendPacket(new PROTOCOL_CLAN_LIST_ENLISTMENTS_ACK(0, count, page, writer.memorystream.ToArray()));
                }
                clanInvites = null;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}