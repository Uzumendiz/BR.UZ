using System;

namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_CREATE_ROOM_REQ : GamePacketReader
    {
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
        private string password;
        private byte aiCount;
        private byte aiLevel;
        private bool IsSpecialMode;
        public override void ReadImplement()
        {
            int roomId = ReadInt();
            roomName = ReadString(23);
            mapId = ReadShort();
            stage4vs4 = ReadByte();
            byte type = ReadByte();
            stageType = (RoomTypeEnum)type;
            state = (RoomStateEnum)ReadByte();
            players = ReadByte();
            slots = ReadByte();
            ping = ReadByte();
            weaponsFlag = ReadByte(); //RoomWeaponsFlag È um Enum
            randomMap = ReadByte(); //0=DESATIVADO 2=ATIVADO
            modeSpecial = (RoomModeSpecial)ReadByte(); //2=DINO 3=SHOTGUN 4=SNIPER 6=CHALLENGE NORMAL 7=HEADHUNTER 8=CHALLENGE KNIFE 9=ZOMBIE 10=CHAOS (MODO PORRADA NAO É MODO SPECIAL=0)
            leaderName = ReadString(33);
            killTime = ReadInt();
            limit = ReadByte(); //BLOQUEAR A ENTRADA QUANDO A PARTIDA ESTIVER EM ANDAMENTO(JOGANDO)
            seeConf = ReadByte(); //[MIN 0 MAX 30] 0=DESATIVADO 1=TERCEIRA PESSOA 2=CAMERA LIVRE 4=VISUALIZAR TIME ADVERSÁRIO 8=VER HP DO INIMIGO 16=DESATIVAR O USO DA VISÃO DE TERCEIRA PESSOA
            balancing = ReadShort();
            password = ReadString(4);
            IsSpecialMode = modeSpecial == RoomModeSpecial.CHALLENGE_NORMAL || modeSpecial == RoomModeSpecial.CHALLENGE_KNIFE || modeSpecial == RoomModeSpecial.ZOMBIE;
            if (IsSpecialMode)
            {
                aiCount = ReadByte();
                aiLevel = ReadByte();
            }
            Logger.DebugPacket(GetType().Name, $"RoomId: {roomId} RoomName: {roomName} MapId: {mapId} Stage4vs4: {stage4vs4} StageType: {stageType} Type: {type} State: {state} Players: {players} Slots: {slots} Ping: {ping} WeaponsFlag: {weaponsFlag} RandomMap: {randomMap} ModeSpecial: {modeSpecial} KillTime: {killTime} Limit: {limit} SeeConf: {seeConf} Balancing: {balancing} AiCount: {aiCount} AiLevel: {aiLevel}");
        }

        public override void RunImplement()
        {
            try
            {
                //MODO CHAOS E PORRADA: BALANCING [!] DESATIVADO: 256 QTY: 257 RANK: 258
                //value, initSlotCount, killTime, seeConf, modeSpecial, aiCount, aiLevel balancing e outros reads sem uso
                //if (stageType <= RoomTypeEnum.Tutorial || stageType > RoomTypeEnum.Escort || roomName.Length == 0 || !MapsIdXML.CheckId(mapId) || weaponsFlag < 1 || weaponsFlag > 128 || randomMap < 0 || randomMap > 2 || limit < 0 || limit > 1 || stage4vs4 < 0 || stage4vs4 > 1 || ping < 0 || ping > 5 || killTime != 66 || mapId == 44)
                //{
                //    Logger.Warning(" [PROTOCOL_LOBBY_CREATE_ROOM_REQ] Informações inválidas foram recebidas [!] PlayerId: " + client.playerId);
                //    client.SendCompletePacket(PackageDataManager.LOBBY_CREATE_ROOM_0x80000000_PAK);
                //    return;
                //}
                Account player = client.SessionPlayer;
                Channel channel = player != null ? player.GetChannel() : null;
                DateTime now = DateTime.Now;
                if (player == null || channel == null || player.nickname.Length < Settings.NickMinLength || player.nickname.Length > Settings.NickMaxLength || player.room != null || player.match != null || !client.PacketGetRoomList || (now - player.lastCreateRoom).TotalSeconds < 4)
                {
                    Logger.Warning(" [PROTOCOL_LOBBY_CREATE_ROOM_REQ] Não foi possivel criar a sala.");
                    client.Close();
                    return;
                }
                player.lastCreateRoom = now;
                if (stageType == RoomTypeEnum.Tutorial || mapId == 44)
                {
                    Logger.Warning(" [PROTOCOL_LOBBY_CREATE_ROOM_REQ] Não é permitido criar sala modo e/ou mapa tutorial.");
                    return;
                }
                if (IsSpecialMode && channel.type == 4)
                {
                    client.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK("Não é permitido criar sala modo special no servidor de clã."));
                    Logger.Warning(" [PROTOCOL_LOBBY_CREATE_ROOM_REQ] Não é permitido criar sala modo special no servidor de clã.");
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
                                mode = stageType,
                                leaderName = leaderName
                            };
                            room.GenerateRoomSeed();
                            room.InitSlotCount(slots);
                            room.weaponsFlag = weaponsFlag;
                            if (weaponsFlag == 128)
                            {
                                room.isModePorrada = true;
                            }
                            room.randomMap = randomMap;
                            room.modeSpecial = modeSpecial;
                            room.killtime = killTime;
                            if (channel.type == 4)
                            {
                                room.limit = 1;
                                room.balancing = 0;
                            }
                            else
                            {
                                room.limit = limit;
                                room.balancing = (BalancingTeamEnum)balancing;
                            }
                            room.seeConf = seeConf;
                            room.password = password;
                            if (IsSpecialMode)
                            {
                                room.aiCount = aiCount;
                                room.aiLevel = aiLevel;
                            }
                            if (modeSpecial == RoomModeSpecial.CHALLENGE_NORMAL || modeSpecial == RoomModeSpecial.CHALLENGE_KNIFE) //OBS: 9=Zumbie Mode: chama o pacote para fechar os slots.
                            {
                                room.ChangeSlotState(1, SlotStateEnum.CLOSE, true);
                                room.ChangeSlotState(3, SlotStateEnum.CLOSE, true);
                                room.ChangeSlotState(5, SlotStateEnum.CLOSE, true);
                                room.ChangeSlotState(7, SlotStateEnum.CLOSE, true);
                            }
                            if (room.AddPlayer(player) >= 0 && channel.AddRoom(room))
                            {
                                player.ResetPages();
                                client.SendPacket(new LOBBY_CREATE_ROOM_PAK(0, room, player));
                                client.SendPacket(new PROTOCOL_ROOM_GET_SLOTINFO_ACK(room));
                                ApiManager.SendPacketToAllClients(new API_USER_ROOM_ENTER_OR_CREATE_ACK(player, room, true));
                                return;
                            }
                        }
                    }
                    client.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK("Não foi possivel criar uma sala!\nLimite de salas por canal foi excedido."));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}