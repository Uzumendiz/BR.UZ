using System.Collections.Generic;

namespace PointBlank.Game
{
    public class BATTLE_RESPAWN_PAK : GamePacketWriter
    {
        private Slot slot;
        private Room room;
        public BATTLE_RESPAWN_PAK(Room room, Slot slot)
        {
            this.slot = slot;
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3338);
            WriteD(slot.Id);
            WriteD(room.spawnsCount++); //total number of all players' respawns
            WriteD(slot.spawnsCount++); //total number of current player's respawns
            WriteD(slot.equipment.primary);
            WriteD(slot.equipment.secondary);
            WriteD(slot.equipment.melee);
            WriteD(slot.equipment.grenade);
            WriteD(slot.equipment.special);
            WriteD(0);
            WriteB(new byte[6] { 100, 100, 100, 100, 100, 1 }); //Durabilidade das armas
            WriteD(slot.equipment.red);
            WriteD(slot.equipment.blue);
            WriteD(slot.equipment.helmet);
            WriteD(slot.equipment.beret);
            WriteD(slot.equipment.dino);
            if (room.mode == RoomTypeEnum.Dino || room.mode == RoomTypeEnum.CrossCounter)
            {
                List<int> pL = room.GetDinossaurs(false, slot.Id);
                int TRex = pL.Count == 1 || room.mode == RoomTypeEnum.CrossCounter ? 255 : room.TRex;
                WriteC((byte)TRex);
                for (int i = 0; i < pL.Count; i++)
                {
                    int slotId = pL[i];
                    if (slotId != room.TRex)
                    {
                        WriteC((byte)slotId);
                    }
                }
                int falta = 8 - pL.Count - (TRex == 255 ? 1 : 0);
                for (int i = 0; i < falta; i++)
                {
                    WriteC(255);
                }
                WriteC(255);
                WriteC(255);
            }
        }
    }
}