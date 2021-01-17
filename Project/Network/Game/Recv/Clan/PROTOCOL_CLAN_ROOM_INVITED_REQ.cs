using System;

namespace PointBlank.Game
{
    /*
     * Ação: Requisita um convite para uma sala pela lista de membros do clã.
     */
    public class PROTOCOL_CLAN_ROOM_INVITED_REQ : GamePacketReader
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
                if (player == null || player.clanId <= 0)
                {
                    return;
                }
                Account member = AccountManager.GetAccount(playerId, true);
                if (member != null && member.clanId == player.clanId && member.isOnline)
                {
                    member.SendPacket(new PROTOCOL_CLAN_ROOM_INVITE_RESULT_ACK(player.playerId));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}