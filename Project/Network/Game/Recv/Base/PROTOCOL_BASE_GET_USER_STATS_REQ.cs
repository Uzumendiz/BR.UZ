using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_GET_USER_STATS_REQ : GamePacketReader
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
                if (player == null)
                {
                    return;
                }
                Account playerInfos = AccountManager.GetAccount(playerId, 0);
                if (playerInfos != null)
                {
                    client.SendPacket(new BASE_GET_USER_STATS_PAK(playerInfos.statistics));
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BASE_GET_USER_STATS_NULL_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}