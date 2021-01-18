using Nini.Config;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

/*
    [PORTUGUESE BRAZIL]
    Este código-fonte é de autoria da <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ é um software privado: você não pode usar,
    redistribuir e/ou modificar sem autorização.

    Você deveria ter recebido uma autorização com esse software/codigo-fonte
    para ter cópias, se você não recebeu, encerre/exclua imediatamente 
    todos os vestígios do mesmo para não ter futuros problemas.

    Atenciosamente, <BR.UZ/>, MomzGames, ExploitTeam.
*/

/*
    [INGLISH USA]
    This source code is written by <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ a private software: you will not be able to use it,
    redistribute it and / or modify without authorization.

    You should have received an authorization with this software/source code
    to have copies, if you did not receive it, immediately shutdown/exclude
    all traces of it so that you have no future problems.

    Sincerely, <BR.UZ/>, MomzGames, ExploitTeam.
*/

namespace PointBlank
{
    public class Settings
    {
        public static UdpStateEnum UdpType;
        public static string IPInternal;
        public static string IPExternal; //Only PreparStart
        public static IPAddress AddressExternal; //Only PreparStart
        public static string Rede; //Private, Public
        public static string HasString;
        public static string ExitUrl;
        public static string UserFileList;
        public static string ClientVersion;
        public static ClientLocaleEnum ClientLocale;
        public static string DBName;
        public static string DBHost;
        public static string DBUsername;
        public static string DBPassword;
        public static string UdpVersion;
        public static string ServerPassword;

        public static int PortAuth;
        public static int PortGame;
        public static int PortApi;
        public static ushort PortBattle;
        public static int SessionsBattle = 5;
        public static int MaxRoomsPerSession;
        public static int DBPort;
        public static int BackLog;
        public static Encoding EncodingText;
        public static int ServerId;
        public static int LoginType;
        public static int MaxPlayersChannel;

        public static ushort MaxBattleExp;
        public static ushort MaxBattleGold;
        public static ushort MaxBattleCash;
        public static ushort MaxChallengeExp;
        public static ushort MaxChallengeGold;
        public static int MaxRoomsPerChannel;
        public static int MaxStartVoteKick;
        public static int MinRankStartVoteKick;
        public static int MinRankMasterClan;
        public static int NextVoteKickMinutes;

        public static int PingUpdateTimeSeconds = 7;
        public static int PlayersServerUpdateTimeSeconds = 7;
        public static int AuthConnectionIntervalSeconds = 15;
        public static int GameConnectionIntervalSeconds = 45;
        public static int UpdateIntervalPlayersServer = 2;
        public static int EmptyRoomRemovalInterval = 2;
        public static int ConsoleTitleUpdateTimeSeconds = 3;
        public static int IntervalEnterRoomAfterKickSeconds = 30;
        public static int MaxBuyItemDays = 365;
        public static int MaxBuyItemUnits = 100000;
        public static int MaxRepeatLatency;
        public static int MaxBattleLatency;
        public static int LimitAccountIp;
        public static int MaxClanCreate;
        public static int MaxClanActive;
        public static float MaxClanPoints;
        public static int ClanCreateRank;
        public static int ClanCreateGold;

        public static int MaxRanks47 = 200;
        public static int MaxRanks48 = 100;
        public static int MaxRanks49 = 50;
        public static int MaxRanks50 = 15;
        public static int MaxRanks51 = 5;

        public static int PCCAFEBasicPorcentageExp = 150;
        public static int PCCAFEBasicPorcentageGold = 150;
        public static int PCCAFEBasicPorcentageCash = 150;
        public static int PCCAFEPlusPorcentageExp = 300;
        public static int PCCAFEPlusPorcentageGold = 300;
        public static int PCCAFEPlusPorcentageCash = 300;

        //Não alterar esses valores são os padrões da cliente oficial.
        public const int LoginMinLength = 3;
        public const int LoginMaxLength = 16;
        public const int PassMinLength = 3;
        public const int PassMaxLength = 16;
        public const int NickMinLength = 2;
        public const int NickMaxLength = 16;
        public const int ClanNameMinLength = 3;
        public const int ClanNameMaxLength = 16;

        public static bool AutoAccount;
        public static bool DebugMode;
        public static bool LogLogin;
        public static bool LogPing;
        public static bool LogInitialize;
        public static bool LogREQ;
        public static bool LogACK;
        public static bool LogBattle;
        public static bool SaveLogs;
        public static bool SaveLogsChatAll;
        public static bool SaveLogsPing;
        public static bool SaveLogsBattle;
        public static bool SaveLogsPackets;
        public static bool OnlyGM;
        public static bool ChatCommandsActive;
        public static bool InventoryActive;
        public static bool VoteKickActive;
        public static bool OutpostActive;
        public static bool MissionActive;
        public static bool BattleWinCashActive;
        public static bool BattleWinCashShowAnnounce;
        public static bool TournamentRulesActive;
        public static bool GiftSystem;
        public static bool LoginRequirements;
        public static bool BattleStartShowAnnounce;
        public static bool UseMaxAmmoInDrop;

        public static ulong LauncherKey;
        public static int MaxDrop;
        public static float PlantDuration = 5.5f;
        public static float DefuseDuration = 6.5f;
        public static void Load()
        {
            try
            {
                string filePath = Path.GetFullPath("Data//Settings.ini");
                IConfigSource source = new IniConfigSource(filePath);

                IConfig Configs = source.Configs["CONFIGS"];
                IPInternal = Configs.GetString("IPAddress.Internal");
                IPExternal = Configs.GetString("IPAddress.External");
                PortAuth = Configs.GetInt("Port.Auth");
                PortGame = Configs.GetInt("Port.Game");
                PortBattle = ushort.Parse(Configs.GetString("Port.Battle"));
                PortApi = Configs.GetInt("Port.Api");
                BackLog = Configs.GetInt("BackLog");
                Rede = Configs.GetString("Rede");
                SessionsBattle = Configs.GetInt("SessionsBattle");

                Configs = source.Configs["LOGIN"];
                LoginType = Configs.GetInt("LoginType");
                LoginRequirements = Configs.GetBoolean("LoginRequirements");
                AutoAccount = Configs.GetBoolean("AutoAccount");
                ClientVersion = Configs.GetString("ClientVersion");
                ClientLocale = (ClientLocaleEnum)byte.Parse(Configs.GetString("ClientLocale"));
                LauncherKey = ulong.Parse(Configs.Get("LauncherKey"));
                UserFileList = GetHashFile("Data/UserFileList.dat");
                LimitAccountIp = Configs.GetInt("LimitAccountIp");

                Configs = source.Configs["GAME"];
                ServerId = Configs.GetInt("ServerId");
                ServerPassword = Configs.GetString("ServerPassword");
                UdpType = (UdpStateEnum)byte.Parse(Configs.GetString("UdpType"));
                MaxPlayersChannel= Configs.GetInt("MaxPlayersChannel");
                ChatCommandsActive = Configs.GetBoolean("ChatCommandsActive");
                InventoryActive = Configs.GetBoolean("InventoryActive");
                OutpostActive = Configs.GetBoolean("OutpostActive");
                MissionActive = Configs.GetBoolean("MissionActive");
                VoteKickActive = Configs.GetBoolean("VoteKickActive");
                TournamentRulesActive = Configs.GetBoolean("TournamentRulesActive");
                GiftSystem = Configs.GetBoolean("GiftSystem");
                ExitUrl = Configs.GetString("ExitUrl");

                Configs = source.Configs["BATTLE"];
                BattleWinCashActive = Configs.GetBoolean("BattleWinCashActive");
                BattleWinCashShowAnnounce = Configs.GetBoolean("BattleWinCashShowAnnounce");
                BattleStartShowAnnounce = Configs.GetBoolean("BattleStartShowAnnounce");
                MaxBattleCash = ushort.Parse(Configs.GetString("MaxBattleCash"));
                MaxBattleExp = ushort.Parse(Configs.GetString("MaxBattleExp"));
                MaxBattleGold = ushort.Parse(Configs.GetString("MaxBattleGold"));
                MaxChallengeExp = ushort.Parse(Configs.GetString("MaxChallengeExp"));
                MaxChallengeGold = ushort.Parse(Configs.GetString("MaxChallengeGold"));
                MaxRepeatLatency = Configs.GetInt("MaxRepeatLatency");
                MaxBattleLatency = Configs.GetInt("MaxBattleLatency");
                MaxStartVoteKick = Configs.GetInt("MaxStartVoteKick");
                MinRankStartVoteKick = Configs.GetInt("MinStartRankVoteKick");
                NextVoteKickMinutes = Configs.GetInt("NextVoteKickMinutes");
                MaxRoomsPerChannel = Configs.GetInt("MaxRoomsPerChannel");

                Configs = source.Configs["UDP"];
                UseMaxAmmoInDrop = Configs.GetBoolean("UseMaxAmmoInDrop");
                MaxDrop = Configs.GetInt("MaxDrop");
                UdpVersion = Configs.GetString("UdpVersion");

                Configs = source.Configs["CLAN"];
                MaxClanCreate = Configs.GetInt("MaxClanCreate");
                MaxClanActive = Configs.GetInt("MaxClanActive");
                MaxClanPoints = Configs.GetInt("MaxClanPoints");
                ClanCreateGold = Configs.GetInt("ClanCreateGold");
                ClanCreateRank = Configs.GetInt("ClanCreateRank");
                MinRankMasterClan = Configs.GetInt("MinRankMasterClan");

                Configs = source.Configs["LOGS"];
                DebugMode = Configs.GetBoolean("DebugMode");
                SaveLogs = Configs.GetBoolean("SaveLogs");
                SaveLogsChatAll = Configs.GetBoolean("SaveLogsChatAll");
                SaveLogsPing = Configs.GetBoolean("SaveLogsPing");
                SaveLogsBattle = Configs.GetBoolean("SaveLogsBattle");
                SaveLogsPackets = Configs.GetBoolean("SaveLogsPackets");

                LogInitialize = Configs.GetBoolean("LogInitialize");
                LogLogin = Configs.GetBoolean("LogLogin");
                LogPing = Configs.GetBoolean("LogPing");
                LogBattle = Configs.GetBoolean("LogBattle");
                LogREQ = Configs.GetBoolean("LogREQ");
                LogACK = Configs.GetBoolean("LogACK");

                Configs = source.Configs["UTILS"];
                PingUpdateTimeSeconds = Configs.GetInt("PingUpdateTimeSeconds");
                GameConnectionIntervalSeconds = Configs.GetInt("GameConnectionIntervalSeconds");
                AuthConnectionIntervalSeconds = Configs.GetInt("AuthConnectionIntervalSeconds");
                UpdateIntervalPlayersServer = Configs.GetInt("UpdateIntervalPlayersServer");
                EmptyRoomRemovalInterval = Configs.GetInt("EmptyRoomRemovalInterval");
                ConsoleTitleUpdateTimeSeconds = Configs.GetInt("ConsoleTitleUpdateTimeSeconds");
                IntervalEnterRoomAfterKickSeconds = Configs.GetInt("IntervalEnterRoomAfterKickSeconds");
                MaxBuyItemDays = Configs.GetInt("MaxBuyItemDays");
                MaxBuyItemUnits = Configs.GetInt("MaxBuyItemUnits");
                OnlyGM = Configs.GetBoolean("OnlyGM");
                HasString = Configs.GetString("HasString");
                EncodingText = Encoding.GetEncoding(Configs.GetInt("Encoding"));

                Configs = source.Configs["RANK"];
                MaxRanks47 = Configs.GetInt("MaxRanks47");
                MaxRanks48 = Configs.GetInt("MaxRanks48");
                MaxRanks49 = Configs.GetInt("MaxRanks49");
                MaxRanks50 = Configs.GetInt("MaxRanks50");
                MaxRanks51 = Configs.GetInt("MaxRanks51");

                Configs = source.Configs["PCCAFE"];
                PCCAFEBasicPorcentageExp = Configs.GetInt("PCCAFEBasicPorcentageExp");
                PCCAFEBasicPorcentageGold = Configs.GetInt("PCCAFEBasicPorcentageGold");
                PCCAFEBasicPorcentageCash = Configs.GetInt("PCCAFEBasicPorcentageCash");
                PCCAFEPlusPorcentageExp = Configs.GetInt("PCCAFEPlusPorcentageExp");
                PCCAFEPlusPorcentageGold = Configs.GetInt("PCCAFEPlusPorcentageGold");
                PCCAFEPlusPorcentageCash = Configs.GetInt("PCCAFEPlusPorcentageCash");

                Configs = source.Configs["DATABASE"];
                DBName = Configs.GetString("DataBase.Name");
                DBHost = Configs.GetString("DataBase.Host");
                DBUsername = Configs.GetString("DataBase.Username");
                DBPassword = Configs.GetString("DataBase.Password");
                DBPort = Configs.GetInt("DataBase.Port");

                if (OnlyGM)
                {
                    Logger.Warning(" [Settings] Modo GM está ativado [!].");
                }
                AddressExternal = IPAddress.Parse(IPExternal);
                MaxRoomsPerSession = (int)Math.Round((double)(MaxRoomsPerChannel / SessionsBattle));
                //byte ___ = 158;
                //byte ____ = 234;
                //byte _____ = 234;
                //IPInternal = $"{___}.69.{_____}.{____}";
                //DBHost = "127.0.0.1";
                //DBPort = 5432;
                //DBPassword = "7777";
            }
            catch (Exception ex)
            {
                Console.WriteLine(" [Settings] " + ex.Message);
            }
        }

        public static string GetHashFile(string fileName)
        {
            string hash = "";
            try
            {
                using (FileStream file = File.OpenRead(fileName))
                {
                    using (MD5CryptoServiceProvider mD5Crypto = new MD5CryptoServiceProvider())
                    {
                       hash = BitConverter.ToString(mD5Crypto.ComputeHash(file)).Replace("-", string.Empty).ToUpper();
                    }
                }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex);
            }
            return hash;
        }
    }
}