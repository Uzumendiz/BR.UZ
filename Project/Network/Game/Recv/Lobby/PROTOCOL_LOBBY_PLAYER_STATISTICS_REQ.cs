using System;

namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_PLAYER_STATISTICS_REQ : GamePacketReader
    {
        private int sessionId;
        public override void ReadImplement()
        {
            sessionId = ReadInt();
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
                Account playerInfo = null;
                try
                {
                    playerInfo = AccountManager.GetAccount(player.GetChannel().GetPlayer(sessionId).playerId, true);
                }
                catch
                {
                }
                if (playerInfo != null && playerInfo.statistics != null)
                {
                    client.SendPacket(new PROTOCOL_LOBBY_PLAYER_STATISTICS_ACK(playerInfo.statistics));
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.LOBBY_GET_PLAYERINFO_NULL_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}