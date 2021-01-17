using System;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CLOSE_SLOT_REQ : GamePacketReader
    {
        private int slotInfo;
        public override void ReadImplement()
        {
            slotInfo = ReadInt(); //0x10000000 = Abrindo slot fechado || 0 = Fechando slot
        }

        public override void RunImplement()
        {
            try
            {
                Logger.DebugPacket(GetType().Name, $"SlotInfo: {slotInfo}");
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room != null && room.leaderSlot == player.slotId)
                {
                    Slot slot = room.GetSlot(slotInfo & 0xFFFFFFF);
                    if (slot == null)
                    {
                        return;
                    }
                    if ((slotInfo & 0x10000000) == 0x10000000)
                    {
                        OpenSlot(room, slot);
                    }
                    else
                    {
                        CloseSlot(room, slot);
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.ROOM_CLOSE_SLOT_ERROR_PAK);
                    return;
                }
                client.SendCompletePacket(PackageDataManager.ROOM_CLOSE_SLOT_SUCCESS_PAK);
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
        private void CloseSlot(Room room, Slot slot)
        {
            switch (slot.state)
            {
                case SlotStateEnum.EMPTY: room.ChangeSlotState(slot, SlotStateEnum.CLOSE, true); break;
                case SlotStateEnum.CLAN:
                case SlotStateEnum.NORMAL:
                case SlotStateEnum.INFO:
                case SlotStateEnum.INVENTORY:
                case SlotStateEnum.OUTPOST:
                case SlotStateEnum.SHOP:
                case SlotStateEnum.READY:
                    Account player = room.GetPlayerBySlot(slot);
                    if (player != null && !player.antiKickGM)
                    {
                        if (slot.state != SlotStateEnum.READY && (room.channelType == 4 && room.state != RoomStateEnum.CountDown || room.channelType != 4) || slot.state == SlotStateEnum.READY && (room.channelType == 4 && room.state == 0 || room.channelType != 4))
                        {
                            player.SendCompletePacket(PackageDataManager.SERVER_MESSAGE_KICK_PLAYER_PAK); //2147484673 - 4vs4 error
                            if (!room.KickedPlayersHost.ContainsKey(player.playerId))
                            {
                                room.KickedPlayersHost.Add(player.playerId, DateTime.Now);
                            }
                            else
                            {
                                room.KickedPlayersHost[player.playerId] = DateTime.Now;
                            }
                            room.RemovePlayer(player, slot, false);
                        }
                    }
                    break;
            }
        }
        private void OpenSlot(Room room, Slot slot)
        {
            if (((slotInfo & 0x10000000) != 0x10000000) || slot.state != SlotStateEnum.CLOSE)
            {
                return;
            }
            room.ChangeSlotState(slot, SlotStateEnum.EMPTY, true);
        }
    }
}