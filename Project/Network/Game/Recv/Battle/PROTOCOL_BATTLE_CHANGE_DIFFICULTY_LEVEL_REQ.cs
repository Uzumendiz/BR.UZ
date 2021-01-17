using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_REQ : GamePacketReader
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
                if (room == null || room.state != RoomStateEnum.Battle || room.IngameAiLevel >= 10)
                {
                    return;
                }
                Slot slot = room.GetSlot(player.slotId);
                if (slot == null || slot.state != SlotStateEnum.BATTLE)
                {
                    return;
                }
                if (room.IngameAiLevel <= 9)
                {
                    room.IngameAiLevel++;
                }
                using (PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_ACK packet = new PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_ACK(room))
                {
                    room.SendPacketToPlayers(packet, SlotStateEnum.READY, 1);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}