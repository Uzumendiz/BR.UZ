using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_LOADING_REQ : GamePacketReader
    {
        private string mapName;
        public override void ReadImplement()
        {
            mapName = ReadString(ReadByte());
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (mapName.Length > 3 && room != null && MapsXML.CheckInfo(room.mapId, mapName) && room.IsPreparing() && room.GetSlot(player.slotId, out Slot slot) && slot.state == SlotStateEnum.LOAD)
                {
                    room.StartCounter(0, player, slot);
                    room.ChangeSlotState(slot, SlotStateEnum.RENDEZVOUS, true);
                    room.mapName = mapName;
                    if (slot.Id == room.leaderSlot)
                    {
                        room.sessionPort = BattleManager.GetSessionPort();
                        Logger.Warning("PORTA: " + room.sessionPort);
                        room.state = RoomStateEnum.Rendezvous;
                        room.UpdateRoomInfo();
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