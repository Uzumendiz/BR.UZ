using System;

namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_LEAVE_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Channel channel = player != null ? player.GetChannel() : null;
                if (player.room != null || player.match != null)
                {
                    return;
                }
                if (channel == null || player.session == null || !channel.RemovePlayer(player))
                {
                    client.SendCompletePacket(PackageDataManager.LOBBY_LEAVE_ERROR_PAK);
                    client.Close(1000);
                    return;
                }
                client.SendCompletePacket(PackageDataManager.LOBBY_LEAVE_SUCCESS_PAK);
                player.ResetPages();
                player.status.UpdateChannel(255);
                player.SyncPlayerToFriends(false);
                player.SyncPlayerToClanMembers();
            }
            catch (Exception ex)
            {
                client.SendCompletePacket(PackageDataManager.LOBBY_LEAVE_ERROR_PAK);
                PacketLog(ex);
            }
        }
    }
}