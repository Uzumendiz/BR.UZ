using System;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CHANGE_INFO_REQ : GamePacketReader
    {
        private int roomId;
        private string roomName;
        private short mapId;
        private byte stage4vs4;
        private RoomTypeEnum stageType;
        private RoomStateEnum state;
        private byte players;
        private byte slots;
        private byte ping;
        private byte weaponsFlag;
        private byte randomMap;
        private RoomModeSpecial modeSpecial;
        private string leaderName;
        private int killTime;
        private byte limit;
        private byte seeConf;
        private short balancing;
        private byte aiCount;
        private byte aiLevel;
        public override void ReadImplement()
        {
            roomId = ReadInt();
            roomName = ReadString(23);
            mapId = ReadShort();
            stage4vs4 = ReadByte();
            stageType = (RoomTypeEnum)ReadByte();
            state = (RoomStateEnum)ReadByte();
            players = ReadByte();
            slots = ReadByte();
            ping = ReadByte();
            weaponsFlag = ReadByte();
            randomMap = ReadByte();
            modeSpecial = (RoomModeSpecial)ReadByte();
            leaderName = ReadString(33);
            killTime = ReadInt();
            limit = ReadByte();
            seeConf = ReadByte();
            balancing = ReadShort();
            aiCount = ReadByte();
            aiLevel = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                string log = $"roomId: {roomId} " +
                             $"roomName: {roomName} " +
                             $"mapId: {mapId} " +
                             $"stage4vs4: {stage4vs4} " +
                             $"stageType: {stageType} " +
                             $"state: {state} " +
                             $"players: {players} " +
                             $"slots: {slots} " +
                             $"ping: {ping} " +
                             $"weaponsFlag: {weaponsFlag} " +
                             $"randomMap: {randomMap} " +
                             $"modeSpecial: {modeSpecial} " +
                             $"leaderName: {leaderName} " +
                             $"killTime: {killTime} " +
                             $"limit: {limit} " +
                             $"seeConf: {seeConf} " +
                             $"balancing: {balancing} " +
                             $"aiCount: {aiCount} " +
                             $"aiLevel: {aiLevel}";
                Logger.DebugPacket(GetType().Name, log);
                //if (roomId >= 0 && roomId <= Settings.MaxRoomsPerChannel && roomName.Length > 0 && stage4vs4 >= 0 && stage4vs4 <= 1 && stageType >= RoomTypeEnum.Tutorial && stageType <= RoomTypeEnum.Escort && leaderName.Length > Settings.NickMinLength && limit >= 0 && limit <= 1 && balancing >= 0 && balancing <= 2)
                //{
                //}
                Logger.DebugPacket(GetType().Name, $"RoomId: {roomId} RoomName: {roomName} MapId: {mapId} Stage4vs4: {stage4vs4} StageType: {stageType} State: {state} Players: {players} Slots: {slots} Ping: {ping} WeaponsFlag: {weaponsFlag} RandomMap: {randomMap} ModeSpecial: {modeSpecial} LeaderName: {leaderName} KillTime: {killTime} Limit: {limit} SeeConf: {seeConf} Balancing: {balancing} AiCount: {aiCount} AiLevel: {aiLevel}");
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (leaderName != player.nickname)
                {
                    Logger.Warning($" [PROTOCOL_ROOM_CHANGE_INFO_REQ] Nome do lider incorreto! Leader: {leaderName} Player: {player.nickname}");
                }
                if (room.roomId != roomId)
                {
                    Logger.Warning($" [PROTOCOL_ROOM_CHANGE_INFO_REQ] Id da sala incorreto! RoomId: {roomId} SalaId: {room.roomId}");
                }
                if (room != null && room.roomId == roomId && leaderName == player.nickname && room.leaderSlot == player.slotId && room.state == state && room.state == RoomStateEnum.Ready && MapsXML.CheckId(mapId))
                {
                    string oldName = room.roomName;
                    room.roomName = roomName;
                    room.mapId = mapId;
                    room.stage4vs4 = stage4vs4;
                    room.ping = ping;
                    room.randomMap = randomMap;
                    room.modeSpecial = modeSpecial;
                    room.leaderName = leaderName;
                    room.killtime = killTime;
                    room.limit = limit;
                    room.seeConf = seeConf;
                    room.balancing = (BalancingTeamEnum)balancing;
                    room.aiCount = aiCount;
                    room.aiLevel = aiLevel;
                    if (stageType != room.mode || weaponsFlag != room.weaponsFlag || oldName != roomName && TournamentRulesManager.CheckRoomRule(oldName.ToUpper()) || TournamentRulesManager.CheckRoomRule(roomName.ToUpper()))
                    {
                        room.mode = stageType;
                        if (!room.isModePorrada)
                        {
                            room.weaponsFlag = weaponsFlag;
                        }
                        int count = 0;
                        for (int i = 0; i < 16; i++)
                        {
                            Slot slot = room.slots[i];
                            if (slot.state == SlotStateEnum.READY)
                            {
                                slot.state = SlotStateEnum.NORMAL;
                                count++;
                            }
                        }
                        if (count > 0)
                        {
                            room.UpdateSlotsInfo();
                        }
                    }
                    room.UpdateRoomInfo();
                }
                else
                {
                    Logger.Warning($"[{GetType().Name}] NÃO FOI POSSIVEL ALTERAR AS INFORMAÇÕES DA SALA [!] RoomId ({room.roomId}/{roomId}) LeaderName ({leaderName}/{player.nickname}) LeaderSlot ({room.leaderSlot}/{player.slotId}) RoomState ({room.state}/{state})");
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}