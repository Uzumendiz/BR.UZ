using System;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CHANGE_HOST_REQ : GamePacketReader
    {
        private int slotId;
        public override void ReadImplement()
        {
            slotId = ReadInt();
        }

        public override void RunImplement()
        {
            Account player = client.SessionPlayer;
            Room room = player != null ? player.room : null;
            try
            {
                if (slotId < 0 || slotId > 15 || room == null || room.leaderSlot == slotId || room.slots[slotId].playerId == 0)
                {
                    client.SendCompletePacket(PackageDataManager.ROOM_CHANGE_HOST_ERROR_PAK);
                }
                else if (room.state == RoomStateEnum.Ready && room.leaderSlot == player.slotId)
                {
                    room.SetNewLeader(slotId, 0, room.leaderSlot, true);
                    using (ROOM_CHANGE_HOST_PAK packet = new ROOM_CHANGE_HOST_PAK(slotId))
                    {
                        room.SendPacketToPlayers(packet);
                    }
                    room.UpdateSlotsInfo();
                    room.UpdateRoomInfo();
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}