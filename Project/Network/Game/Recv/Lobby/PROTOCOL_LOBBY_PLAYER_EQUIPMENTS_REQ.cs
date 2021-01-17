using System;

namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_PLAYER_EQUIPMENTS_REQ : GamePacketReader
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
                long playerId = 0;
                try
                {
                    playerId = player.GetChannel().GetPlayer(sessionId).playerId;
                }
                catch
                {
                }
                Account playerInfo = AccountManager.GetAccount(playerId, true);
                if (playerInfo != null && playerInfo.equipments != null)
                {
                    client.SendPacket(new PROTOCOL_LOBBY_PLAYER_EQUIPMENTS_ACK(playerInfo.equipments));
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.LOBBY_GET_PLAYERINFO2_NULL_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}