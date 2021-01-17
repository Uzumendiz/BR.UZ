using System;

namespace PointBlank.Game
{
    public class PROTOCOL_GM_LOG_ROOM_REQ : GamePacketReader
    {
        private int slot;
        public override void ReadImplement()
        {
            slot = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account p = client.SessionPlayer;
                if (p == null || !p.IsGM())
                {
                    return;
                }
                Room room = p.room;
                if (room != null && room.GetPlayerBySlot(slot, out Account pR))
                {
                    client.SendPacket(new GM_LOG_ROOM_PAK(pR));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}