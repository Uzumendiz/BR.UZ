using System;

namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_JOIN_ROOM_REQ : GamePacketReader
    {
        private int roomId, type;
        private string password;
        public override void ReadImplement()
        {
            roomId = ReadInt();
            password = ReadString(4);
            type = ReadByteJoinRoom();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player != null && player.nickname.Length > 0 && player.room == null && player.match == null && player.GetChannel(out Channel channel) && client.PacketGetRoomList && player.loadedShop)
                {
                    Room room = channel.GetRoom(roomId);
                    if (room != null && room.GetLeader(out Account leader))
                    {
                        DateTime now = DateTime.Now;
                        if (room.mode == RoomTypeEnum.Tutorial || room.mapId == 44)
                        {
                            client.SendCompletePacket(PackageDataManager.LOBBY_JOIN_ROOM_0x8000107C_PAK);//Tutorial
                        }
                        else if (room.password.Length > 0 && password != room.password && player.rankId != 53 && !player.HaveGMLevel() && type != 1)
                        {
                            client.SendCompletePacket(PackageDataManager.LOBBY_JOIN_ROOM_0x80001005_PAK);
                        }
                        else if (room.limit == 1 && room.state >= RoomStateEnum.CountDown && !player.HaveGMLevel() || room.modeSpecial == RoomModeSpecial.CLAN_MATCH)
                        {
                            client.SendCompletePacket(PackageDataManager.LOBBY_JOIN_ROOM_0x80001013_PAK);//Entrada proibida com partida em andamento
                        }
                        else if (room.KickedPlayersVote.Contains(player.playerId) && !player.HaveGMLevel())
                        {
                            client.SendCompletePacket(PackageDataManager.LOBBY_JOIN_ROOM_0x8000100C_PAK);//Você foi expulso dessa sala.
                        }
                        else if (room.KickedPlayersHost.ContainsKey(player.playerId) && (now - room.KickedPlayersHost[player.playerId]).Seconds < Settings.IntervalEnterRoomAfterKickSeconds)
                        {
                            client.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK($"Você foi expulso por {Settings.IntervalEnterRoomAfterKickSeconds} segundos pelo dono da sala!\nAguarde {(now - room.KickedPlayersHost[player.playerId]).Seconds} segundos para entrar novamente."));
                        }
                        else if (room.AddPlayer(player) >= 0)
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
                            client.SendCompletePacket(PackageDataManager.LOBBY_JOIN_ROOM_0x80001003_PAK);//SLOTFULL
                        }
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.LOBBY_JOIN_ROOM_0x80001004_PAK);//INVALIDROOM
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.LOBBY_JOIN_ROOM_0x80001004_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}