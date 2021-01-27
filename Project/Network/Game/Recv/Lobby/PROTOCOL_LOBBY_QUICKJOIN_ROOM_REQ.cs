using PointBlank.Api;
using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_QUICKJOIN_ROOM_REQ : GamePacketReader
    {
        private List<Room> salas;
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null)
                {
                    return;
                }
                if (player.nickname.Length > 0 && player.room == null && player.match == null && player.GetChannel(out Channel channel) && client.PacketGetRoomList)
                {
                    salas = new List<Room>();
                    lock (channel.rooms)
                    {
                        for (int roomId = 0; roomId < channel.rooms.Count; roomId++)
                        {
                            Room room = channel.rooms[roomId];
                            if (room.mode == RoomTypeEnum.Tutorial)
                            {
                                continue;
                            }
                            if (room.password.Length == 0 && room.limit == 0 && room.modeSpecial != RoomModeSpecial.CLAN_MATCH && (!room.KickedPlayersVote.Contains(player.playerId) || player.HaveGMLevel()))
                            {
                                for (int slotId = 0; slotId < 16; slotId++)
                                {
                                    Slot slot = room.slots[slotId];
                                    if (slot.playerId == 0 && slot.state == 0)
                                    {
                                        salas.Add(room);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (salas.Count == 0)
                {
                    client.SendCompletePacket(PackageDataManager.LOBBY_QUICKJOIN_ROOM_PAK);
                }
                else
                {
                    GetRandomRoom(player);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
            finally
            {
                salas = null;
            }
        }

        private void GetRandomRoom(Account player)
        {
            if (player != null)
            {
                Room room = salas[new Random().Next(salas.Count)];
                if (room != null && room.GetLeader(out Account leader) && room.AddPlayer(player) >= 0)
                {
                    player.ResetPages();
                    using (PROTOCOL_ROOM_GET_SLOTONEINFO_ACK packet = new PROTOCOL_ROOM_GET_SLOTONEINFO_ACK(player))
                    {
                        room.SendPacketToPlayers(packet, player.playerId);
                    }
                    client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0, player, leader));
                    ApiManager.SendPacketToAllClients(new API_USER_ROOM_ENTER_OR_CREATE_ACK(player, room, false));
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.LOBBY_QUICKJOIN_ROOM_PAK);
                }
            }
            else
            {
                client.SendCompletePacket(PackageDataManager.LOBBY_QUICKJOIN_ROOM_PAK);
            }
        }
    }
}