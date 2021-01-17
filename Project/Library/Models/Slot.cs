using System;
using System.Collections.Generic;
using System.Net;

namespace PointBlank
{
    public class Slot
    {
        public DateTime lastTick = new DateTime();
        public int TICKS = 0;
        public int Id;
        public long playerId;
        public bool firstRespawn = true;
        public bool repeatLastState;
        public bool check;
        public bool espectador;
        public bool specGM;
        public bool withHost;
        public int teamId;
        public int flag;
        public int aiLevel;
        public byte ping;
        public int passSequence;
        public int isPlaying;
        public int earnedXP;
        public int spawnsCount;
        public int headshots;
        public int lastKillState;
        public int killsOnLife;
        public uint lastTimeRoom = uint.MaxValue;
        public int exp;
        public int cash;
        public int gold;
        public ushort score;
        public ushort allKills;
        public ushort allDeaths;
        public int objetivos;
        public int bonusEXP;
        public int bonusGold;
        public int bonusCash;
        public int unkItem;

        public int latency;
        public int failLatencyTimes;

        public int voteCounts;
        public DateTime nextVoteDate;
        public DateTime startTime;
        public ushort damageBar1;
        public ushort damageBar2;
        public bool missionsCompleted;

        public PlayerMissions missions;
        public SlotStateEnum state;
        public ResultIcon bonusFlags;
        public DeadEnum deathState = DeadEnum.isAlive;
        public PlayerEquipedItems equipment;
        public TimerState timing = new TimerState();
        public List<int> EquipmentsUsed = new List<int>();

        #region BATTLE

        public int life = 100;
        public int maxLife = 100;
        public int PlayerIdRegister;
        public float plantDuration;
        public float defuseDuration;
        public float C4FTime;
        public bool isDead = true;
        public bool TRexImmortal;
        public Half3 position;
        public IPEndPoint client;
        public DateTime pingDate;
        public DateTime lastDie;
        public DateTime C4First;

        public ClassTypeEnum weaponClass = ClassTypeEnum.Unknown;
        public CharactersEnum character;
        public WeaponInfo weaponModel;
        public float PartidaTime;

        public float GetSlotTime()
        {
            return (float)(DateTime.Now - pingDate).TotalSeconds;
        }

        public bool CompareIP(IPEndPoint ip)
        {
            return client != null && ip != null && client.Address.Equals(ip.Address) && client.Port == ip.Port;
        }

        public bool AccountIdIsValid(int number)
        {
            return PlayerIdRegister == number;
        }

        public void CheckLifeValue()
        {
            if (life > maxLife)
            {
                life = maxLife;
            }
        }

        public void ResetAllInfos()
        {
            client = null;
            pingDate = new DateTime();
            PlayerIdRegister = 0;
            ResetBattleInfos();
        }
        public void ResetLife()
        {
            life = maxLife;
        }

        public void ResetBattleInfos()
        {
            TRexImmortal = false;
            isDead = true;
            weaponClass = ClassTypeEnum.Unknown;
            lastDie = new DateTime();
            C4First = new DateTime();
            C4FTime = 0;
            position = new Half3();
            life = 100;
            maxLife = 100;
            plantDuration = Settings.PlantDuration;
            defuseDuration = Settings.DefuseDuration;
        }
        #endregion

        public Slot(int slotIdx)
        {
            SetSlotId(slotIdx);
        }

        /// <summary>
        /// Cancela a contagem de tempo para o jogador ser expulso da partida.
        /// </summary>
        public void StopTiming()
        {
            if (timing != null)
            {
                timing.Timer = null;
            }
        }

        public void SetSlotId(int slotIdx)
        {
            Id = slotIdx;
            teamId = slotIdx % 2;
            flag = 1 << slotIdx;
        }

        public void ResetSlot()
        {
            repeatLastState = false;
            deathState = DeadEnum.isAlive;
            StopTiming();
            check = false;
            espectador = false;
            specGM = false;
            withHost = false;
            firstRespawn = true;
            failLatencyTimes = 0;
            latency = 0;
            ping = 0;
            passSequence = 0;
            allDeaths = 0;
            allKills = 0;
            bonusFlags = 0;
            killsOnLife = 0;
            lastKillState = 0;
            score = 0;
            gold = 0;
            exp = 0;
            headshots = 0;
            objetivos = 0;
            bonusGold = 0;
            bonusEXP = 0;
            spawnsCount = 0;
            damageBar1 = 0;
            damageBar2 = 0;
            earnedXP = 0;
            isPlaying = 0;
            cash = 0;
            voteCounts = 0;
            nextVoteDate = new DateTime();
            aiLevel = 0;
            EquipmentsUsed.Clear();
            missionsCompleted = false;
            missions = null;
        }
        public void SetMissionsClone(PlayerMissions missions)
        {
            missionsCompleted = false;
            this.missions = null;
            this.missions = missions.DeepCopy();
        }

        /// <summary>
        /// Retorna a quantidade de segundos em que o jogador participou da partida.
        /// </summary>
        /// <param name="date">Data de término da partida</param>
        /// <returns></returns>
        public double InBattleTime(DateTime date)
        {
            if (startTime == new DateTime())// || startTime > date)
            {
                return 0;
            }
            return (date - startTime).TotalSeconds;
        }
    }
}