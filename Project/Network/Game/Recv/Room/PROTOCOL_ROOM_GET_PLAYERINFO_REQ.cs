using System;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_GET_PLAYERINFO_REQ : GamePacketReader
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
                if (slotId >= 0 && slotId <= 15 && room != null)
                {
                    Account playerInfo = room.GetPlayerBySlot(slotId);
                    client.SendPacket(new PROTOCOL_ROOM_GET_PLAYERINFO_ACK(playerInfo, player.access >= AccessLevelEnum.GameMaster));
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.ROOM_GET_PLAYERINFO_NULL_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}