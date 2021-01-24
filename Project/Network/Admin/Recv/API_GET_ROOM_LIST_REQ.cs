using System;
using System.Collections.Generic;

namespace PointBlank.Api
{
    public class API_GET_ROOM_LIST_REQ : ApiPacketReader
    {
        private int channelId;
        public override void ReadImplement()
        {
            channelId = ReadInt();
        }

        public override void RunImplement()
        {
            Channel channel = ServersManager.GetChannel(channelId);
            if (channel != null)
            {
                channel.RemoveEmptyRooms();
                List<Room> rooms = channel.rooms;
                List<Account> waiting = channel.GetWaitPlayers();
                int Rpages = (int)Math.Ceiling(rooms.Count / 15d);
                int Apages = (int)Math.Ceiling(waiting.Count / 10d);
                if (client.lastRoomPage >= Rpages)
                {
                    client.lastRoomPage = 0;
                }
                if (client.lastPlayerPage >= Apages)
                {
                    client.lastPlayerPage = 0;
                }
                int roomsCount = 0, playersCount = 0;
                byte[] roomsArray = GetRoomListData(ref roomsCount, rooms);
                byte[] waitingArray = GetPlayerListData(client.lastPlayerPage, ref playersCount, waiting);
                client.SendPacket(new API_ROOM_LIST_ACK(rooms.Count, waiting.Count, client.lastRoomPage++, client.lastPlayerPage++, roomsCount, playersCount, roomsArray, waitingArray));
            }
        }

        private byte[] GetRoomListData(ref int count, List<Room> list)
        {
            using (PacketWriter send = new PacketWriter())
            {
                for (int i = client.lastRoomPage * 15; i < list.Count; i++)
                {
                    Room room = list[i];
                    byte restrictions = 0;
                    send.WriteD(room.roomId);
                    send.WriteH((ushort)room.mapId);
                    send.WriteC(room.stage4vs4);
                    send.WriteC((byte)room.mode);
                    send.WriteC((byte)room.state);
                    send.WriteC(room.GetAllPlayersCount());
                    send.WriteC(room.GetSlotCount());
                    send.WriteC(room.ping);
                    send.WriteC(room.weaponsFlag);
                    if (room.randomMap > 0)
                    {
                        restrictions += 2;
                    }
                    if (room.password.Length > 0)
                    {
                        restrictions += 4;
                    }
                    if (room.limit > 0 && room.state > RoomStateEnum.Ready)
                    {
                        restrictions += 128;
                    }
                    send.WriteC(restrictions);
                    send.WriteC((byte)room.modeSpecial);
                    if (count++ == 15)
                    {
                        break;
                    }
                }
                return send.memorystream.ToArray();
            }
        }
        private byte[] GetPlayerListData(int page, ref int count, List<Account> list)
        {
            using (PacketWriter send = new PacketWriter())
            {
                for (int i = page * 10; i < list.Count; i++)
                {
                    Account player = list[i];
                    Clan clan = ClanManager.GetClan(player.clanId);
                    send.WriteD(player.GetSessionId());
                    send.WriteD(clan.logo);
                    send.WriteS(clan.name, 17);
                    send.WriteH((short)player.GetRank());
                    send.WriteS(player.nickname, 33);
                    send.WriteC(player.nickcolor);
                    send.WriteC(player.country);
                    if (count++ == 10)
                    {
                        break;
                    }
                }
                return send.memorystream.ToArray();
            }
        }
    }
}