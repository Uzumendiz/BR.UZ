using PointBlank.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PointBlank
{
    public class Room
    {
        public Slot[] slots = new Slot[16];
        public readonly int[] TIMES = new int[9] { 3, 5, 7, 5, 10, 15, 20, 25, 30 };
        public readonly int[] KILLS = new int[6] { 60, 80, 100, 120, 140, 160 };
        public readonly int[] ROUNDS = new int[6] { 1, 2, 3, 5, 7, 9 };
        public readonly int[] RED_TEAM = new int[8] { 0, 2, 4, 6, 8, 10, 12, 14 };
        public readonly int[] BLUE_TEAM = new int[8] { 1, 3, 5, 7, 9, 11, 13, 15 };
        public byte[] HitParts = new byte[35];
        public byte[] DefaultParts = new byte[35] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34 };
        public int roomId;
        public int roomSeed;
        public int sessionPort;
        public int mapId;
        public int channelId;
        public int channelType;
        public byte rounds = 1;
        public int TRex = -1;
        public ushort blueRounds;
        public ushort blueDino;
        public ushort redRounds;
        public ushort redDino;
        public byte ping = 5;
        public ushort redKills;
        public ushort redDeaths;
        public ushort blueKills;
        public ushort blueDeaths;
        public int spawnsCount;
        public int killtime;
        public int leaderSlot;
        public string leaderName = "";
        public byte weaponsFlag;
        public byte randomMap; //Se a randomização de mapas está ativada. ()
        public RoomModeSpecial modeSpecial; //Valor do modo special. (2=DINO 3=SHOTGUN 4=SNIPER 6=CHALLENGE NORMAL 7=HEADHUNTER 8=CHALLENGE KNIFE 9=ZUMBIE 10=CHAOS (MODO PORRADA NAO É MODO SPECIAL=0)
        public byte limit; //Bloqueia a entrada de jogadores na sala em andamento. (0=DESATIVADO 1=ATIIVADO)
        public byte seeConf; //Configurações de Observar da sala. (0=DESATIVADO 1=TERCEIRA PESSOA 2=CAMERA LIVRE 4=VISUALIZAR TIME ADVERSÁRIO 8=VER HP DO INIMIGO 16=DESATIVAR O USO DA VISÃO DE TERCEIRA PESSOA) Se selecionar varios é somado o valor de cada um. Valor Maximo: 30
        public byte aiCount = 1; //Quantidade de BOTS. (Minimo: 1 Maximo: 8)
        public byte aiLevel = 1; //Level do modo BOT. (Minimo: 1 Maximo: 10)
        public byte IngameAiLevel;
        public byte stage4vs4; //Se o modo da sala é 4 vs 4. (0=DESATIVADO 1=ATIVADO)
        public uint timeRoom;
        public uint StartDate;
        public uint UniqueRoomId;
        public long StartTick;
        public string roomName;
        public string password;
        public string mapName;
        public VoteKick votekick;
        public RoomTypeEnum mode;
        public RoomStateEnum state;
        public BalancingTeamEnum balancing;
        public bool PlantedBombC4;
        public bool isModePorrada;
        public bool swapRound;
        public bool changingSlots;
        public bool blockedClan;

        public DateTime BattleStart;
        public DateTime lastPingSync;
        public DateTime lastChangeTeam;

        public TimerState bomba = new TimerState();
        public TimerState countdown = new TimerState();
        public TimerState round = new TimerState();
        public TimerState vote = new TimerState();
        public SafeList<long> KickedPlayersVote = new SafeList<long>();
        public SafeList<long> RequestHost = new SafeList<long>();
        public SortedList<long, DateTime> KickedPlayersHost = new SortedList<long, DateTime>();

        #region BATTLE

        private object LockOne = new object();
        private object LockTwo = new object();

        public ObjectInfo[] Objects = new ObjectInfo[200];
        public DateTime LastObjectsSync;
        public DateTime LastPlayersSync;
        public int Bar1 = 6000;
        public int Bar2 = 6000;
        public int DefaultBar1 = 6000;
        public int DefaultBar2 = 6000;
        public int ObjectsSyncRound;
        public int SourceToMap = -1;
        public int LastRound;
        public int DropCounter;
        public bool IsCase4BotMode;
        public long LastStartTick;
        public MapModel Map;
        public Half3 BombPosition;
        public DateTime StartTime;

        public float GetStartTime()
        {
            return (float)(DateTime.Now - StartTime).TotalSeconds;
        }
        public uint GetRoomIdByUniqueId() //Obtém a RoomId pelo UniqueRoomId
        {
            return UniqueRoomId & 0x0000000fff;
        }

        /// <param name="objects"></param>
        /// <param name="type">1 = Obj | 2 = Players</param>
        public void SyncInfo(List<ObjectHitInfo> objects, int type)
        {
            lock (LockTwo)
            {
                if (IsCase4BotMode || !ObjectsIsValid())
                {
                    return;
                }
                DateTime now = DateTime.Now;
                double timeObjects = (now - LastObjectsSync).TotalSeconds;
                double timePlayers = (now - LastPlayersSync).TotalSeconds;
                if (timeObjects >= 2.5 && (type & 1) == 1)
                {
                    LastObjectsSync = now;
                    for (int i = 0; i < Objects.Length; i++)
                    {
                        ObjectInfo objectInfo = Objects[i];
                        ObjModel objModel = objectInfo.info;
                        if (objModel != null && (objModel.IsDestroyable && objectInfo.life != objModel.Life || objModel.NeedSync))
                        {
                            float SyncingTime = Packet4Creator.GetTime(objectInfo.useDate);
                            AnimModel anim = objectInfo.animation;
                            if (anim != null && anim.Duration > 0 && SyncingTime >= anim.Duration)
                            {
                                objModel.GetAnim(anim.NextAnim, SyncingTime, anim.Duration, objectInfo);
                            }
                            objects.Add(new ObjectHitInfo(objModel.UpdateId)
                            {
                                ObjSyncId = objModel.NeedSync ? 1 : 0,
                                AnimationId1 = objModel.Anim1,
                                AnimationId2 = objectInfo.animation != null ? objectInfo.animation.Id : 255,
                                DestroyState = objectInfo.destroyState,
                                ObjId = objModel.Id,
                                ObjectLife = objectInfo.life,
                                SpecialUse = SyncingTime
                            });
                        }
                    }
                }
                if (timePlayers >= 1/* 6.5 */&& (type & 2) == 2)
                {
                    LastPlayersSync = now;
                    for (int i = 0; i < 16; i++)
                    {
                        Slot slot = slots[i];
                        if (!slot.TRexImmortal && (slot.maxLife != slot.life || slot.isDead))
                        {
                            objects.Add(new ObjectHitInfo(4)
                            {
                                ObjId = slot.Id,
                                ObjectLife = slot.life
                            });
                        }
                    }
                }
            }
        }

        public bool ObjectsIsValid()
        {
            return rounds == ObjectsSyncRound;
        }

        /// <summary>
        /// Checa se a data de ínicio da partida é válida. Caso seja válida, reseta as informações da sala, como objetos, informações de drop..
        /// StartTick = Começa a contar quando inicia a partida. (StartBattle)
        /// Reseta as informações da sala quando chamar o void pela segunda vez, se a partida foi iniciada.
        /// </summary>
        /// <param name="newvalue">Nova data</param>
        public void ResyncTick()
        {
            if (StartTick > LastStartTick)
            {
                StartTime = new DateTime(StartTick);
                if (LastStartTick > 0)
                {
                    ResetRoomInfo();
                }
                LastStartTick = StartTick;
            }
        }

        public void ResetRoomInfo()
        {
            for (int i = 0; i < 200; i++)
            {
                Objects[i] = new ObjectInfo(i);
            }
            SourceToMap = -1;
            Map = null;
            LastRound = 0;
            DropCounter = 0;
            PlantedBombC4 = false;
            ObjectsSyncRound = 0;
            LastObjectsSync = new DateTime();
            LastPlayersSync = new DateTime();
            BombPosition = new Half3();
        }

        /// <summary>
        /// Reseta as informações da sala, caso a rodada seja válida.
        /// </summary>
        /// <param name="round">Rodada</param>
        /// <returns></returns>
        public bool RoundResetRoomF1()
        {
            lock (LockOne)
            {
                if (LastRound != rounds)
                {
                    DateTime now = DateTime.Now;
                    LastRound = rounds;
                    PlantedBombC4 = false;
                    BombPosition = new Half3();
                    DropCounter = 0;
                    ObjectsSyncRound = 0;
                    SourceToMap = mapId;
                    if (!IsCase4BotMode)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            slots[i].ResetLife();
                        }
                        LastPlayersSync = now;
                        Map = MappingXML.GetMapById(mapId);
                        if (Map != null)
                        {
                            List<ObjModel> listObjects = Map.objects;
                            if (listObjects != null)
                            {
                                for (int i = 0; i < listObjects.Count; i++)
                                {
                                    ObjModel objectModel = listObjects[i];
                                    ObjectInfo objectInfo = Objects[objectModel.Id];
                                    objectInfo.life = objectModel.Life;
                                    if (!objectModel.NoInstaSync)
                                    {
                                        objectModel.GetARandomAnim(this, objectInfo);
                                    }
                                    else
                                    {
                                        objectInfo.animation = new AnimModel { NextAnim = 1 };
                                        objectInfo.useDate = now;
                                    }
                                    objectInfo.info = objectModel;
                                    objectInfo.destroyState = 0;
                                    if (objectModel.UltraSYNC == 1 || objectModel.UltraSYNC == 3)
                                    {
                                        Bar1 = objectModel.Life;
                                        DefaultBar1 = Bar1;
                                    }
                                    else if (objectModel.UltraSYNC == 2 || objectModel.UltraSYNC == 4)
                                    {
                                        Bar2 = objectModel.Life;
                                        DefaultBar2 = Bar2;
                                    }
                                }
                            }
                        }
                        LastObjectsSync = now;
                        ObjectsSyncRound = rounds;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reseta as informações da sala, caso a rodada seja válida.
        /// </summary>
        /// <param name="round">Rodada</param>
        /// <returns></returns>
        public bool RoundResetRoomS1()
        {
            lock (LockOne)
            {
                if (LastRound != rounds)
                {
                    LastRound = rounds;
                    PlantedBombC4 = false;
                    DropCounter = 0;
                    BombPosition = new Half3();
                    if (!IsCase4BotMode)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            slots[i].ResetLife();
                        }
                        DateTime now = DateTime.Now;
                        LastPlayersSync = now;
                        for (int i = 0; i < Objects.Length; i++)
                        {
                            ObjectInfo objectInfo = Objects[i];
                            ObjModel objectModel = objectInfo.info;
                            if (objectModel != null)
                            {
                                objectInfo.life = objectModel.Life;
                                if (!objectModel.NoInstaSync)
                                {
                                    objectModel.GetARandomAnim(this, objectInfo);
                                }
                                else
                                {
                                    objectInfo.animation = new AnimModel { NextAnim = 1 };
                                    objectInfo.useDate = now;
                                }
                                objectInfo.destroyState = 0;
                            }
                        }
                        LastObjectsSync = now;
                        ObjectsSyncRound = rounds;
                        if (mode == RoomTypeEnum.Sabotage || mode == RoomTypeEnum.Defense)
                        {
                            Bar1 = DefaultBar1;
                            Bar2 = DefaultBar2;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public Slot GetPlayer(int slot, bool isActive)
        {
            Slot player = GetSlot(slot);
            return player != null && (!isActive || player.client != null) ? player : null;
        }
        public bool GetPlayer(int slotId, out Slot player)
        {
            player = GetSlot(slotId);
            return player != null;
        }

        public Slot GetPlayer(int slotId, IPEndPoint client)
        {
            Slot slot = GetSlot(slotId);
            return slot != null && slot.CompareIP(client) ? slot : null;
        }

        public Slot GetPlayer(IPEndPoint client)
        {
            for (int i = 0; i < 16; i++)
            {
                Slot slot = slots[i];
                if (slot.CompareIP(client))
                {
                    return slot;
                }
            }
            return null;
        }

        public ObjectInfo GetObject(int id)
        {
            try
            {
                return Objects[id];
            }
            catch
            {
                return null;
            }
        }
        #endregion

        public Room(int roomId, Channel channel)
        {
            this.roomId = roomId;
            for (int i = 0; i < 16; i++)
            {
                slots[i] = new Slot(i);
            }
            for (int i = 0; i < 200; i++)
            {
                Objects[i] = new ObjectInfo(i);
            }
            channelId = channel.id;
            channelType = channel.type;
            SetUniqueId();
            DateTime now = new DateTime();
            lastChangeTeam = now;
            lastPingSync = now;
        }
        private void SetUniqueId() => UniqueRoomId = (uint)(((Settings.ServerId & 0xff) << 20) | ((channelId & 0xff) << 12) | (roomId & 0xfff));
        public int GetTimeByMask() => TIMES[killtime >> 4];
        public int GetRoundsByMask() => ROUNDS[killtime & 15];
        public int GetKillsByMask() => KILLS[killtime & 15];
        public int[] GetTeamArray(int index) => index == 0 ? RED_TEAM : BLUE_TEAM;
        public int GetNewSlotId(int slotIdx) => slotIdx % 2 == 0 ? (slotIdx + 1) : (slotIdx - 1);
        public void GenerateRoomSeed()
        {
            roomSeed = (mapId * 16) + (int)mode;
        }
        //public void SetSeed()
        //{
        //    Seed = (uint)(((int)mapId & 0xff) << 20 | (rule & 0xff) << 12 | (int)room_type & 0xfff);
        //}
        /// <summary>
        /// Se estiver em contagem regressiva, preparação, ou em batalha, retorna TRUE, caso contrário FALSE.
        /// <para>(state > RoomState.Empty)</para>
        /// </summary>
        public bool IsStartingMatch() => state > RoomStateEnum.Ready;

        /// <summary>
        /// Se estiver em preparação, ou em batalha, retorna TRUE, caso contrário FALSE.
        /// <para>Não funciona em contagem regressiva.</para>
        /// <para>(state >= RoomState.Loading)</para>
        /// </summary>
        /// <returns></returns>
        public bool IsPreparing() => state >= RoomStateEnum.Loading && state < RoomStateEnum.BattleEnd;

        /// <summary>
        /// Retorna TRUE se o modo atual tiver contagem regressiva.
        /// </summary>
        public bool ThisModeHaveCountDown()
        {
            return mode == RoomTypeEnum.Destruction
                || mode == RoomTypeEnum.Suppression 
                || mode == RoomTypeEnum.Dino
                || mode == RoomTypeEnum.CrossCounter
                || mode == RoomTypeEnum.Escort;
        }

        /// <summary>
        /// Retorna TRUE se o modo atual utilizar o contador de rodadas.
        /// </summary>
        public bool ThisModeHaveRounds()
        {
            return mode == RoomTypeEnum.Destruction
                || mode == RoomTypeEnum.Sabotage 
                || mode == RoomTypeEnum.Suppression 
                || mode == RoomTypeEnum.Defense;
        }

        public bool IsBotMode()
        {
            return modeSpecial == RoomModeSpecial.CHALLENGE_NORMAL
                || modeSpecial == RoomModeSpecial.CHALLENGE_KNIFE
                || modeSpecial == RoomModeSpecial.ZOMBIE;
        }
        
        public void LoadHitParts()
        {
            Random rnd = new Random();
            int next = rnd.Next(34);
            byte[] MyRandomArray = DefaultParts.OrderBy(x => x <= next).ToArray();

            byte first = MyRandomArray[14];
            byte second = MyRandomArray[29];
            MyRandomArray[29] = first;
            MyRandomArray[14] = second;

            Logger.Warning("By: " + next + "/ Hits: " + BitConverter.ToString(MyRandomArray));
            HitParts = MyRandomArray;

            byte[] newarray = new byte[35];
            for (int i = 0; i < 35; i++)
            {
                byte valor = HitParts[i];
                newarray[(i + 8) % 35] = valor;
            }
            Logger.Warning("P: " + BitConverter.ToString(newarray));
        }
        public void SetBotLevel()
        {
            if (!IsBotMode())
            {
                return;
            }
            IngameAiLevel = aiLevel;
            for (int i = 0; i < 16; i++)
            {
                slots[i].aiLevel = IngameAiLevel;
            }
        }

        private void SetSpecialStage()
        {
            if (mode == RoomTypeEnum.Defense)
            {
                if (mapId == 39)
                {
                    Bar1 = 6000;
                    Bar2 = 9000;
                }
            }
            else if (mode == RoomTypeEnum.Sabotage)
            {
                if (mapId == 38)
                {
                    Bar1 = 12000;
                    Bar2 = 12000;
                }
                else if (mapId == 35)
                {
                    Bar1 = 6000;
                    Bar2 = 6000;
                }
            }
        }
        /// <summary>
        /// Retorna os segundos decorridos desde o inicio da partida.
        /// </summary>
        /// <returns>Tempo em segundos</returns>
        public int GetInBattleTime()
        {
            int seconds = 0;
            if (BattleStart != new DateTime() && (state == RoomStateEnum.Battle || state == RoomStateEnum.PreBattle))
            {
                seconds = (int)(DateTime.Now - BattleStart).TotalSeconds;
                if (seconds < 0)
                {
                    seconds = 0;
                }
            }
            return seconds;
        }
        /// <summary>
        /// Retorna o tempo restante que falta para a partida terminar.
        /// </summary>
        /// <returns>Tempo em segundos</returns>
        public int GetInBattleTimeLeft()
        {
            int remaining = GetInBattleTime();
            return (GetTimeByMask() * 60) - remaining; 
        }
        public bool GetChannel(out Channel channel)
        {
            channel = ServersManager.GetChannel(channelId);
            return channel != null;
        }
        public bool GetSlot(int slotIdx, out Slot slot)
        {
            slot = null;
            lock (slots)
            {
                if (slotIdx >= 0 && slotIdx <= 15)
                {
                    slot = slots[slotIdx];
                }
                return slot != null;
            }
        }
        public Slot GetSlot(int slotIdx)
        {
            lock (slots)
            {
                return slotIdx >= 0 && slotIdx <= 15 ? slots[slotIdx] : null;
            }
        }
        /// <summary>
        /// Inicia o temporizador de DC do jogador.
        /// <para>Type = 0: (90 segundos)</para>
        /// <para>Type = 1: (30 segundos)</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="player"></param>
        /// <param name="slot"></param>
        public void StartCounter(int type, Account player, Slot slot)
        {
            int dueTime = 0;
            EventErrorEnum error = 0;
            if (type == 0)
            {
                error = EventErrorEnum.Battle_First_MainLoad;
                dueTime = 90000;
            }
            else if (type == 1)
            {
                error = EventErrorEnum.Battle_First_Hole;
                dueTime = 30000;
            }
            slot.timing.StartJob(dueTime, (callbackState) =>
            {
                FirewallSecurity.RemoveRuleUdp(player.client.GetIPAddress(), sessionPort);
                player.SendPacket(new SERVER_MESSAGE_KICK_BATTLE_PLAYER_PAK(error));
                player.SendPacket(new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(player, 0));
                slot.state = SlotStateEnum.NORMAL;
                BattleEndPlayersCount(IsBotMode());
                UpdateSlotsInfo();
                lock (callbackState)
                {
                    if (slot != null)
                    {
                        slot.StopTiming();
                    }
                }
            });
        }
        public void StartBomb()
        {
            try
            {
                bomba.StartJob(42000, (callbackState) =>
                {
                    if (PlantedBombC4)
                    {
                        PlantedBombC4 = false;
                        redRounds++;
                        BattleEndRound(0, RoundEndTypeEnum.BombFire);
                    }
                    lock (callbackState)
                    {
                        bomba.Timer = null;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
        public void StartVote()
        {
            try
            {
                if (votekick == null)
                {
                    return;
                }
                vote.StartJob(20000, (callbackState) =>
                {
                    VotekickResult();
                    lock (callbackState)
                    {
                        vote.Timer = null;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                if (vote.Timer != null)
                {
                    vote.Timer = null;
                }
                votekick = null;
            }
        }

        public void RoundRestart()
        {
            try
            {
                StopBomb();
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = slots[i];
                    if (slot.playerId > 0 && slot.state == SlotStateEnum.BATTLE)
                    {
                        if (!slot.deathState.HasFlag(DeadEnum.useChat))
                        {
                            slot.deathState |= DeadEnum.useChat;
                        }
                        if (slot.espectador)
                        {
                            slot.espectador = false;
                        }
                        if (slot.killsOnLife >= 3 && mode == RoomTypeEnum.Suppression)
                        {
                            slot.objetivos++;
                        }
                        slot.killsOnLife = 0;
                        slot.lastKillState = 0;
                        slot.repeatLastState = false;
                        slot.damageBar1 = 0;
                        slot.damageBar2 = 0;
                    }
                }
                round.StartJob(8000, (callbackState) =>
                {
                    for (int i = 0; i < 16; i++)
                    {
                        Slot slot = slots[i];
                        if (slot.playerId > 0)
                        {
                            if (!slot.deathState.HasFlag(DeadEnum.useChat))
                            {
                                slot.deathState |= DeadEnum.useChat;
                            }
                            if (slot.espectador)
                            {
                                slot.espectador = false;
                            }
                        }
                    }
                    StopBomb();
                    DateTime now = DateTime.Now;
                    if (state == RoomStateEnum.Battle)
                    {
                        BattleStart = mode == RoomTypeEnum.Dino || mode == RoomTypeEnum.CrossCounter ? now.AddSeconds(5) : now;
                    }
                    using (BATTLE_ROUND_RESTART_PAK packet = new BATTLE_ROUND_RESTART_PAK(this))
                    using (PROTOCOL_BATTLE_TIMERSYNC_ACK packet2 = new PROTOCOL_BATTLE_TIMERSYNC_ACK(this))
                    {
                        SendPacketToPlayers(packet, packet2, SlotStateEnum.BATTLE, 0);
                    }
                    StopBomb();
                    swapRound = false;
                    lock (callbackState)
                    {
                        round.Timer = null;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
        public void StopBomb()
        {
            if (!PlantedBombC4)
            {
                return;
            }
            PlantedBombC4 = false;
            if (bomba != null)
            {
                bomba.Timer = null;
            }
        }
        public void StartBattle(bool updateInfo)
        {
            lock (slots)
            {
                state = RoomStateEnum.Loading;
                RequestHost.Clear();
                SetBotLevel();
                CheckClanMatchRestrict();
                StartTick = DateTime.Now.Ticks;
                StartDate = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                using (BATTLE_READYBATTLE_PAK packet = new BATTLE_READYBATTLE_PAK(this))
                {
                    byte[] data = packet.GetCompleteBytes("Room.StartBattle");
                    List<Account> list = GetAllPlayers(SlotStateEnum.READY, 0);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Account player = list[i];
                        Slot slot = GetSlot(player.slotId);
                        if (slot != null)
                        {
                            slot.withHost = true;
                            slot.state = SlotStateEnum.LOAD;
                            slot.SetMissionsClone(player.missions);
                            player.SendCompletePacket(data);
                            player.SetCuponsFlags();
                        }
                    }
                }
                if (updateInfo)
                {
                    UpdateSlotsInfo();
                }
            }
        }
        public void StartCountDown()
        {
            SendPacketToPlayers(PackageDataManager.BATTLE_COUNTDOWN_START_PAK);
            countdown.StartJob(5250, (callbackState) =>
            {
                if (slots[leaderSlot].state == SlotStateEnum.READY && state == RoomStateEnum.CountDown)
                {
                    StartBattle(true);
                }
                else
                {
                    List<Account> list = GetAllPlayers(SlotStateEnum.READY, 0);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Account player = list[i];
                        Slot slot = GetSlot(player.slotId);
                        if (player != null && slot != null && slot.state == SlotStateEnum.READY)
                        {
                            player.SendCompletePacket(PackageDataManager.BATTLE_READY_ERROR_0x80001008_PAK);
                        }
                    }
                }
                lock (callbackState)
                {
                    countdown.Timer = null;
                }
            });
        }
        /// <summary>
        /// Desativa a contagem regressiva e envia um pacote com o cancelamento da contagem.
        /// </summary>
        /// <param name="motive"></param>
        /// <param name="refreshRoom">Atualizar infos da sala?</param>
        public void StopCountDown(CountDownEnum motive, bool refreshRoom = true)
        {
            state = RoomStateEnum.Ready;
            countdown.Timer = null;
            if (refreshRoom)
            {
                UpdateRoomInfo();
            }
            using (BATTLE_COUNTDOWN_PAK packet = new BATTLE_COUNTDOWN_PAK(motive))
            {
                SendPacketToPlayers(packet);
            }
        }

        /// <summary>
        /// Cancela a contagem regressiva se o slot do jogador for o dono da sala, ou se não houver jogadores no time oposto ao do dono da sala.
        /// </summary>
        /// <param name="slotId">Slot do jogador</param>
        public void StopCountDown(int slotId)
        {
            if (state != RoomStateEnum.CountDown)
            {
                return;
            }
            if (slotId == leaderSlot)
            {
                StopCountDown(CountDownEnum.StopByHost);
            }
            else if (GetPlayingPlayers(leaderSlot % 2 == 0 ? 1 : 0, SlotStateEnum.READY, 0) == 0)
            {
                ChangeSlotState(leaderSlot, SlotStateEnum.NORMAL, false);
                StopCountDown(CountDownEnum.StopByPlayer);
            }
        }

        /// <summary>
        /// Calcula os resultados da partida. É possível definir o time vencedor e se a partida é contra I.A.
        /// </summary>
        /// <param name="winnerTeam">Time vencedor</param>
        /// <param name="isBotMode">Partida contra I.A?</param>
        public void CalculateResult(TeamResultTypeEnum winnerTeam, bool isBotMode)
        {
            lock (slots)
            {
                try
                {
                    state = RoomStateEnum.BattleEnd;
                    EventUpModel eventRankUp = EventRankUpSyncer.GetRunningEvent();
                    EventMapModel eventMap = EventMapSyncer.GetRunningEvent();
                    bool mapEvUse = EventMapSyncer.EventIsValid(eventMap, mapId, (byte)mode);
                    PlayTimeModel eventPlayTime = EventPlayTimeSyncer.GetRunningEvent();
                    DateTime finishDate = DateTime.Now;
                    for (int i = 0; i < 16; i++)
                    {
                        Slot slot = slots[i];
                        if (!slot.check && slot.state == SlotStateEnum.BATTLE && GetPlayerBySlot(slot, out Account player))
                        {
                            slot.check = true;
                            FirewallSecurity.RemoveRuleUdp(player.client.GetIPAddress(), sessionPort);
                            using (DBQuery query = new DBQuery())
                            {
                                double inBattleTime = slot.InBattleTime(finishDate);
                                int PreviousGOLD = player.gold, PreviousEXP = player.exp, PreviousCASH = player.cash;
                                if (!isBotMode)
                                {
                                    if (Settings.MissionActive)
                                    {
                                        if (winnerTeam != TeamResultTypeEnum.TeamDraw)
                                        {
                                            MissionCompleteBase(player, slot, slot.teamId == (int)winnerTeam ? MissionTypeEnum.WIN : MissionTypeEnum.DEFEAT, 0);
                                        }
                                        if (slot.missionsCompleted)
                                        {
                                            player.missions = slot.missions;
                                            player.UpdateMissionContent();
                                        }
                                        player.GenerateMissionAwards(query);
                                    }
                                    int timePlayed = slot.allKills == 0 && slot.allDeaths == 0 ? (int)(inBattleTime / 3) : (int)inBattleTime;
                                    if (mode == RoomTypeEnum.Destruction || mode == RoomTypeEnum.Suppression)
                                    {
                                        slot.exp = (int)(slot.score + (timePlayed / 2.5) + (slot.allDeaths * 3.2) + (slot.objetivos * 80)) * 3;
                                        slot.gold = (int)(slot.score + (timePlayed / 3.0) + (slot.allDeaths * 3.2) + (slot.objetivos * 80)) * 3;
                                        slot.cash = (int)((slot.score / 2) + (timePlayed / 6.5) + (slot.allDeaths * 1.9) + (slot.objetivos * 40)) * 3;
                                    }
                                    else
                                    {
                                        slot.exp = (int)(slot.score + (timePlayed / 2.5) + (slot.allDeaths * 2.8) + (slot.objetivos * 80)) * 3;
                                        slot.gold = (int)(slot.score + (timePlayed / 3.0) + (slot.allDeaths * 2.8) + (slot.objetivos * 80)) * 3;
                                        slot.cash = (int)((slot.score / 1.5) + (timePlayed / 4.5) + (slot.allDeaths * 1.5) + (slot.objetivos * 40)) * 3;
                                    }
                                    bool WonTheMatch = slot.teamId == (int)winnerTeam;
                                    if (mode != RoomTypeEnum.Chaos && mode != RoomTypeEnum.HeadHunter)
                                    {
                                        player.statistics.headshots += slot.headshots;
                                        player.statistics.kills += slot.allKills;
                                        player.statistics.totalkills += slot.allKills;
                                        player.statistics.deaths += slot.allDeaths;
                                        AddKDInfosToQuery(slot, player.statistics, query);
                                        UpdateMatchCount(WonTheMatch, player, (int)winnerTeam, query);
                                    }
                                    if (WonTheMatch)
                                    {
                                        slot.gold += Percentage(slot.gold, 15);
                                        slot.exp += Percentage(slot.exp, 20);
                                    }
                                    if (slot.earnedXP > 0)
                                    {
                                        slot.exp += slot.earnedXP * 5;
                                    }
                                    if (slot.allKills >= 80)
                                    {
                                        Logger.Analyze($" ANALYZE: (Suspected Free Kill or Hack) Code: Kills >= 80 PlayerId: {slot.playerId} K/D: {slot.allKills}/{slot.allDeaths} HS: {slot.headshots}");
                                    }
                                    else if (slot.allKills >= 30 && slot.allDeaths <= 5)
                                    {
                                        Logger.Analyze($" ANALYZE: (Suspected Free Kill or Hack) Code: Kills: >= 50 e Deaths <= 5 PlayerId: {slot.playerId} K/D: {slot.allKills}/{slot.allDeaths} HS: {slot.headshots}");
                                    }
                                    else if (slot.allKills <= 5 && slot.allDeaths >= 30)
                                    {
                                        Logger.Analyze($" ANALYZE: (Suspected account used for free kill) Code: Kills <= 5 e Deaths >= 30 PlayerId: {slot.playerId} K/D: {slot.allKills}/{slot.allDeaths} HS: {slot.headshots}");
                                    }
                                    else if (slot.allDeaths >= 50 && slot.allDeaths <= 30)
                                    {
                                        Logger.Analyze($" ANALYZE: (Suspected account used for free kill) Code: Deaths >= 50 PlayerId: {slot.playerId} K/D: {slot.allKills}/{slot.allDeaths} HS: {slot.headshots}");
                                    }
                                }
                                else
                                {
                                    int level = IngameAiLevel * (150 + slot.allDeaths);
                                    if (level == 0)
                                    {
                                        level++;
                                    }
                                    int reward = slot.score / level;
                                    slot.gold += reward;
                                    slot.exp += reward;
                                    if (slot.allKills >= 250)
                                    {
                                        Logger.Analyze($" ANALYZE: (SPECIAL MODE: Suspected) PlayerId: {slot.playerId} K/D: {slot.allKills}/{slot.allDeaths} HS: {slot.headshots} Score: {slot.score} Level: {level}");
                                    }
                                }
                                slot.exp = slot.exp > Settings.MaxBattleExp ? Settings.MaxBattleExp : slot.exp;
                                slot.gold = slot.gold > Settings.MaxBattleGold ? Settings.MaxBattleGold : slot.gold;
                                slot.cash = slot.cash > Settings.MaxBattleCash ? Settings.MaxBattleCash : slot.cash;
                                if (slot.exp < 0 || slot.gold < 0 || slot.cash < 0)
                                {
                                    Logger.Error($" [Room] [BaseResultGame] Exp: {slot.exp} Gold: {slot.gold} Cash: {slot.cash} K/D: {slot.allKills}/{slot.allDeaths} HeadShots: {slot.headshots} InBattleTime: {inBattleTime} Objetivos: {slot.objetivos} IsBotMode: {isBotMode} Start: {slot.startTime} Finish: {finishDate}");
                                    slot.exp = 105;
                                    slot.gold = 90;
                                    slot.cash = 20;
                                }
                                int PorcentageEXP = 0, PorcentageGold = 0, PorcentageCash = 0;
                                if (eventRankUp != null || mapEvUse)
                                {
                                    if (eventRankUp != null)
                                    {
                                        PorcentageEXP += eventRankUp.percentXp;
                                        PorcentageGold += eventRankUp.percentGp;
                                    }
                                    if (mapEvUse)
                                    {
                                        PorcentageEXP += eventMap.percentXp;
                                        PorcentageGold += eventMap.percentGp;
                                    }
                                    if (!slot.bonusFlags.HasFlag(ResultIcon.Event))
                                    {
                                        slot.bonusFlags |= ResultIcon.Event;
                                    }
                                }
                                PlayerBonus playerBonus = player.bonus;
                                if (playerBonus != null && playerBonus.bonuses > 0)
                                {
                                    if ((playerBonus.bonuses & 8) == 8)
                                    {
                                        PorcentageEXP += 100;
                                    }
                                    if ((playerBonus.bonuses & 128) == 128)
                                    {
                                        PorcentageGold += 100;
                                    }
                                    if ((playerBonus.bonuses & 4) == 4)
                                    {
                                        PorcentageEXP += 50;
                                    }
                                    if ((playerBonus.bonuses & 64) == 64)
                                    {
                                        PorcentageGold += 50;
                                    }
                                    if ((playerBonus.bonuses & 2) == 2)
                                    {
                                        PorcentageEXP += 30;
                                    }
                                    if ((playerBonus.bonuses & 32) == 32)
                                    {
                                        PorcentageGold += 30;
                                    }
                                    if ((playerBonus.bonuses & 1) == 1)
                                    {
                                        PorcentageEXP += 10;
                                    }
                                    if ((playerBonus.bonuses & 16) == 16)
                                    {
                                        PorcentageGold += 10;
                                    }
                                    if (!slot.bonusFlags.HasFlag(ResultIcon.Item))
                                    {
                                        slot.bonusFlags |= ResultIcon.Item;
                                    }
                                }
                                if (player.pccafe == 2 || player.pccafe == 1)
                                {
                                    PorcentageEXP += player.pccafe == 2 ? Settings.PCCAFEPlusPorcentageExp : Settings.PCCAFEBasicPorcentageExp;
                                    PorcentageGold += player.pccafe == 2 ? Settings.PCCAFEPlusPorcentageGold : Settings.PCCAFEBasicPorcentageGold;
                                    PorcentageCash += player.pccafe == 2 ? Settings.PCCAFEPlusPorcentageCash : Settings.PCCAFEBasicPorcentageCash;
                                    if (player.pccafe == 1 && !slot.bonusFlags.HasFlag(ResultIcon.Pc))
                                    {
                                        slot.bonusFlags |= ResultIcon.Pc;
                                    }
                                    else if (player.pccafe == 2 && !slot.bonusFlags.HasFlag(ResultIcon.PcPlus))
                                    {
                                        slot.bonusFlags |= ResultIcon.PcPlus;
                                    }
                                }
                                slot.bonusEXP = Percentage(slot.exp, PorcentageEXP);
                                slot.bonusGold = Percentage(slot.gold, PorcentageGold);
                                slot.bonusCash = Percentage(slot.cash, PorcentageGold);

                                if ((player.gold + slot.gold + slot.bonusGold) <= 999999999)
                                {
                                    player.gold += slot.gold + slot.bonusGold;
                                }
                                if ((player.exp + slot.exp + slot.bonusEXP) <= 999999999)
                                {
                                    player.exp += slot.exp + slot.bonusEXP;
                                }
                                if (Settings.BattleWinCashActive && (player.cash + slot.cash + slot.bonusCash) <= 999999999)
                                {
                                    player.cash += slot.cash + slot.bonusCash;
                                    player.cashReceivedLastMatch = slot.cash + slot.bonusCash;
                                }
                                RankModel MyRank = RankManager.GetRank(player.rankId);
                                long playerIdReduceRank = 0;
                                if ((MyRank != null && player.rankId <= 51 && player.exp >= (MyRank.onNextLevel + MyRank.onAllExp) && player.rankId <= 45) || (player.exp >= (MyRank.onNextLevel + MyRank.onAllExp) && player.rankId >= 46 && RankManager.CheckUp(player.playerId, player.rankId, player.exp, out playerIdReduceRank)))
                                {
                                    player.rankId++;
                                    int itemIdToRemove = 0;
                                    if (player.rankId == 8)
                                    {
                                        itemIdToRemove = 1301268000;
                                    }
                                    else if (player.rankId == 12)
                                    {
                                        itemIdToRemove = 1301271000;
                                    }
                                    else if (player.rankId == 14)
                                    {
                                        itemIdToRemove = 1301272000;
                                    }
                                    else if (player.rankId == 17)
                                    {
                                        itemIdToRemove = 1301276000;
                                    }
                                    else if (player.rankId == 26)
                                    {
                                        itemIdToRemove = 1302040000;
                                    }
                                    else if (player.rankId == 31)
                                    {
                                        itemIdToRemove = 1302041000;
                                    }
                                    else if (player.rankId == 36)
                                    {
                                        itemIdToRemove = 1302042000;
                                    }
                                    else if (player.rankId == 41)
                                    {
                                        itemIdToRemove = 1302043000;
                                    }
                                    if (itemIdToRemove != 0)
                                    {
                                        ItemsModel item = player.inventory.GetItem(itemIdToRemove);
                                        if (item != null)
                                        {
                                            if (player.DeleteItem(item.objectId))
                                            {
                                                player.inventory.RemoveItem(item);
                                            }
                                            player.SendPacket(new INVENTORY_ITEM_EXCLUDE_PAK(1, item.objectId));
                                        }
                                    }
                                    List<ItemsModel> items = RankManager.GetAwards(player.rankId);
                                    if (items.Count > 0)
                                    {
                                        player.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, items));
                                    }
                                    RankModel NextRank = RankManager.GetRank(player.rankId);
                                    if (NextRank != null && player.rankId < 47)
                                    {
                                        int Experience = player.exp - MyRank.onNextLevel - MyRank.onAllExp + NextRank.onAllExp;
                                        player.exp = Experience;
                                    }
                                    if ((player.gold + MyRank.onGoldUp) <= 999999999)
                                    {
                                        player.gold += MyRank.onGoldUp;
                                    }
                                    player.lastRankUpDate = uint.Parse(finishDate.ToString("yyMMddHHmm"));
                                    player.SendPacket(new PROTOCOL_BASE_RANK_UP_ACK(player.rankId, MyRank.onNextLevel));
                                    query.AddQuery("last_rankup", (long)player.lastRankUpDate);
                                    query.AddQuery("rank", player.rankId);

                                    Account playerReduceRank = AccountManager.GetAccount(playerIdReduceRank, 0);
                                    if (player != null)
                                    {
                                        playerReduceRank.rankId--;
                                        player.SendPacket(new PROTOCOL_BASE_RANK_UP_ACK(playerReduceRank.rankId, player.exp));
                                    }
                                }
                                if (eventPlayTime != null)
                                {
                                    player.PlayTimeEvent((long)inBattleTime, eventPlayTime, isBotMode);
                                }
                                player.DailyCashEvent((long)inBattleTime, isBotMode);
                                player.DiscountPlayerItems(slot);
                                if (PreviousGOLD != player.gold)
                                {
                                    query.AddQuery("gold", player.gold);
                                }
                                if (PreviousEXP != player.exp)
                                {
                                    query.AddQuery("exp", player.exp);
                                }
                                if (PreviousCASH != player.cash)
                                {
                                    query.AddQuery("cash", player.cash);
                                }
                                Utilities.UpdateDB("accounts", "id", player.playerId, query.GetTables(), query.GetValues());
                            }
                        }
                    }
                    UpdateSlotsInfo();
                    CalculateClanMatchResult((int)winnerTeam);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
        }

        private void AddKDInfosToQuery(Slot slot, PlayerStats stats, DBQuery query)
        {
            if (slot.allKills > 0)
            {
                query.AddQuery("kills", stats.kills);
                query.AddQuery("all_kills", stats.totalkills);
            }
            if (slot.allDeaths > 0)
            {
                query.AddQuery("deaths", stats.deaths);
            }
            if (slot.headshots > 0)
            {
                query.AddQuery("headshots", stats.headshots);
            }
        }
        private void CalculateClanMatchResult(int winnerTeam)
        {
            lock (slots)
            {
                if (channelType != 4 || blockedClan)
                {
                    return;
                }
                SortedList<int, Clan> list = new SortedList<int, Clan>();
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = slots[i];
                    if (slot.state == SlotStateEnum.BATTLE && GetPlayerBySlot(slot, out Account player))
                    {
                        Clan clan = ClanManager.GetClan(player.clanId);
                        if (clan.id == 0)
                        {
                            continue;
                        }
                        bool WonTheMatch = slot.teamId == winnerTeam;
                        clan.exp += slot.exp;
                        clan.BestPlayers.SetBestExp(slot);
                        clan.BestPlayers.SetBestKills(slot);
                        clan.BestPlayers.SetBestHeadshot(slot);
                        clan.BestPlayers.SetBestWins(player.statistics, slot, WonTheMatch);
                        clan.BestPlayers.SetBestParticipation(player.statistics, slot);
                        if (!list.ContainsKey(player.clanId))
                        {
                            list.Add(player.clanId, clan);
                            if (winnerTeam != 2)
                            {
                                CalculateSpecialCM(clan, winnerTeam, slot.teamId);
                                if (WonTheMatch)
                                {
                                    clan.vitorias++;
                                }
                                else
                                {
                                    clan.derrotas++;
                                }
                            }
                            clan.partidas++;
                            clan.UpdateBattles();
                        }
                    }
                }
                foreach (Clan clan in list.Values)
                {
                    clan.UpdateExp();
                    if (modeSpecial == RoomModeSpecial.CLAN_MATCH)
                    {
                        clan.UpdatePoints();
                    }
                    clan.UpdateBestPlayers();
                    RankModel rankModel = ClanRankXML.GetRank(clan.rank);
                    if (rankModel != null && clan.exp >= rankModel.onNextLevel + rankModel.onAllExp)
                    {
                        clan.rank++;
                        clan.UpdateRank();
                    }
                }
            }
        }
        private void CalculateSpecialCM(Clan clan, int winnerTeam, int teamIdx)
        {
            if (modeSpecial != RoomModeSpecial.CLAN_MATCH || winnerTeam == 2)
            {
                return;
            }
            if (winnerTeam == teamIdx)
            {
                float morePoints = mode == RoomTypeEnum.DeathMatch ? (teamIdx == 0 ? redKills : blueKills) / 20 : teamIdx == 0 ? redRounds : blueRounds;
                float POINTS = 25 + morePoints;
                clan.pontos += POINTS;
                Logger.Room($" [ClanFronto] ClanId: {clan.id} Earned Points: {POINTS} Final Points: {clan.pontos}");
            }
            else
            {
                if (clan.pontos == 0)
                {
                    Logger.Room($" [ClanFronto] O Clã Id: {clan.id} não perdeu Pontos devido a baixa pontuação.");
                    return;
                }
                float morePoints = mode == RoomTypeEnum.DeathMatch ? (teamIdx == 0 ? redKills : blueKills) / 20 : teamIdx == 0 ? redRounds : blueRounds;
                float POINTS = 40 - morePoints;
                clan.pontos -= POINTS;
                Logger.Room($" [ClanFronto] ClanId: {clan.id} Losed Points: {POINTS} Final Points: {clan.pontos}");
            }
        }
        /// <summary>
        /// Atualiza as informações da sala, para todos os jogadores.
        /// </summary>
        public void UpdateRoomInfo()
        {
            GenerateRoomSeed();
            using (PROTOCOL_ROOM_CHANGE_INFO_ACK packet = new PROTOCOL_ROOM_CHANGE_INFO_ACK(this))
            {
                SendPacketToPlayers(packet);
            }
        }
        public void InitSlotCount(int count)
        {
            if (stage4vs4 == 1)
            {
                count = 8;
            }
            if (count <= 0)
            {
                count = 1;
            }
            for (int slotId = 0; slotId < slots.Length; slotId++)
            {
                if (slotId >= count)
                {
                    slots[slotId].state = SlotStateEnum.CLOSE;
                }
            }
        }
        public byte GetSlotCount()
        {
            lock (slots)
            {
                byte count = 0;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].state != SlotStateEnum.CLOSE)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        /// <summary>
        /// Procura um novo slot vazio para fazer a troca do slot da conta.
        /// </summary>
        /// <param name="slots">Lista de mudanças</param>
        /// <param name="p">Conta</param>
        /// <param name="old">Slot antigo</param>
        /// <param name="teamIdx">Time</param>
        public void SwitchNewSlot(List<SlotChange> slots, Account p, Slot old, int teamIdx)
        {
            int[] teamArray = GetTeamArray(teamIdx);
            for (int i = 0; i < teamArray.Length; i++)
            {
                int index = teamArray[i];
                Slot newSlot = this.slots[index];
                if (newSlot.playerId == 0 && newSlot.state == SlotStateEnum.EMPTY)
                {
                    newSlot.ResetSlot();
                    newSlot.state = SlotStateEnum.NORMAL;
                    newSlot.playerId = p.playerId;
                    newSlot.equipment = p.equipments;

                    newSlot.ResetSlot();
                    old.state = SlotStateEnum.EMPTY;
                    old.playerId = 0;
                    old.equipment = null;

                    if (p.slotId == leaderSlot)
                    {
                        leaderName = p.nickname;
                        leaderSlot = index;
                    }
                    p.slotId = index;
                    slots.Add(new SlotChange(old, newSlot));
                    break;
                }
            }
        }
        /// <summary>
        /// Troca todas as informações de 2 slots.
        /// </summary>
        /// <param name="changeSlots">Lista de mudanças</param>
        /// <param name="newSlotId">Novo slot</param>
        /// <param name="oldSlotId">Antigo slot</param>
        /// <param name="changeReady">Caso estado estiver como Ready, mudar para Normal?</param>
        /// <returns></returns>
        public void SwitchSlots(List<SlotChange> changeSlots, int newSlotId, int oldSlotId, bool changeReady)
        {
            Slot newSLOT = slots[newSlotId];
            Slot oldSLOT = slots[oldSlotId];
            if (changeReady)
            {
                if (newSLOT.state == SlotStateEnum.READY)
                {
                    newSLOT.state = SlotStateEnum.NORMAL;
                }
                if (oldSLOT.state == SlotStateEnum.READY)
                {
                    oldSLOT.state = SlotStateEnum.NORMAL;
                }
            }
            newSLOT.SetSlotId(oldSlotId);
            oldSLOT.SetSlotId(newSlotId);
            slots[newSlotId] = oldSLOT;
            slots[oldSlotId] = newSLOT;
            changeSlots.Add(new SlotChange(oldSLOT, newSLOT));
        }
        /// <summary>
        /// Troca o estado do slot do jogador.
        /// <para>Se for Empty/Close, ele reseta as informações do slot.</para>
        /// <para>Nenhum código é executado se o estado já for o informado.</para>
        /// </summary>
        /// <param name="slot">Slot</param>
        /// <param name="state">Novo estado</param>
        /// <param name="sendInfo">Atualizar informações para todos os outros jogadores?</param>
        public void ChangeSlotState(int slotId, SlotStateEnum state, bool sendInfo)
        {
            Slot slot = GetSlot(slotId);
            ChangeSlotState(slot, state, sendInfo);
        }
        /// <summary>
        /// Troca o estado do slot do jogador.
        /// <para>Se for Empty/Close, ele reseta as informações do slot.</para>
        /// <para>Nenhum código é executado se o estado já for o informado.</para>
        /// </summary>
        /// <param name="slot">Slot</param>
        /// <param name="state">Novo estado</param>
        /// <param name="sendInfo">Atualizar informações para todos os outros jogadores?</param>
        public void ChangeSlotState(Slot slot, SlotStateEnum state, bool sendInfo)
        {
            if (slot == null || slot.state == state)
            {
                return;
            }
            slot.state = state;
            if (state == SlotStateEnum.EMPTY || state == SlotStateEnum.CLOSE)
            {
                ResetSlotInfo(slot, false);
                slot.playerId = 0;
            }
            if (sendInfo)
            {
                UpdateSlotsInfo();
            }
        }
        public Account GetPlayerBySlot(Slot slot)
        {
            try
            {
                long id = slot.playerId;
                return id > 0 ? AccountManager.GetAccount(id, true) : null;
            }
            catch
            {
                return null;
            }
        }
        public Account GetPlayerBySlot(int slotId)
        {
            try
            {
                long id = slots[slotId].playerId;
                return id > 0 ? AccountManager.GetAccount(id, true) : null;
            }
            catch
            {
                return null;
            }
        }
        public bool GetPlayerBySlot(int slotId, out Account player)
        {
            try
            {
                long id = slots[slotId].playerId;
                player = id > 0 ? AccountManager.GetAccount(id, true) : null;
                return player != null;
            }
            catch
            {
                player = null;
                return false;
            }
        }
        public bool GetPlayerBySlot(Slot slot, out Account player)
        {
            try
            {
                long id = slot.playerId;
                player = id > 0 ? AccountManager.GetAccount(id, true) : null;
                return player != null;
            }
            catch
            {
                player = null;
                return false;
            }
        }
        /// <summary>
        /// Atualiza as informações dos slots da sala, para todos os jogadores.
        /// </summary>
        public void UpdateSlotsInfo()
        {
            using (PROTOCOL_ROOM_GET_SLOTINFO_ACK packet = new PROTOCOL_ROOM_GET_SLOTINFO_ACK(this))
            {
                SendPacketToPlayers(packet);
            }
        }
        /// <summary>
        /// Pega as informações da conta do dono da sala. Caso a quantidade de jogadores for 0, é retornado FALSO. Se não houver líder, será feita uma tentativa de troca de dono.
        /// </summary>
        /// <param name="player">Conta do dono da sala</param>
        /// <returns></returns>
        public bool GetLeader(out Account player)
        {
            player = null;
            if (GetAllPlayersCount() < 1)
            {
                return false;
            }
            if (leaderSlot == -1)
            {
                SetNewLeader(-1, 0, -1, false);
            }
            if (leaderSlot >= 0)
            {
                player = AccountManager.GetAccount(GetSlot(leaderSlot).playerId, true);
            }
            return player != null;
        }
        /// <summary>
        /// Pega as informações da conta do dono da sala. Caso a quantidade de jogadores for 0, é retornado FALSO. Se não houver líder, será feita uma tentativa de troca de dono.
        /// </summary>
        /// <returns></returns>
        public Account GetLeader()
        {
            if (GetAllPlayersCount() < 1)
            {
                return null;
            }
            if (leaderSlot == -1)
            {
                SetNewLeader(-1, 0, -1, false);
            }
            return leaderSlot == -1 ? null : AccountManager.GetAccount(GetSlot(leaderSlot).playerId, true);
        }
        /// <summary>
        /// Indica um novo líder da sala. Seguindo alguns paramêtros. Se o novo dono estiver com PRONTO, ele será retirado automaticamente.
        /// </summary>
        /// <param name="leader">Slot do líder. (-1 = Seleção aleatória)</param>
        /// <param name="state">Indicar um novo através do estado do slot do jogador. (O estado tem que ser maior do que o indicado)</param>
        /// <param name="oldLeader">Ignorar um certo slot. (-1 = Nenhum)</param>
        /// <param name="updateInfo">Utilizar o recurso de atualizar slot das salas?</param>
        public void SetNewLeader(int leader, int state, int oldLeader, bool updateInfo)
        {
            lock (slots)
            {
                if (leader == -1)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        Slot slot = slots[i];
                        if (i != oldLeader && slot.playerId > 0 && (int)slot.state > state)
                        {
                            leaderSlot = i;
                            break;
                        }
                    }
                }
                else
                {
                    leaderSlot = leader;
                }
                if (leaderSlot != -1)
                {
                    Slot slot = slots[leaderSlot];
                    if (slot.state == SlotStateEnum.READY)
                    {
                        slot.state = SlotStateEnum.NORMAL;
                    }               
                    if (updateInfo)
                    {
                        UpdateSlotsInfo();
                    }
                }
            }
        }
        public void SendPacketToPlayers(GamePacketWriter packet)
        {
            List<Account> players = GetAllPlayers();
            if (players.Count == 0)
            {
                return;
            }
            byte[] data = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket)");
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(data);
            }
        }
        public int SendMessageToPlayers(GamePacketWriter packet)
        {
            List<Account> players = GetAllPlayers();
            if (players.Count == 0)
            {
                return 0;
            }
            byte[] data = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket)");
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(data);
            }
            return players.Count;
        }
        public void SendPacketToPlayers(byte[] data)
        {
            List<Account> players = GetAllPlayers();
            if (players.Count == 0)
            {
                return;
            }
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(data);
            }
        }
        public void SendPacketToPlayers(GamePacketWriter packet, long player_id)
        {
            List<Account> players = GetAllPlayers(player_id);
            if (players.Count == 0)
            {
                return;
            }
            byte[] data = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,long)");
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(data);
            }
        }
        public void SendPacketToPlayers(GamePacketWriter packet, SlotStateEnum state, int type)
        {
            List<Account> players = GetAllPlayers(state, type);
            if (players.Count == 0)
            {
                return;
            }
            byte[] data = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,SLOT_STATE,int)");
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(data);
            }
        }
        public void SendPacketToPlayers(byte[] data, SlotStateEnum state, int type)
        {
            List<Account> players = GetAllPlayers(state, type);
            if (players.Count == 0)
            {
                return;
            }
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(data);
            }
        }
        public void SendPacketToPlayers(GamePacketWriter packet, GamePacketWriter packet2, SlotStateEnum state, int type)
        {
            List<Account> players = GetAllPlayers(state, type);
            if (players.Count == 0)
            {
                return;
            }
            byte[] data = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,SendPacket,SLOT_STATE,int)-1");
            byte[] data2 = packet2.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,SendPacket,SLOT_STATE,int)-2");
            for (int i = 0; i < players.Count; i++)
            {
                Account pR = players[i];
                pR.SendCompletePacket(data);
                pR.SendCompletePacket(data2);
            }
        }
        public void SendPacketToPlayers(GamePacketWriter packet, SlotStateEnum state, int type, int exception)
        {
            List<Account> players = GetAllPlayers(state, type, exception);
            if (players.Count == 0)
            {
                return;
            }
            byte[] data = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,SLOT_STATE,int,int)");
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(data);
            }
        }
        public void SendPacketToPlayers(byte[] data, SlotStateEnum state, int type, int exception)
        {
            List<Account> players = GetAllPlayers(state, type, exception);
            if (players.Count == 0)
            {
                return;
            }
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(data);
            }
        }
        public void SendPacketToPlayers(GamePacketWriter packet, SlotStateEnum state, int type, int exception, int exception2)
        {
            List<Account> players = GetAllPlayers(state, type, exception, exception2);
            if (players.Count == 0)
            {
                return;
            }
            byte[] data = packet.GetCompleteBytes("Room.SendPacketToPlayers(SendPacket,SLOT_STATE,int,int,int)");
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(data);
            }
        }
        public void RemovePlayer(Account player, bool WarnAllPlayers, int quitMotive = 0)
        {
            if (player == null || !GetSlot(player.slotId, out Slot slot))
            {
                return;
            }
            BaseRemovePlayer(player, slot, WarnAllPlayers, quitMotive);
        }
        public void RemovePlayer(Account player, Slot slot, bool WarnAllPlayers, int quitMotive = 0)
        {
            if (player == null || slot == null)
            {
                return;
            }
            BaseRemovePlayer(player, slot, WarnAllPlayers, quitMotive);
        }
        /// <summary>
        /// Remove um jogador da sala.
        /// </summary>
        /// <param name="player">Conta do jogador</param>
        /// <param name="slot">Slot do jogador</param>
        /// <param name="WarnAllPlayers">Enviar aviso de saída para o jogador também?</param>
        /// <param name="quitMotive">Motivo da saída. 0 = Normal; 1 = GM; 2 = Votação</param>
        private void BaseRemovePlayer(Account player, Slot slot, bool WarnAllPlayers, int quitMotive)
        {
            lock (slots)
            {
                bool useRoomUpdate = false;
                bool hostChanged = false;
                if (player != null && slot != null)
                {
                    player.showBoxWelcome = false;
                    if (slot.state >= SlotStateEnum.LOAD)
                    {
                        if (leaderSlot == slot.Id)
                        {
                            int oldLeader = leaderSlot;
                            int bestState = 1; //Maior que Close
                            if (state == RoomStateEnum.Battle)
                            {
                                bestState = 12; //Maior que BattleReady
                            }
                            else if (state >= RoomStateEnum.Loading)
                            {
                                bestState = 8; //Maior que Ready
                            }
                            if (GetAllPlayers(slot.Id).Count >= 1)
                            {
                                SetNewLeader(-1, bestState, leaderSlot, false);
                            }
                            if (GetPlayingPlayers(2, SlotStateEnum.READY, 1) >= 2)
                            {
                                using (PROTOCOL_BATTLE_GIVEUPBATTLE_ACK packet = new PROTOCOL_BATTLE_GIVEUPBATTLE_ACK(this, oldLeader))
                                {
                                    SendPacketToPlayers(packet, SlotStateEnum.RENDEZVOUS, 1, slot.Id);
                                }
                            }
                            hostChanged = true;
                        }
                        using (PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK packet = new PROTOCOL_BATTLE_LEAVEP2PSERVER_ACK(player, quitMotive))
                        {
                            SendPacketToPlayers(packet, SlotStateEnum.READY, 1, !WarnAllPlayers ? slot.Id : -1);
                        }
                        slot.ResetAllInfos(); //Battle
                        slot.ResetSlot();
                        if (votekick != null)
                        {
                            votekick.TotalArray[slot.Id] = false;
                        }
                    }
                    slot.playerId = 0;
                    slot.equipment = null;
                    slot.state = SlotStateEnum.EMPTY;
                    if (state == RoomStateEnum.CountDown)
                    {
                        if (slot.Id == leaderSlot)
                        {
                            state = RoomStateEnum.Ready;
                            useRoomUpdate = true;
                            countdown.Timer = null;
                            SendPacketToPlayers(PackageDataManager.BATTLE_COUNTDOWN_STOPBYHOST_PAK);
                        }
                        else if (GetPlayingPlayers(slot.teamId, SlotStateEnum.READY, 0) == 0)
                        {
                            if (slot.Id != leaderSlot)
                            {
                                ChangeSlotState(leaderSlot, SlotStateEnum.NORMAL, false);
                            }
                            StopCountDown(CountDownEnum.StopByPlayer, false);
                            useRoomUpdate = true;
                        }
                    }
                    else if (IsPreparing())
                    {
                        BattleEndPlayersCount(IsBotMode());
                        if (state == RoomStateEnum.Battle)
                        {
                            BattleEndRoundPlayersCount();
                        }
                    }
                    CheckToEndWaitingBattle(hostChanged);
                    RequestHost.Remove(player.playerId);
                    if (vote.Timer != null && votekick != null && votekick.victimIdx == player.slotId && quitMotive != 2)
                    {
                        vote.Timer = null;
                        votekick = null;
                        SendPacketToPlayers(PackageDataManager.VOTEKICK_CANCEL_VOTE_PAK, SlotStateEnum.BATTLE, 0);
                    }
                    Match match = player.match;
                    if (match != null && player.matchSlot >= 0)
                    {
                        match.slots[player.matchSlot].state = SlotMatchStateEnum.Normal;
                        using (CLAN_WAR_REGIST_MERCENARY_PAK packet = new CLAN_WAR_REGIST_MERCENARY_PAK(match))
                        {
                            match.SendPacketToPlayers(packet);
                        }
                    }
                    player.room = null;
                    player.slotId = -1;
                    player.status.UpdateRoom(255);
                    player.SyncPlayerToFriends(false);
                    player.SyncPlayerToClanMembers();
                    player.UpdateCacheInfo();
                }
                UpdateSlotsInfo();
                if (useRoomUpdate)
                {
                    UpdateRoomInfo();
                }
            }
        }
        public int AddPlayer(Account player)
        {
            lock (slots)
            {
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = slots[i];
                    if (slot.playerId == 0 && slot.state == SlotStateEnum.EMPTY)
                    {
                        slot.playerId = player.playerId;
                        slot.state = SlotStateEnum.NORMAL;
                        player.room = this;
                        player.slotId = i;
                        slot.equipment = player.equipments;
                        player.status.UpdateRoom((byte)roomId);
                        player.SyncPlayerToClanMembers();
                        player.SyncPlayerToFriends(false);
                        player.UpdateCacheInfo();
                        return i;
                    }
                }
                return -1;
            }
        }
        public int AddPlayer(Account player, int teamIdx)
        {
            int[] array = GetTeamArray(teamIdx);
            lock (slots)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    int SlotIdx = array[i];
                    Slot slot = slots[SlotIdx];
                    if (slot.playerId == 0 && slot.state == SlotStateEnum.EMPTY)
                    {
                        slot.playerId = player.playerId;
                        slot.state = SlotStateEnum.NORMAL;
                        player.room = this;
                        player.slotId = SlotIdx;
                        slot.equipment = player.equipments;
                        player.status.UpdateRoom((byte)roomId);
                        player.SyncPlayerToClanMembers();
                        player.SyncPlayerToFriends(false);
                        player.UpdateCacheInfo();
                        return SlotIdx;
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// Gera uma lista das contas dos jogadores.
        /// </summary>
        /// <param name="state">Estado do slot</param>
        /// <param name="type">Tipo de pesquisa. 0 = Igual; 1 = Maior</param>
        /// <returns></returns>
        public List<Account> GetAllPlayers(SlotStateEnum state, int type)
        {
            List<Account> list = new List<Account>();
            lock (slots)
            {
                for (int i = 0; i < slots.Length; ++i)
                {
                    Slot slot = slots[i];
                    long id = slot.playerId;
                    if (id > 0 && (type == 0 && slot.state == state || type == 1 && slot.state > state))
                    {
                        Account player = AccountManager.GetAccount(id, true);
                        if (player != null && player.slotId != -1)
                        {
                            list.Add(player);
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// Gera uma lista das contas dos jogadores, com uma excessão de slot.
        /// </summary>
        /// <param name="state">Estado do slot</param>
        /// <param name="type">Tipo de pesquisa. 0 = Igual; 1 = Maior</param>
        /// <param name="exception">Slot do jogador</param>
        /// <returns></returns>
        public List<Account> GetAllPlayers(SlotStateEnum state, int type, int exception)
        {
            List<Account> list = new List<Account>();
            lock (slots)
            {
                for (int i = 0; i < slots.Length; ++i)
                {
                    Slot slot = slots[i];
                    long id = slot.playerId;
                    if (id > 0 && i != exception && (type == 0 && slot.state == state || type == 1 && slot.state > state))
                    {
                        Account player = AccountManager.GetAccount(id, true);
                        if (player != null && player.slotId != -1)
                        {
                            list.Add(player);
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// Gera uma lista das contas dos jogadores, com duas excessões de slot.
        /// </summary>
        /// <param name="state">Estado do slot</param>
        /// <param name="type">Tipo de pesquisa. 0 = Igual; 1 = Maior</param>
        /// <param name="exception">Slot do jogador</param>
        /// <param name="exception2">Slot do jogador (2)</param>
        /// <returns></returns>
        public List<Account> GetAllPlayers(SlotStateEnum state, int type, int exception, int exception2)
        {
            List<Account> list = new List<Account>();
            lock (slots)
            {
                for (int i = 0; i < slots.Length; ++i)
                {
                    Slot slot = slots[i];
                    long id = slot.playerId;
                    if (id > 0 && i != exception && i != exception2 && (type == 0 && slot.state == state || type == 1 && slot.state > state))
                    {
                        Account player = AccountManager.GetAccount(id, true);
                        if (player != null && player.slotId != -1)
                        {
                            list.Add(player);
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// Gera uma lista das contas dos jogadores, com uma excessão de slot.
        /// </summary>
        /// <param name="exception">Slot do jogador</param>
        /// <returns></returns>
        public List<Account> GetAllPlayers(int exception)
        {
            List<Account> list = new List<Account>();
            lock (slots)
            {
                for (int i = 0; i < slots.Length; ++i)
                {
                    long id = slots[i].playerId;
                    if (id > 0 && i != exception)
                    {
                        Account player = AccountManager.GetAccount(id, true);
                        if (player != null && player.slotId != -1)
                        {
                            list.Add(player);
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// Gera uma lista das contas dos jogadores, com uma excessão de id.
        /// </summary>
        /// <param name="exception">Id do jogador</param>
        /// <returns></returns>
        public List<Account> GetAllPlayers(long exception)
        {
            List<Account> list = new List<Account>();
            lock (slots)
            {
                for (int i = 0; i < slots.Length; ++i)
                {
                    long id = slots[i].playerId;
                    if (id > 0 && id != exception)
                    {
                        Account player = AccountManager.GetAccount(id, true);
                        if (player != null && player.slotId != -1)
                        {
                            list.Add(player);
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// Gera uma lista das contas dos jogadores.
        /// </summary>
        /// <returns></returns>
        public List<Account> GetAllPlayers()
        {
            List<Account> list = new List<Account>();
            lock (slots)
            {
                for (int i = 0; i < slots.Length; ++i)
                {
                    long id = slots[i].playerId;
                    if (id > 0)
                    {
                        Account player = AccountManager.GetAccount(id, true);
                        if (player != null && player.slotId != -1)
                        {
                            list.Add(player);
                        }
                    }
                }
            }
            return list;
        }
        public byte GetAllPlayersCount()
        {
            byte count = 0;
            lock (slots)
            {
                for (int i = 0; i < slots.Length; ++i)
                {
                    long id = slots[i].playerId;
                    if (id > 0)
                    {
                        Account player = AccountManager.GetAccount(id, true);
                        if (player != null && player.slotId != -1)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }
        /// <summary>
        /// Retorna a quantidade de jogadores, seguindo alguns parâmetros.
        /// </summary>
        /// <param name="team">Time</param>
        /// <param name="state">Estado do slot</param>
        /// <param name="type">Tipo de pesquisa. 0 = Igual; 1 = Maior</param>
        /// <returns></returns>
        public int GetPlayingPlayers(int team, SlotStateEnum state, int type)
        {
            int players = 0;
            lock (slots)
            {
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = slots[i];
                    if (slot.playerId > 0 && ((type == 0 && slot.state == state) || (type == 1 && slot.state > state)) && (team == 2 || slot.teamId == team))
                    {
                        players++;
                    }
                }
            }
            return players;
        }
        /// <summary>
        /// Retorna a quantidade de jogadores, seguindo alguns parâmetros.
        /// </summary>
        /// <param name="team">Time</param>
        /// <param name="state">Estado do slot</param>
        /// <param name="type">Tipo de pesquisa. 0 = Igual; 1 = Maior</param>
        /// <param name="exception">Slot de excessão</param>
        /// <returns></returns>
        public int GetPlayingPlayers(int team, SlotStateEnum state, int type, int exception)
        {
            int players = 0;
            lock (slots)
            {
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = slots[i];
                    if (i != exception && slot.playerId > 0 && ((type == 0 && slot.state == state) || (type == 1 && slot.state > state)) && (team == 2 || slot.teamId == team))
                    {
                        players++;
                    }
                }
            }
            return players;
        }
        /// <summary>
        /// Se inBattle for TRUE pega slots com "state" = BATTLE e "espectador" = FALSE.
        /// <para>Se inBattle for FALSE pega slots com "state" >= RENDEZVOUS.</para>
        /// </summary>
        /// <param name="inBattle">Pegar jogadores em partida?</param>
        /// <param name="RedPlayers">Quantidade (Time vermelho)</param>
        /// <param name="BluePlayers">Quantidade (Time azul)</param>
        public void GetPlayingPlayers(bool inBattle, out int RedPlayers, out int BluePlayers)
        {
            RedPlayers = 0;
            BluePlayers = 0;
            lock (slots)
            {
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = slots[i];
                    if (slot.playerId > 0 && ((inBattle && slot.state == SlotStateEnum.BATTLE && !slot.espectador) || (!inBattle && slot.state >= SlotStateEnum.RENDEZVOUS)))
                    {
                        if (slot.teamId == 0)
                        {
                            RedPlayers++;
                        }
                        else
                        {
                            BluePlayers++;
                        }
                    }
                }
            }
        }
        public void GetPlayingPlayers(bool inBattle, out int RedPlayers, out int BluePlayers, out int RedDeaths, out int BlueDeaths)
        {
            RedPlayers = 0;
            BluePlayers = 0;
            RedDeaths = 0;
            BlueDeaths = 0;
            lock (slots)
            {
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = slots[i];
                    if (slot.deathState.HasFlag(DeadEnum.isDead))
                    {
                        if (slot.teamId == 0)
                        {
                            RedDeaths++;
                        }
                        else
                        {
                            BlueDeaths++;
                        }
                    }
                    if (slot.playerId > 0 && ((inBattle && slot.state == SlotStateEnum.BATTLE && !slot.espectador) || (!inBattle && slot.state >= SlotStateEnum.RENDEZVOUS)))
                    {
                        if (slot.teamId == 0)
                        {
                            RedPlayers++;
                        }
                        else
                        {
                            BluePlayers++;
                        }
                    }
                }
            }
        }
        public void CheckToEndWaitingBattle(bool host)
        {
            if ((state == RoomStateEnum.CountDown || state == RoomStateEnum.Loading || state == RoomStateEnum.Rendezvous) && (host || slots[leaderSlot].state == SlotStateEnum.BATTLE_READY))
            {
                EndBattleNoPoints();
            }
        }
        /// <summary>
        /// Tenta iniciar a partida para todos os jogadores que estiverem com o estado BATTLE_READY. Chama o updateSlotsInfo e updateRoomInfo.
        /// </summary>
        public void SpawnReadyPlayers()
        {
            lock (slots)
            {
                BaseSpawnReadyPlayers(IsBotMode());
            }
        }
        /// <summary>
        /// Tenta iniciar a partida para todos os jogadores que estiverem com o estado BATTLE_READY. Chama o updateSlotsInfo e updateRoomInfo.
        /// </summary>
        public void SpawnReadyPlayers(bool isBotMode)
        {
            lock (slots)
            {
                BaseSpawnReadyPlayers(isBotMode);
            }
        }
        private void BaseSpawnReadyPlayers(bool isBotMode)
        {
            DateTime date = DateTime.Now;
            for (int i = 0; i < 16; i++)
            {
                Slot slot = slots[i];
                if (slot.state == SlotStateEnum.BATTLE_READY && slot.isPlaying == 0 && slot.playerId > 0)
                {
                    slot.isPlaying = 1;
                    slot.startTime = date;
                    slot.state = SlotStateEnum.BATTLE;
                    if (state == RoomStateEnum.Battle && (mode == RoomTypeEnum.Destruction || mode == RoomTypeEnum.Suppression))
                    {
                        slot.espectador = true;
                    }
                }
            }
            UpdateSlotsInfo();
            List<int> dinos = GetDinossaurs(false, -1);
            if (state == RoomStateEnum.PreBattle)
            {
                BattleStart = mode == RoomTypeEnum.Dino || mode == RoomTypeEnum.CrossCounter ? date.AddMinutes(5) : date;
                SetSpecialStage();
            }
            bool dinoStart = false;
            using (BATTLE_ROUND_RESTART_PAK packet = new BATTLE_ROUND_RESTART_PAK(this, dinos, isBotMode))
            using (PROTOCOL_BATTLE_TIMERSYNC_ACK packet2 = new PROTOCOL_BATTLE_TIMERSYNC_ACK(this))
            using (BATTLE_RECORD_PAK packet3 = new BATTLE_RECORD_PAK(this))
            {
                byte[] data = packet.GetCompleteBytes("Room.BaseSpawnReadyPlayers-1");
                byte[] data2 = packet2.GetCompleteBytes("Room.BaseSpawnReadyPlayers-2");
                byte[] data3 = packet3.GetCompleteBytes("Room.BaseSpawnReadyPlayers-3");
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = slots[i];
                    if (slot.state == SlotStateEnum.BATTLE && slot.isPlaying == 1 && GetPlayerBySlot(slot, out Account player))
                    {
                        slot.isPlaying = 2;
                        if (state == RoomStateEnum.PreBattle)
                        {
                            using (BATTLE_STARTBATTLE_PAK packet4 = new BATTLE_STARTBATTLE_PAK(slot, player, dinos, isBotMode, true))
                            {
                                SendPacketToPlayers(packet4, SlotStateEnum.READY, 1);
                            }
                            player.SendCompletePacket(data);
                            if (mode == RoomTypeEnum.Dino || mode == RoomTypeEnum.CrossCounter)
                            {
                                dinoStart = true;
                            }
                            else
                            {
                                player.SendCompletePacket(data2);
                            }
                        }
                        else if (state == RoomStateEnum.Battle)
                        {
                            using (BATTLE_STARTBATTLE_PAK packet4 = new BATTLE_STARTBATTLE_PAK(slot, player, dinos, isBotMode, false))
                            {
                                SendPacketToPlayers(packet4, SlotStateEnum.READY, 1);
                            }
                            if (mode == RoomTypeEnum.Destruction || mode == RoomTypeEnum.Suppression)
                            {
                                UpdatePlayerInfoInBattle(slot, 0, 1);
                            }
                            else
                            {
                                player.SendCompletePacket(data);
                            }
                            player.SendCompletePacket(data2);
                            player.SendCompletePacket(data3);
                        }
                    }
                }
            }
            if (state == RoomStateEnum.PreBattle)
            {
                state = RoomStateEnum.Battle;
                UpdateRoomInfo();
            }
            if (dinoStart)
            {
                StartDinoRound();
            }
        }
        private void StartDinoRound()
        {
            round.StartJob(5250, (callbackState) =>
            {
                if (state == RoomStateEnum.Battle)
                {
                    using (PROTOCOL_BATTLE_TIMERSYNC_ACK packet = new PROTOCOL_BATTLE_TIMERSYNC_ACK(this))
                    {
                        SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                    }
                    swapRound = false;
                }
                lock (callbackState)
                {
                    round.Timer = null;
                }
            });
        }

        public void ResetBattleInfo()
        {
            LogRoomResult();
            for (int i = 0; i < 16; i++)
            {
                Slot slot = slots[i];
                if (slot.state >= SlotStateEnum.LOAD)
                {
                    slot.state = SlotStateEnum.NORMAL;
                    slot.ResetSlot();
                }
            }
            blockedClan = false;
            rounds = 1;
            spawnsCount = 0;
            redKills = 0;
            redDeaths = 0;
            blueKills = 0;
            blueDeaths = 0;
            redDino = 0;
            blueDino = 0;
            redRounds = 0;
            blueRounds = 0;
            BattleStart = new DateTime();
            timeRoom = 0;
            Bar1 = 0;
            Bar2 = 0;
            swapRound = false;
            IngameAiLevel = 0;
            state = RoomStateEnum.Ready;
            UpdateRoomInfo();
            votekick = null;
            if (round.Timer != null)
            {
                round.Timer = null;
            }
            if (vote.Timer != null)
            {
                vote.Timer = null;
            }
            if (bomba.Timer != null)
            {
                bomba.Timer = null;
            }
            UpdateSlotsInfo();
        }

        public void VotekickResult()
        {
            if (votekick != null)
            {
                int Count = votekick.GetInGamePlayers();
                if (votekick.kikar > votekick.deixar && votekick.enemys > 0 && votekick.allies > 0 && votekick.votes.Count >= Count / 2)
                {
                    Account j = GetPlayerBySlot(votekick.victimIdx);
                    if (j != null)
                    {
                        j.SendCompletePacket(PackageDataManager.VOTEKICK_KICK_WARNING_PAK);
                        KickedPlayersVote.Add(j.playerId);
                        RemovePlayer(j, true, 2);
                    }
                }
                uint erro = 0;
                if (votekick.allies == 0)
                {
                    erro = 2147488001;
                }
                else if (votekick.enemys == 0)
                {
                    erro = 2147488002;
                }
                else if (votekick.deixar < votekick.kikar || votekick.votes.Count < Count / 2)
                {
                    erro = 2147488000;
                }
                using (VOTEKICK_RESULT_PAK packet = new VOTEKICK_RESULT_PAK(erro, votekick))
                {
                    SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                }
            }
        }

        public void UpdateMatchCount(bool WonTheMatch, Account p, int winnerTeam, DBQuery query)
        {
            if (winnerTeam == 2)
            {
                query.AddQuery("fights_draw", ++p.statistics.fightsDraw);
            }
            else if (WonTheMatch)
            {
                query.AddQuery("fights_wins", ++p.statistics.fightsWin);
            }
            else
            {
                query.AddQuery("fights_lost", ++p.statistics.fightsLost);
            }
            query.AddQuery("fights", ++p.statistics.fights);
            query.AddQuery("all_fights", ++p.statistics.totalfights);
        }

        public int Percentage(int total, int percent)
        {
            try
            {
                return total * percent / 100;
            }
            catch
            {
                return 0;
            }
        }

        public void LogRoomResult()
        {
            lock (slots)
            {
                using (StringUtil str = new StringUtil())
                {
                    str.AppendLine(string.Empty);
                    str.AppendLine($" ===============================================================================================================================");
                    str.AppendLine($" [Room] DateLog: {DateTime.Now} RoomId: {roomId} UniqueId: {UniqueRoomId} Type: {mode} ChannelId: {channelId} Stage4vs4: {stage4vs4} IsBotMode: {IsBotMode()}");
                    str.AppendLine($" ===============================================================================================================================");
                    for (int i = 0; i < 16; i++)
                    {
                        Slot slot = slots[i];
                        if (slot.state == SlotStateEnum.BATTLE && slot.playerId > 0)
                        {
                            str.AppendLine($" [Player] Slot: {i} PlayerId: {slot.playerId} Kill/Death: {slot.allKills}/{slot.allDeaths} HeadShots: {slot.headshots} Team: {(slot.teamId % 2 == 0 ? "RED" : "BLUE")} Exp: {slot.exp} Gold: {slot.gold} Cash: {slot.cash} Score: {slot.score}");
                        }
                    }
                    str.AppendLine($" ===============================================================================================================================");
                    Logger.Room(str.GetString());
                }
            }
        }

        public void TryBalancePlayer(Account player, bool inBattle, ref Slot mySlot)
        {
            lock (slots)
            {
                if (balancing == BalancingTeamEnum.QTY)
                {
                    Slot CurrentSlot = GetSlot(player.slotId);
                    if (CurrentSlot == null)
                    {
                        return;
                    }
                    List<SlotChange> changeList = new List<SlotChange>();
                    int TeamIdx = GetBalanceTeamIdx(inBattle);
                    int[] teamArray = null;
                    if (CurrentSlot.teamId == TeamIdx || TeamIdx == -1)
                    {
                        teamArray = CurrentSlot.teamId == 0 ? RED_TEAM : BLUE_TEAM;
                        foreach (int newSlotId in teamArray)
                        {
                            if (newSlotId < CurrentSlot.Id)
                            {
                                Slot newSlot = slots[newSlotId];
                                if (newSlot.state == SlotStateEnum.EMPTY && newSlot.playerId == 0)
                                {
                                    newSlot.state = SlotStateEnum.READY;
                                    newSlot.playerId = player.playerId;
                                    newSlot.equipment = player.equipments;
                                    if (player.slotId == leaderSlot)
                                    {
                                        leaderSlot = newSlotId;
                                    }
                                    player.slotId = newSlotId;
                                    mySlot = newSlot;

                                    CurrentSlot.ResetSlot();
                                    CurrentSlot.state = SlotStateEnum.EMPTY;
                                    CurrentSlot.playerId = 0;
                                    CurrentSlot.equipment = null;

                                    changeList.Add(new SlotChange(CurrentSlot, newSlot));
                                    Logger.Temporary($" (TryBalance) BALANCE PLAYER [!] Nick: {player.nickname} OldSlot: {CurrentSlot.Id} NewSlot: {newSlot.Id}");
                                    break;
                                }
                            }
                        }
                        if (changeList.Count > 0)
                        {
                            using (PROTOCOL_ROOM_CHANGE_SLOTS_ACK packet = new PROTOCOL_ROOM_CHANGE_SLOTS_ACK(changeList, leaderSlot, 1))
                            {
                                SendPacketToPlayers(packet);
                            }
                            UpdateSlotsInfo();
                        }
                        return;
                    }
                    teamArray = TeamIdx == 0 ? RED_TEAM : BLUE_TEAM;
                    for (int i = 0; i < teamArray.Length; i++) //Move os slots do time que ele foi balanceado para o menor slot vazio deste mesmo time.
                    {
                        int index = teamArray[i];
                        Slot newSlot = slots[index];
                        if (newSlot.playerId == 0 && newSlot.state == SlotStateEnum.EMPTY)
                        {
                            newSlot.state = SlotStateEnum.READY;
                            newSlot.playerId = player.playerId;
                            newSlot.equipment = player.equipments;
                            if (player.slotId == leaderSlot)
                            {
                                leaderSlot = index;
                            }
                            player.slotId = index;
                            mySlot = newSlot;

                            CurrentSlot.ResetSlot();
                            CurrentSlot.state = SlotStateEnum.EMPTY;
                            CurrentSlot.playerId = 0;
                            CurrentSlot.equipment = null;

                            changeList.Add(new SlotChange(CurrentSlot, newSlot));
                            break;
                        }
                    }
                    if (changeList.Count > 0)
                    {
                        using (PROTOCOL_ROOM_CHANGE_SLOTS_ACK packet = new PROTOCOL_ROOM_CHANGE_SLOTS_ACK(changeList, leaderSlot, 1))
                        {
                            SendPacketToPlayers(packet);
                        }
                        UpdateSlotsInfo();
                    }
                }
            }
        }

        //Soma +1 Player no TEAM RED e compara a quantidade de jogadores, se menor ou igual ao TEAM BLUE o jogador é balanceado para o TEAM RED.
        //Caso o calculo de cima nao der verdadeiro, soma +1 Player no TEAM BLUE e compara a quantidade de jogadores, se menor ou igual ao TEAM RED o jogador é balanceado para o TEAM BLUE.
        //Caso um dos calculos dos times nao der verdadeiro, não é necessario o balanceamento, o jogador continua no mesmo slot e time.
        public int GetBalanceTeamIdx(bool inBattle)
        {
            int redPlayers = 0;
            int bluePlayers = 0;
            for (int i = 0; i < 16; i++)
            {
                Slot slot = slots[i];
                if (slot.state == SlotStateEnum.READY && !inBattle || slot.state >= SlotStateEnum.LOAD && inBattle)
                {
                    if (slot.teamId == 0)
                    {
                        redPlayers++;
                    }
                    else
                    {
                        bluePlayers++;
                    }
                }
            }
            return redPlayers + 1 <= bluePlayers ? 0 : bluePlayers + 1 <= redPlayers ? 1 : -1;
        }

        /// <summary>
        /// Checa se só tem 2 clãs na partida de clãs.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <returns></returns>
        public bool Have2ClansToClanMatch()
        {
            SortedList<int, ClanModel> clans = GetClanListMatchPlayers();
            return clans.Count == 2;
        }

        /// <summary>
        /// Checa se os 2 times possuem 4 jogadores de mesmos clãs.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <returns></returns>
        public bool HavePlayersToClanMatch()
        {
            SortedList<int, ClanModel> clans = GetClanListMatchPlayers();
            bool teamRed = false, teamBlue = false;
            foreach (ClanModel clan in clans.Values)
            {
                if (clan.redPlayers >= 4)
                {
                    teamRed = true;
                }
                else if (clan.bluePlayers >= 4)
                {
                    teamBlue = true;
                }
            }
            return teamRed && teamBlue;
        }

        public bool CheckClanMatchRestrict()
        {
            if (channelType == 4)
            {
                SortedList<int, ClanModel> clans = GetClanListMatchPlayers();
                foreach (ClanModel cm in clans.Values)
                {
                    if (cm.redPlayers >= 1 && cm.bluePlayers >= 1)
                    {
                        blockedClan = true;
                        Logger.Warning(" [Utilities] [CheckClanMatchRestrict] XP cancelado em clãfronto [Room: " + roomId + "; Canal: " + channelId + "; Clã: " + cm.clanId + "]");
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gera uma lista de TODOS os clãs que estão na sala, sem especificar qualquer SLOT_STATE.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <returns></returns>
        private SortedList<int, ClanModel> GetClanListMatchPlayers()
        {
            SortedList<int, ClanModel> clans = new SortedList<int, ClanModel>();
            List<Account> roomPlayers = GetAllPlayers();
            foreach (Account player in roomPlayers)
            {
                if (player.clanId == 0)
                {
                    continue;
                }
                if (clans.TryGetValue(player.clanId, out ClanModel model) && model != null)
                {
                    if (player.slotId % 2 == 0)
                    {
                        model.redPlayers++;
                    }
                    else
                    {
                        model.bluePlayers++;
                    }
                }
                else
                {
                    model = new ClanModel
                    {
                        clanId = player.clanId
                    };
                    if (player.slotId % 2 == 0)
                    {
                        model.redPlayers++;
                    }
                    else
                    {
                        model.bluePlayers++;
                    }
                    clans.Add(player.clanId, model);
                }
            }
            return clans;
        }

        public void BattleEndKills(bool isBotMode)
        {
            int killsByMask = GetKillsByMask();
            if (redKills < killsByMask && blueKills < killsByMask)
            {
                return;
            }
            List<Account> players = GetAllPlayers(SlotStateEnum.READY, 1);
            if (players.Count > 0)
            {
                TeamResultTypeEnum winner = GetWinnerTeam();
                CalculateResult(winner, isBotMode);
                GetBattleResult(out ushort missionCompletes, out ushort inBattle, out byte[] a1);
                using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(this, winner, RoundEndTypeEnum.TimeOut))
                {
                    byte[] data = packet.GetCompleteBytes("Utilities.BaseEndByKills");
                    for (int i = 0; i < players.Count; i++)
                    {
                        Account player = players[i];
                        Slot slot = GetSlot(player.slotId);
                        if (slot != null)
                        {
                            if (slot.state == SlotStateEnum.BATTLE)
                            {
                                player.SendCompletePacket(data);
                            }
                            player.SendPacket(new PROTOCOL_BATTLE_ENDBATTLE_ACK(player, winner, inBattle, missionCompletes, isBotMode, a1));
                        }
                    }
                }
            }
            ResetBattleInfo();
        }

        public TeamResultTypeEnum GetWinnerTeam()
        {
            byte value = 0;
            if (mode == RoomTypeEnum.Destruction || mode == RoomTypeEnum.Sabotage || mode == RoomTypeEnum.Suppression || mode == RoomTypeEnum.Defense)
            {
                if (blueRounds == redRounds)
                {
                    value = 2;
                }
                else if (blueRounds > redRounds)
                {
                    value = 1;
                }
                else if (blueRounds < redRounds)
                {
                    value = 0;
                }
            }
            else if (mode == RoomTypeEnum.Dino)
            {
                if (blueDino == redDino)
                {
                    value = 2;
                }
                else if (blueDino > redDino)
                {
                    value = 1;
                }
                else if (blueDino < redDino)
                {
                    value = 0;
                }
            }
            else
            {
                if (blueKills == redKills)
                {
                    value = 2;
                }
                else if (blueKills > redKills)
                {
                    value = 1;
                }
                else if (blueKills < redKills)
                {
                    value = 0;
                }
            }
            return (TeamResultTypeEnum)value;
        }

        public TeamResultTypeEnum GetWinnerTeam(int RedPlayers, int BluePlayers)
        {
            if (RedPlayers == 0)
            {
                return TeamResultTypeEnum.TeamBlueWin;
            }
            else if (BluePlayers == 0)
            {
                return TeamResultTypeEnum.TeamRedWin;
            }
            return TeamResultTypeEnum.TeamDraw;
        }

        /// <summary>
        /// Gera uma array de bytes com o resultado da partida.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="playersCompletedMission">Jogadores que completaram missões</param>
        /// <param name="playersInBattle">Jogadores na partida</param>
        /// <param name="data">Resultado</param>
        public void GetBattleResult(out ushort playersCompletedMission, out ushort playersInBattle, out byte[] data)
        {
            playersCompletedMission = 0;
            playersInBattle = 0;
            data = new byte[144];
            using (PacketWriter write = new PacketWriter())
            {
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = slots[i];
                    if (slot.state >= SlotStateEnum.LOAD)
                    {
                        ushort flag = (ushort)slot.flag;
                        if (slot.missionsCompleted)
                        {
                            playersCompletedMission += flag;
                        }
                        playersInBattle += flag;
                    }
                    write.WriteH(0 + (i * 2), (ushort)slot.exp);
                    write.WriteH(32 + (i * 2), (ushort)slot.gold);
                    write.WriteH(64 + (i * 2), (ushort)slot.bonusEXP);
                    write.WriteH(96 + (i * 2), (ushort)slot.bonusGold);
                    write.WriteC(128 + i, (byte)slot.bonusFlags);
                }
                data = write.memorystream.ToArray();
            }
        }

        /// <summary>
        /// Não calcula os resultados da partida e envia o placar a todos os jogadores com estado acima de READY.
        /// </summary>
        /// <param name="room">Sala</param>
        public void EndBattleNoPoints()
        {
            List<Account> players = GetAllPlayers(SlotStateEnum.READY, 1);
            if (players.Count > 0)
            {
                GetBattleResult(out ushort missionCompletes, out ushort inBattle, out byte[] data);
                bool isBotMode = IsBotMode();
                for (int i = 0; i < players.Count; i++)
                {
                    Account player = players[i];
                    player.SendPacket(new PROTOCOL_BATTLE_ENDBATTLE_ACK(player, TeamResultTypeEnum.TeamDraw, inBattle, missionCompletes, isBotMode, data));
                }
            }
            ResetBattleInfo();
        }

        /// <summary>
        /// Calcula os resultados da partida e envia o placar a todos os jogadores com estado acima de READY.
        /// </summary>
        /// <param name="room">Room</param>
        /// <param name="isBotMode">É modo contra I.A?</param>
        /// <param name="winnerTeam">Time vencedor</param>
        public void EndBattle(bool isBotMode, TeamResultTypeEnum winnerTeam)
        {
            List<Account> players = GetAllPlayers(SlotStateEnum.READY, 1);
            if (players.Count > 0)
            {
                CalculateResult(winnerTeam, isBotMode);
                GetBattleResult(out ushort missionCompletes, out ushort inBattle, out byte[] data);
                for (int i = 0; i < players.Count; i++)
                {
                    Account player = players[i];
                    player.SendPacket(new PROTOCOL_BATTLE_ENDBATTLE_ACK(player, winnerTeam, inBattle, missionCompletes, isBotMode, data));
                }
            }
            ResetBattleInfo();
        }

        /// <summary>
        /// Checa se a partida tem jogadores suficientes para permanecer ativa.
        /// Não funciona em modo contra I.A.
        /// Funciona tanto para partida em andamento, quanto para o preparatório.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="isBotMode">É modo contra I.A?</param>
        public void BattleEndPlayersCount(bool isBotMode)
        {
            if (isBotMode || !IsPreparing())
            {
                return;
            }
            int PlayersTeamBlueInBattle = 0, PlayersTeamRedInBattle  = 0, PlayersTeamBlueInLoad = 0, PlayersTeamRedInLoad = 0;
            for (int i = 0; i < 16; i++)
            {
                Slot slot = slots[i];
                if (slot.state == SlotStateEnum.BATTLE)
                {
                    if (slot.teamId == 0)
                    {
                        PlayersTeamRedInBattle++;
                    }
                    else
                    {
                        PlayersTeamBlueInBattle++;
                    }
                }
                else if (slot.state >= SlotStateEnum.LOAD)
                {
                    if (slot.teamId == 0)
                    {
                        PlayersTeamRedInLoad++;
                    }
                    else
                    {
                        PlayersTeamBlueInLoad++;
                    }
                }
            }
            if (((PlayersTeamRedInBattle == 0 || PlayersTeamBlueInBattle == 0) && state == RoomStateEnum.Battle) || ((PlayersTeamRedInLoad == 0 || PlayersTeamBlueInLoad == 0) && state <= RoomStateEnum.PreBattle))
            {
                EndBattle(isBotMode, GetWinnerTeam(PlayersTeamRedInBattle, PlayersTeamBlueInBattle));
            }
        }

        /// <summary>
        /// Gera uma lista de jogadores como dinossauros. 
        /// <para>É possível indicar um novo T-Rex.</para>
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="forceNewTRex">Gerar um novo T-Rex a força</param>
        /// <param name="forceRexIdx">Indicar um slot para ser o T-Rex. (-2 = Aleatório)</param>
        /// <returns></returns>
        public List<int> GetDinossaurs(bool forceNewTRex, int forceRexIdx)
        {
            List<int> dinos = new List<int>();
            if (mode == RoomTypeEnum.Dino || mode == RoomTypeEnum.CrossCounter)
            {
                int teamIdx = rounds == 1 ? 0 : 1;
                int[] array = GetTeamArray(teamIdx);
                for (int i = 0; i < array.Length; i++)
                {
                    int slotIdx = array[i];
                    Slot slot = slots[slotIdx];
                    if (slot.state == SlotStateEnum.BATTLE && !slot.specGM)
                    {
                        dinos.Add(slotIdx);
                    }
                }
                if ((TRex == -1 || slots[TRex].state <= SlotStateEnum.BATTLE_READY || forceNewTRex) && dinos.Count > 1 && mode == RoomTypeEnum.Dino)
                {
                    if (forceRexIdx >= 0 && dinos.Contains(forceRexIdx))
                    {
                        TRex = forceRexIdx;
                    }
                    else if (forceRexIdx == -2)
                    {
                        TRex = dinos[new Random().Next(0, dinos.Count)];
                    }
                    Logger.Warning(" [Room] [GetDinossaurs] [" + DateTime.Now + "] forceRexIdx: " + forceRexIdx + "; force: " + forceNewTRex + "; teamIdx: " + teamIdx + "; trex: " + TRex);
                }
            }
            return dinos;
        }

        /// <summary>
        /// Reseta as informações básicas do slot. Se o estado do slot for maior que READY, ele volta para NORMAL. É reiniciado o histórico de missões da partida.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="slot">Slot do jogador</param>
        /// <param name="updateInfo">Atualizar informações dos slots?</param>
        public void ResetSlotInfo(Slot slot, bool updateInfo)
        {
            if (slot.state >= SlotStateEnum.LOAD)
            {
                ChangeSlotState(slot, SlotStateEnum.NORMAL, updateInfo);
                slot.ResetSlot();
            }
        }

        /// <summary>
        /// Analisa se a partida deve ser finalizada, ou reiniciada, seguindo alguns parâmetros. Haverá mensagem do motivo da vitória (Time morto). Haverá checagem para desativação da bomba.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="winner">Time vencedor</param>
        /// <param name="forceRestart">Reiniciar uma rodada, de maneira forçada, caso necessário</param>
        public void BattleEndRound(int winner, bool forceRestart)
        {
            int roundsByMask = GetRoundsByMask();
            if (redRounds >= roundsByMask || blueRounds >= roundsByMask)
            {
                StopBomb();
                using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(this, winner, RoundEndTypeEnum.AllDeath))
                {
                    SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                }
                EndBattle(IsBotMode(), (TeamResultTypeEnum)winner);
            }
            else if (!PlantedBombC4 || forceRestart)
            {
                StopBomb();
                rounds++;
                using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(this, winner, RoundEndTypeEnum.AllDeath))
                {
                    SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                }
                RoundRestart();
            }
        }

        /// <summary>
        /// Analisa se a partida deve ser finalizada, ou reiniciada. Haverá mensagem do motivo da vitória e é possível personalizá-la. Haverá checagem para desativação da bomba.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="winner">Time vencedor</param>
        /// <param name="motive">Motivo da vitória</param>
        public void BattleEndRound(int winner, RoundEndTypeEnum motive)
        {
            using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(this, winner, motive))
            {
                SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
            }
            StopBomb();
            int roundsByMask = GetRoundsByMask();
            if (redRounds >= roundsByMask || blueRounds >= roundsByMask)
            {
                EndBattle(IsBotMode(), (TeamResultTypeEnum)winner);
            }
            else
            {
                rounds++;
                RoundRestart();
            }
        }

        /// <summary>
        /// Tenta recomeçar/terminar a partida, de acordo com a quantidade de jogadores vivos. (Somente para os modos Destruição e Supressão)
        /// </summary>
        /// <param name="room">Sala</param>
        public void BattleEndRoundPlayersCount()
        {
            if (round.Timer == null && (mode == RoomTypeEnum.Destruction || mode == RoomTypeEnum.Suppression)) //Destruição e Supressão
            {
                GetPlayingPlayers(true, out int redPlayers, out int bluePlayers, out int redDeaths, out int blueDeaths);
                if (redDeaths == redPlayers)
                {
                    if (!PlantedBombC4)
                    {
                        blueRounds++;
                    }
                    BattleEndRound(1, false);
                }
                else if (blueDeaths == bluePlayers)
                {
                    redRounds++;
                    BattleEndRound(0, true);
                }
            }
        }

        /// <summary>
        /// Identifica os jogadores que estão dentro da partida. Resultado no padrão Flag.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="onlyNoSpectators">Somente jogadores que não estão espectando a partida</param>
        /// <param name="missionSuccess">Somente jogadores que conseguiram completar no mínimo 1 missão</param>
        /// <returns></returns>
        public ushort GetSlotsFlag(bool onlyNoSpectators, bool missionSuccess)
        {
            int flags = 0;
            for (int i = 0; i < 16; i++)
            {
                Slot slot = slots[i];
                if (slot.state >= SlotStateEnum.LOAD && ((missionSuccess && slot.missionsCompleted) || (!missionSuccess && (!onlyNoSpectators || !slot.espectador))))
                {
                    flags += slot.flag;
                }
            }
            return (ushort)flags;
        }

        public void MissionCompleteBase(Account player, Slot slot, FragInfos kills, MissionTypeEnum autoComplete, int moreInfo)
        {
            try
            {
                PlayerMissions missions = slot.missions;
                if (missions == null || player == null)
                {
                    Logger.Error(" [MissionCompleteBase] Missions null1! by accountId: " + slot.playerId);
                    return;
                }
                int currentMissionId = missions.GetCurrentMissionId(), cardId = missions.GetCurrentCard();
                if (currentMissionId <= 0 || missions.selectedCard)
                {
                    return;
                }
                List<Card> cards = MissionCardXML.GetCards(currentMissionId, cardId);
                if (cards.Count == 0)
                {
                    return;
                }
                KillingMessageEnum killingMessage = kills.GetAllKillFlags();
                byte[] missionArray = missions.GetCurrentMissionList();

                ClassTypeEnum weaponClass = GetIdClassType(kills.weaponId);
                ClassTypeEnum convertedClass = ConvertWeaponClass(weaponClass);
                int weaponId = GetIdStatics(kills.weaponId, 4);
                ClassTypeEnum moreClass = moreInfo > 0 ? GetIdClassType(moreInfo) : 0;
                ClassTypeEnum moreConvClass = moreInfo > 0 ? ConvertWeaponClass(moreClass) : 0;
                int moreId = moreInfo > 0 ? GetIdStatics(moreInfo, 4) : 0;

                for (int i = 0; i < cards.Count; i++)
                {
                    Card card = cards[i];
                    byte count = 0;
                    if (card.mapId == 0 || card.mapId == mapId)
                    {
                        if (kills.frags.Count > 0)
                        {
                            if (card.missionType == MissionTypeEnum.KILL ||
                                card.missionType == MissionTypeEnum.CHAINSTOPPER && killingMessage.HasFlag(KillingMessageEnum.ChainStopper) ||
                                card.missionType == MissionTypeEnum.CHAINSLUGGER && killingMessage.HasFlag(KillingMessageEnum.ChainSlugger) ||
                                card.missionType == MissionTypeEnum.CHAINKILLER && slot.killsOnLife >= 4 ||
                                card.missionType == MissionTypeEnum.TRIPLE_KILL && slot.killsOnLife == 3 ||
                                card.missionType == MissionTypeEnum.DOUBLE_KILL && slot.killsOnLife == 2 ||
                                card.missionType == MissionTypeEnum.HEADSHOT && (killingMessage.HasFlag(KillingMessageEnum.Headshot) || killingMessage.HasFlag(KillingMessageEnum.ChainHeadshot)) ||
                                card.missionType == MissionTypeEnum.CHAINHEADSHOT && killingMessage.HasFlag(KillingMessageEnum.ChainHeadshot) ||
                                card.missionType == MissionTypeEnum.PIERCING && killingMessage.HasFlag(KillingMessageEnum.PiercingShot) ||
                                card.missionType == MissionTypeEnum.MASS_KILL && killingMessage.HasFlag(KillingMessageEnum.MassKill) ||
                                card.missionType == MissionTypeEnum.KILL_MAN && (mode == RoomTypeEnum.Dino || mode == RoomTypeEnum.CrossCounter) && (slot.teamId == 1 && rounds == 2 || slot.teamId == 0 && rounds == 1))
                            {
                                count = CheckPlayersClass1(card, weaponClass, convertedClass, weaponId, kills);
                            }
                            else if (card.missionType == MissionTypeEnum.KILL_WEAPONCLASS || card.missionType == MissionTypeEnum.DOUBLE_KILL_WEAPONCLASS && slot.killsOnLife == 2 || (card.missionType == MissionTypeEnum.TRIPLE_KILL_WEAPONCLASS && slot.killsOnLife == 3))
                            {
                                count = CheckPlayersClass2(card, kills);
                            }
                        }
                        else if (card.missionType == MissionTypeEnum.DEATHBLOW && autoComplete == MissionTypeEnum.DEATHBLOW)
                        {
                            count = CheckPlayerClass(card, moreClass, moreConvClass, moreId);
                        }
                        else if (card.missionType == autoComplete)
                        {
                            count = 1;
                        }
                    }
                    if (count == 0)
                    {
                        continue;
                    }
                    int ArrayIdx = card.arrayIdx;
                    if (missionArray[ArrayIdx] + 1 > card.missionLimit)
                    {
                        continue;
                    }
                    slot.missionsCompleted = true;
                    missionArray[ArrayIdx] += count;
                    if (missionArray[ArrayIdx] > card.missionLimit)
                    {
                        missionArray[ArrayIdx] = card.missionLimit;
                    }
                    byte progress = missionArray[ArrayIdx];
                    player.SendPacket(new BASE_QUEST_COMPLETE_PAK(progress, card));
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
        public void MissionCompleteBase(Account player, Slot slot, MissionTypeEnum autoComplete, int moreInfo)
        {
            try
            {
                PlayerMissions missions = slot.missions;
                if (missions == null || player == null)
                {
                    Logger.Error(" [MissionCompleteBase] Missions null2! by accountId: " + slot.playerId);
                    return;
                }
                int currentMissionId = missions.GetCurrentMissionId(), cardId = missions.GetCurrentCard();
                if (currentMissionId <= 0 || missions.selectedCard)
                {
                    return;
                }
                List<Card> cards = MissionCardXML.GetCards(currentMissionId, cardId);
                if (cards.Count == 0)
                {
                    return;
                }
                byte[] missionArray = missions.GetCurrentMissionList();
                ClassTypeEnum moreClass = moreInfo > 0 ? GetIdClassType(moreInfo) : 0;
                ClassTypeEnum moreConvClass = moreInfo > 0 ? ConvertWeaponClass(moreClass) : 0;
                int moreId = moreInfo > 0 ? GetIdStatics(moreInfo, 4) : 0;

                for (int i = 0; i < cards.Count; i++)
                {
                    Card card = cards[i];
                    byte count = 0;
                    if (card.mapId == 0 || card.mapId == mapId)
                    {
                        if (card.missionType == MissionTypeEnum.DEATHBLOW && autoComplete == MissionTypeEnum.DEATHBLOW)
                        {
                            count = CheckPlayerClass(card, moreClass, moreConvClass, moreId);
                        }
                        else if (card.missionType == autoComplete)
                        {
                            count = 1;
                        }
                    }
                    if (count == 0)
                    {
                        continue;
                    }

                    int ArrayIdx = card.arrayIdx;
                    if (missionArray[ArrayIdx] + 1 > card.missionLimit)
                    {
                        continue;
                    }
                    slot.missionsCompleted = true;
                    missionArray[ArrayIdx] += count;
                    if (missionArray[ArrayIdx] > card.missionLimit)
                    {
                        missionArray[ArrayIdx] = card.missionLimit;
                    }

                    byte progress = missionArray[ArrayIdx];
                    player.SendPacket(new BASE_QUEST_COMPLETE_PAK(progress, card));
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private byte CheckPlayersClass1(Card card, ClassTypeEnum weaponClass, ClassTypeEnum convertedClass, int weaponId, FragInfos infos)
        {
            byte count = 0;
            if ((card.weaponReqId == 0 || card.weaponReqId == weaponId) && (card.weaponReq == ClassTypeEnum.Unknown || card.weaponReq == weaponClass || card.weaponReq == convertedClass))
            {
                for (int i = 0; i < infos.frags.Count; i++)
                {
                    Frag frag = infos.frags[i];
                    if (frag.VictimSlot % 2 != infos.killerIdx % 2)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        private byte CheckPlayersClass2(Card card, FragInfos infos)
        {
            byte count = 0;
            for (int i = 0; i < infos.frags.Count; i++)
            {
                Frag frag = infos.frags[i];
                if (frag.VictimSlot % 2 != infos.killerIdx % 2 && (card.weaponReq == ClassTypeEnum.Unknown || card.weaponReq == (ClassTypeEnum)frag.victimWeaponClass || card.weaponReq == ConvertWeaponClass((ClassTypeEnum)frag.victimWeaponClass)))
                {
                    count++;
                }
            }
            return count;
        }

        private byte CheckPlayerClass(Card card, ClassTypeEnum weaponClass, ClassTypeEnum convertedClass, int weaponId)
        {
            return (byte)((card.weaponReqId == 0 || card.weaponReqId == weaponId) && (card.weaponReq == ClassTypeEnum.Unknown || card.weaponReq == weaponClass || card.weaponReq == convertedClass) ? 1 : 0);
        }

        private ClassTypeEnum ConvertWeaponClass(ClassTypeEnum weaponClass)
        {
            if (weaponClass == ClassTypeEnum.DualSMG)
            {
                return ClassTypeEnum.SMG;
            }
            else if (weaponClass == ClassTypeEnum.DualHandGun)
            {
                return ClassTypeEnum.HandGun;
            }
            else if (weaponClass == ClassTypeEnum.DualKnife || weaponClass == ClassTypeEnum.Knuckle)
            {
                return ClassTypeEnum.Knife;
            }
            else if (weaponClass == ClassTypeEnum.DualShotgun)
            {
                return ClassTypeEnum.Shotgun;
            }
            return weaponClass;
        }

        /// <summary>
        /// Gera informações do Id de um item.
        /// </summary>
        /// <param name="weaponId">Id do item</param>
        /// <param name="type">Tipo de informação. 1 = Inicio (ITEM_CLASS); 2 = Usage; 3 = Meio (ClassType); 4 = Final (Number)</param>
        /// <returns></returns>
        public int GetIdStatics(int weaponId, int type)
        {
            try
            {
                if (type == 1)
                {
                    return weaponId / 100000000; //primeiros valores - classtype
                }
                else if (type == 2)
                {
                    return weaponId % 100000000 / 1000000; //usage
                }
                else if (type == 3)
                {
                    return weaponId % 1000000 / 1000; //valores do meio - type
                }
                else if (type == 4)
                {
                    return weaponId % 1000; //ultimos 3 valores - number
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return 0;
        }

        /// <summary>
        /// Retorna a classe de um equipamento.
        /// </summary>
        /// <param name="id">Id do item</param>
        /// <returns></returns>
        public ClassTypeEnum GetIdClassType(int id)
        {
            try
            {
                return (ClassTypeEnum)(id % 1000000 / 1000);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return 0;
        }

        public void UpdatePlayerInfoInBattle(Slot slot, CupomEffects effects, int type)
        {
            try
            {
                if (slot != null && slot.client != null)
                {
                    ResyncTick();
                    if (type == 0 || type == 2) //0 = Entrando na partida (normalmente, primeiro respawn) | 1 = Entrando na partida de espectador (Destru/Supressão) | 2 = Entrando na partida (normalmente/não é o primeiro respawn do jogador)
                    {
                        int charaId = 0, value = 0, number = 0;
                        if (mode == RoomTypeEnum.Dino || mode == RoomTypeEnum.CrossCounter)
                        {
                            if ((rounds == 1 && slot.teamId == 1) || (rounds == 2 && slot.teamId == 0))
                            {
                                charaId = rounds == 2 ? slot.equipment.red : slot.equipment.blue;
                            }
                            else if (TRex == slot.Id)
                            {
                                charaId = -1;
                            }
                            else
                            {
                                charaId = slot.equipment.dino;
                            }
                        }
                        else
                        {
                            charaId = slot.teamId == 0 ? slot.equipment.red : slot.equipment.blue;
                        }
                        int HPBonus = 0;
                        if (effects.HasFlag(CupomEffects.Ketupat))
                        {
                            HPBonus += 10;
                        }
                        if (effects.HasFlag(CupomEffects.HP5))
                        {
                            HPBonus += 5;
                        }
                        if (effects.HasFlag(CupomEffects.HP10))
                        {
                            HPBonus += 10;
                        }
                        if (charaId == -1)
                        {
                            value = 255;
                            number = 65535;
                        }
                        else
                        {
                            value = charaId % 100000000 / 1000000;
                            number = charaId % 1000;
                        }
                        slot.isDead = false;
                        slot.plantDuration = Settings.PlantDuration;
                        slot.defuseDuration = Settings.DefuseDuration;
                        if (effects.HasFlag(CupomEffects.C4SpeedKit))
                        {
                            slot.plantDuration -= Settings.PlantDuration * 50 / 100;
                            slot.defuseDuration -= Settings.DefuseDuration * 25 / 100;
                        }
                        if (!IsCase4BotMode)
                        {
                            if (SourceToMap == -1)
                            {
                                RoundResetRoomF1();
                            }
                            else
                            {
                                RoundResetRoomS1();
                            }
                        }
                        if (value == 255)
                        {
                            slot.TRexImmortal = true;
                        }
                        else
                        {
                            slot.TRexImmortal = false;
                            int CharaHP = CharaXML.GetLifeById(number, value);
                            CharaHP += CharaHP * HPBonus / 100;
                            slot.maxLife = CharaHP;
                            slot.ResetLife();
                        }
                    }
                    if (IsCase4BotMode || type == 2 || !ObjectsIsValid())
                    {
                        return;
                    }
                    List<ObjectHitInfo> syncList = new List<ObjectHitInfo>();
                    for (int i = 0; i < Objects.Length; i++)
                    {
                        ObjectInfo objectInfo = Objects[i];
                        ObjModel objModel = objectInfo.info;
                        if (objModel != null && (type != 2 && objModel.IsDestroyable && objectInfo.life != objModel.Life || objModel.NeedSync))
                        {
                            syncList.Add(new ObjectHitInfo(3)
                            {
                                ObjSyncId = objModel.NeedSync ? 1 : 0,
                                AnimationId1 = objModel.Anim1,
                                AnimationId2 = objectInfo.animation != null ? objectInfo.animation.Id : 255,
                                DestroyState = objectInfo.destroyState,
                                ObjId = objModel.Id,
                                ObjectLife = objectInfo.life,
                                SpecialUse = Packet4Creator.GetTime(objectInfo.useDate)
                            });
                        }
                    }
                    for (int i = 0; i < 16; i++)
                    {
                        Slot playerRoom = slots[i];
                        if (playerRoom.Id != slot.Id && !playerRoom.TRexImmortal && playerRoom.pingDate != new DateTime() && (playerRoom.maxLife != playerRoom.life || playerRoom.isDead))
                        {
                            syncList.Add(new ObjectHitInfo(4)
                            {
                                ObjId = playerRoom.Id,
                                ObjectLife = playerRoom.life
                            });
                        }
                    }
                    if (syncList.Count > 0)
                    {
                        byte[] actions = Packet4Creator.GetCode4SyncData(syncList);
                        byte[] packet = Packet4Creator.GetCode4(actions, StartTime, rounds, 255);
                        BattleManager.SendPacket(packet, slot.client, sessionPort);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public void PassPortal(Slot slot)
        {
            if (mode != RoomTypeEnum.Dino)
            {
                return;
            }
            slot.ResetLife();
            if (round.Timer == null && state == RoomStateEnum.Battle && mode == RoomTypeEnum.Dino && slot.state == SlotStateEnum.BATTLE)
            {
                slot.passSequence++;
                if (slot.teamId == 0)
                {
                    redDino += 5;
                }
                else
                {
                    blueDino += 5;
                }
                MissionTypeEnum mission = MissionTypeEnum.NA;
                if (slot.passSequence == 1)
                {
                    mission = MissionTypeEnum.TOUCHDOWN;
                }
                else if (slot.passSequence == 2)
                {
                    mission = MissionTypeEnum.TOUCHDOWN_ACE_ATTACKER;
                }
                else if (slot.passSequence == 3)
                {
                    mission = MissionTypeEnum.TOUCHDOWN_HATTRICK;
                }
                else if (slot.passSequence >= 4)
                {
                    mission = MissionTypeEnum.TOUCHDOWN_GAME_MAKER;
                }
                if (mission != MissionTypeEnum.NA)
                {
                    MissionCompleteBase(GetPlayerBySlot(slot), slot, mission, 0);
                }
                using (BATTLE_MISSION_ESCAPE_PAK packet = new BATTLE_MISSION_ESCAPE_PAK(this, slot))
                using (BATTLE_DINO_PLACAR_PAK packet2 = new BATTLE_DINO_PLACAR_PAK(this))
                {
                    SendPacketToPlayers(packet, packet2, SlotStateEnum.BATTLE, 0);
                }
            }
        }
    }
}