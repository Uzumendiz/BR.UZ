using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_RESPAWN_FOR_AI_REQ : GamePacketReader
    {
        private int slotId;
        public override void ReadImplement()
        {
            slotId = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (slotId >= 0 && slotId <= 15 && room != null && room.state == RoomStateEnum.Battle && player.slotId == room.leaderSlot)
                {
                    Slot slot = room.GetSlot(slotId);
                    if (slot != null)
                    {
                        slot.aiLevel = room.IngameAiLevel;
                    }
                    room.spawnsCount++;
                    using (BATTLE_RESPAWN_FOR_AI_PAK packet = new BATTLE_RESPAWN_FOR_AI_PAK(slotId))
                    {
                        room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
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