using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_CREATE_ROOM_REQ : GamePacketReader
    {
        private short matchId;
        private string roomName;
        private short mapId;
        private byte stage4vs4;
        private RoomTypeEnum mode;
        private byte slots;
        private byte weaponsFlag;
        private byte randomMap;
        private RoomModeSpecial modeSpecial;
        public override void ReadImplement()
        {
            try
            {
                matchId = ReadShort();
                int channel1 = ReadInt();
                int channel2 = ReadInt();
                ReadShort();
                roomName = ReadString(23);
                mapId = ReadShort();
                stage4vs4 = ReadByte();
                mode = (RoomTypeEnum)ReadByte();
                ReadShort();
                slots = ReadByte();
                ReadByte();
                weaponsFlag = ReadByte();
                randomMap = ReadByte();
                modeSpecial = (RoomModeSpecial)ReadByte();
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.clanId <= 0 || player.match == null)
                {
                    return;
                }
                Channel channel = player.GetChannel();
                if (channel == null)
                {
                    return;
                }
                Match MyMatch = player.match;
                Match EnemyMatch = channel.GetMatch(matchId);
                if (EnemyMatch == null)
                {
                    return;
                }
                lock (channel.rooms)
                {
                    for (int roomId = 0; roomId < Settings.MaxRoomsPerChannel; roomId++)
                    {
                        if (channel.GetRoom(roomId) == null)
                        {
                            Room room = new Room(roomId, channel)
                            {
                                roomName = roomName,
                                mapId = mapId,
                                stage4vs4 = stage4vs4,
                                mode = mode
                            };
                            room.InitSlotCount(slots);
                            room.weaponsFlag = weaponsFlag;
                            room.randomMap = randomMap;
                            room.modeSpecial = modeSpecial;
                            room.password = "";
                            room.killtime = 3;
                            room.AddPlayer(player);
                            channel.AddRoom(room);
                            client.SendPacket(new LOBBY_CREATE_ROOM_PAK(0, room, player));

                            using (CLAN_WAR_ENEMY_INFO_PAK packet = new CLAN_WAR_ENEMY_INFO_PAK(EnemyMatch))
                            using (CLAN_WAR_JOINED_ROOM_PAK packet2 = new CLAN_WAR_JOINED_ROOM_PAK(EnemyMatch, roomId, 0))
                            {
                                byte[] data = packet.GetCompleteBytes("CLAN_WAR_CREATE_ROOM_REQ-1");
                                byte[] data2 = packet2.GetCompleteBytes("CLAN_WAR_CREATE_ROOM_REQ-2");
                                foreach (Account pM in MyMatch.GetAllPlayers(MyMatch.leader))
                                {
                                    if (pM.match != null)
                                    {
                                        pM.SendCompletePacket(data);
                                        pM.SendCompletePacket(data2);
                                        MyMatch.slots[pM.matchSlot].state = SlotMatchStateEnum.Ready;
                                    }
                                }
                            }
                            using (CLAN_WAR_ENEMY_INFO_PAK packet = new CLAN_WAR_ENEMY_INFO_PAK(MyMatch))
                            using (CLAN_WAR_JOINED_ROOM_PAK packet2 = new CLAN_WAR_JOINED_ROOM_PAK(MyMatch, roomId, 1))
                            {
                                byte[] data = packet.GetCompleteBytes("CLAN_WAR_CREATE_ROOM_REQ-3");
                                byte[] data2 = packet2.GetCompleteBytes("CLAN_WAR_CREATE_ROOM_REQ-4");
                                foreach (Account pM in EnemyMatch.GetAllPlayers())
                                {
                                    if (pM.match != null)
                                    {
                                        pM.SendCompletePacket(data);
                                        pM.SendCompletePacket(data2);
                                        MyMatch.slots[pM.matchSlot].state = SlotMatchStateEnum.Ready;
                                    }
                                }
                            }
                            return;
                        }
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