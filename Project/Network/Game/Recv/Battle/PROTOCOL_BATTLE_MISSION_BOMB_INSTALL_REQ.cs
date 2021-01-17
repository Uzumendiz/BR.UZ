using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_MISSION_BOMB_INSTALL_REQ : GamePacketReader
    {
        private int slotId;
        private float x, y, z;
        private byte area;
        public override void ReadImplement()
        {
            slotId = ReadInt();
            area = ReadByte();
            x = ReadFloat();
            y = ReadFloat();
            z = ReadFloat();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (slotId >= 0 && slotId <= 15 && room != null && room.round.Timer == null && room.state == RoomStateEnum.Battle && !room.PlantedBombC4)
                {
                    Slot slot = room.GetSlot(slotId);
                    if (slot == null || slot.state != SlotStateEnum.BATTLE)
                    {
                        return;
                    }
                    using (BATTLE_MISSION_BOMB_INSTALL_PAK packet = new BATTLE_MISSION_BOMB_INSTALL_PAK(slot.Id, area, x, y, z))
                    {
                        room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                    }
                    if (room.mode != RoomTypeEnum.Tutorial && room.mapId != 44)
                    {
                        room.PlantedBombC4 = true;
                        slot.objetivos++;
                        room.MissionCompleteBase(room.GetPlayerBySlot(slot), slot, MissionTypeEnum.C4_PLANT, 0);
                        room.StartBomb();
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