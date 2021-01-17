using System;

namespace PointBlank.Game
{
    public class PROTOCOL_A_3428_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room != null && room.state >= RoomStateEnum.Loading && room.slots[player.slotId].state == SlotStateEnum.NORMAL)
                {
                    client.SendPacket(new A_3428_PAK(room));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}