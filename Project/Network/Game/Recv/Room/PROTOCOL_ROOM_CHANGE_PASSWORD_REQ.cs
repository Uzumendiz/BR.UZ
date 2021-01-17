using System;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CHANGE_PASSWORD_REQ : GamePacketReader
    {
        private string password;
        public override void ReadImplement()
        {
            password = ReadString(4);
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room != null && room.leaderSlot == player.slotId && room.password != password)
                {
                    room.password = password;
                    using (PROTOCOL_ROOM_CHANGE_PASSWORD_ACK packet = new PROTOCOL_ROOM_CHANGE_PASSWORD_ACK(password))
                    {
                        room.SendPacketToPlayers(packet);
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