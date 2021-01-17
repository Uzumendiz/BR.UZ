using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_MISSION_BOMB_UNINSTALL_REQ : GamePacketReader
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
                if (slotId >= 0 && slotId <= 15 && room != null && room.round.Timer == null && room.state == RoomStateEnum.Battle && room.PlantedBombC4)
                {
                    Slot slot = room.GetSlot(slotId);
                    if (slot == null || slot.state != SlotStateEnum.BATTLE)
                    {
                        return;
                    }
                    using (BATTLE_MISSION_BOMB_UNINSTALL_PAK packet = new BATTLE_MISSION_BOMB_UNINSTALL_PAK(slot.Id))
                    {
                        room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                    }
                    if (room.mode != RoomTypeEnum.Tutorial && room.mapId != 44)
                    {
                        slot.objetivos++;
                        room.blueRounds++;
                        room.MissionCompleteBase(room.GetPlayerBySlot(slot), slot, MissionTypeEnum.C4_DEFUSE, 0);
                        room.BattleEndRound(1, RoundEndTypeEnum.Uninstall);
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