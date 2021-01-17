using System;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_GET_LOBBY_USER_LIST_REQ : GamePacketReader
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
                DateTime now = DateTime.Now;
                if (channel != null && (now - player.lastRoomGetLobbyPlayers).TotalSeconds >= 1)
                {
                    client.SendPacket(new PROTOCOL_ROOM_GET_LOBBY_USER_LIST_ACK(channel));
                    player.lastRoomGetLobbyPlayers = now;
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}