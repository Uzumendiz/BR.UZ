using System;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_INVITE_PLAYERS_REQ : GamePacketReader
    {
        private int count;
        public override void ReadImplement()
        {
            count = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                DateTime now = DateTime.Now;
                if (count > 0 && count <= 8 && room != null && (now - player.lastRoomInvitePlayers).TotalSeconds >= 2)
                {
                    Channel channel = player.GetChannel();
                    if (channel != null)
                    {
                        player.lastRoomInvitePlayers = now;
                        using (PROTOCOL_ROOM_INVITE_SHOW_ACK packet = new PROTOCOL_ROOM_INVITE_SHOW_ACK(player, room))
                        {
                            byte[] data = packet.GetCompleteBytes("ROOM_INVITE_PLAYERS_REQ");
                            for (int i = 0; i < count; i++)
                            {
                                try
                                {
                                    int sessionId = ReadInt();
                                    PlayerSession session = channel.GetPlayer(sessionId);
                                    if (session != null && session.playerId > 0)
                                    {
                                        Account playerLobby = AccountManager.GetAccount(session.playerId, true);
                                        if (playerLobby != null)
                                        {
                                            playerLobby.SendCompletePacket(data);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Exception(ex);
                                }
                            }
                        }
                    }
                    client.SendCompletePacket(PackageDataManager.ROOM_INVITE_RETURN_SUCCESS_PAK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.ROOM_INVITE_RETURN_ERROR_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}