using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_REQUEST_HOST_REQ : GamePacketReader
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
                if (room != null)
                {
                    if (room.state != RoomStateEnum.Ready || room.leaderSlot == player.slotId)
                    {
                        return;
                    }
                    List<Account> players = room.GetAllPlayers();
                    if (players.Count == 0)
                    {
                        return;
                    }
                    if (player.access >= AccessLevelEnum.Developer)
                    {
                        ChangeLeader(room, players, player.slotId);
                    }
                    else
                    {
                        if (!room.RequestHost.Contains(player.playerId))
                        {
                            room.RequestHost.Add(player.playerId);
                            if (room.RequestHost.Count() < (players.Count / 2) + 1)
                            {
                                using (PROTOCOL_ROOM_GET_HOST_ACK packet = new PROTOCOL_ROOM_GET_HOST_ACK(player.slotId))
                                {
                                    SendPacketToRoom(packet, players);
                                }
                            }
                        }
                        if (room.RequestHost.Count() >= (players.Count / 2) + 1)
                        {
                            ChangeLeader(room, players, player.slotId);
                        }
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.ROOM_GET_HOST_ERROR_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
        private void ChangeLeader(Room room, List<Account> players, int slotId)
        {
            room.SetNewLeader(slotId, 0, -1, false);
            using (ROOM_CHANGE_HOST_PAK packet = new ROOM_CHANGE_HOST_PAK(slotId))
            {
                SendPacketToRoom(packet, players);
            }
            room.UpdateSlotsInfo();
            room.RequestHost.Clear();
        }
        private void SendPacketToRoom(GamePacketWriter packet, List<Account> players)
        {
            byte[] data = packet.GetCompleteBytes("ROOM_REQUEST_HOST_REQ");
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(data);
            }
        }
    }
}