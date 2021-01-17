using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_REQUEST_INFO_REQ : GamePacketReader
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
                if (player != null)
                {
                    Account playerInfo = AccountManager.GetAccount(playerId, 0);
                    string text = player.GetRequestText(playerId);
                    if (playerInfo != null && text != null)
                    {
                        client.SendPacket(new PROTOCOL_CLAN_REQUEST_INFO_ACK(playerInfo, text, 0));
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.PROTOCOL_CLAN_REQUEST_INFO_ERROR_ACK);
                    }
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}