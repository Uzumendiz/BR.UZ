using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_ROOM_INFO_REQ : GamePacketReader
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
            aiCount = ReadByte();
            aiLevel = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Logger.DebugPacket(GetType().Name, $"RoomName: {roomName} MapId: {mapId} Stage4vs4: {stage4vs4} StageType: {stageType} Ping: {ping} WeaponsFlag: {weaponsFlag} RandomMap: {randomMap} ModeSpecial: {modeSpecial} AiCount: {aiCount} AiLevel: {aiLevel} roomId: {roomId} state: {state} players: {players} slots: {slots}");
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room == null || room.state != state || room.state != RoomStateEnum.Ready || room.leaderSlot != player.slotId || !MapsXML.CheckId(mapId))
                {
                    return;
                }
                string oldName = room.roomName;
                room.roomName = roomName;
                room.mapId = mapId;
                room.stage4vs4 = stage4vs4;
                room.ping = ping;
                room.randomMap = randomMap;
                room.modeSpecial = modeSpecial;
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
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}