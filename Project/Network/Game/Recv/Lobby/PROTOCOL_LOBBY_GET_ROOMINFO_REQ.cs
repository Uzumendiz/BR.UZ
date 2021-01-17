using System;

namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_GET_ROOMINFO_REQ : GamePacketReader
    {
        private int roomId;
        public override void ReadImplement()
        {
            roomId = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Channel channel = player != null ? player.GetChannel() : null;
                if (channel != null)
                {
                    Room room = channel.GetRoom(roomId);
                    if (room != null)
                    {
                        client.SendPacket(new LOBBY_GET_ROOMINFO_PAK(room));
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