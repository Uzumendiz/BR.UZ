using System;

namespace PointBlank.Api
{
    public class API_SETTINGS_CHANGE_REQ : ApiPacketReader
    {
        public override void ReadImplement()
        {
            Settings.IPInternal = ReadString(ReadByte());
            Settings.IPExternal = ReadString(ReadByte());
            Settings.Rede = ReadString(ReadByte());
            Settings.ExitUrl = ReadString(ReadByte());
            Settings.UserFileList = ReadString(ReadByte());
            Settings.ClientVersion = ReadString(ReadByte());
            Settings.ServerPassword = ReadString(ReadByte());
            Settings.UdpVersion = ReadString(ReadByte());
            Settings.UdpType = (UdpStateEnum)ReadByte();
            Settings.ClientLocale = (ClientLocaleEnum)ReadByte();

            Settings.PortApi = ReadInt();
            Settings.PortAuth = ReadInt();
            Settings.PortGame = ReadInt();
            Settings.PortBattle = (ushort)ReadInt();
            Settings.DBPort = ReadInt();
            Settings.BackLog = ReadInt();
            Settings.ServerId = ReadInt();
            Settings.LoginType = ReadInt();
            Settings.MaxPlayersChannel = ReadInt();

            Settings.MaxBattleExp = ReadUshort();
            Settings.MaxBattleGold = ReadUshort();
            Settings.MaxBattleCash = ReadUshort();
            Settings.MaxChallengeExp = ReadUshort();
            Settings.MaxChallengeGold = ReadUshort();

            Settings.MaxRoomsPerChannel = ReadInt();
            Settings.MaxStartVoteKick = ReadInt();
            Settings.MinRankStartVoteKick = ReadInt();
            Settings.MinRankMasterClan = ReadInt();
            Settings.NextVoteKickMinutes = ReadInt();
            Settings.PingUpdateTimeSeconds = ReadInt();
            Settings.PlayersServerUpdateTimeSeconds = ReadInt();
            Settings.AuthConnectionIntervalSeconds = ReadInt();
            Settings.GameConnectionIntervalSeconds = ReadInt();
            Settings.UpdateIntervalPlayersServer = ReadInt();
            Settings.EmptyRoomRemovalInterval = ReadInt();
            Settings.MaxBuyItemDays = ReadInt();
            Settings.MaxBuyItemUnits = ReadInt();
            Settings.MaxRepeatLatency = ReadInt();
            Settings.MaxBattleLatency = ReadInt();
            Settings.LimitAccountIp = ReadInt();
            Settings.MaxClanCreate = ReadInt();
            Settings.MaxClanActive = ReadInt();
            Settings.ClanCreateRank = ReadInt();
            Settings.ClanCreateGold = ReadInt();
            Settings.MaxRanks47 = ReadInt();
            Settings.MaxRanks48 = ReadInt();
            Settings.MaxRanks49 = ReadInt();
            Settings.MaxRanks50 = ReadInt();
            Settings.MaxRanks51 = ReadInt();
            Settings.PCCAFEBasicPorcentageExp = ReadInt();
            Settings.PCCAFEBasicPorcentageGold = ReadInt();
            Settings.PCCAFEBasicPorcentageCash = ReadInt();
            Settings.PCCAFEPlusPorcentageExp = ReadInt();
            Settings.PCCAFEPlusPorcentageGold = ReadInt();
            Settings.PCCAFEPlusPorcentageCash = ReadInt();

            Settings.AutoAccount = ReadBool();
            Settings.DebugMode = ReadBool();
            Settings.LogLogin = ReadBool();
            Settings.LogPing = ReadBool();
            Settings.LogInitialize = ReadBool();
            Settings.LogREQ = ReadBool();
            Settings.LogACK = ReadBool();
            Settings.LogBattle = ReadBool();
            Settings.SaveLogs = ReadBool();
            Settings.SaveLogsChatAll = ReadBool();
            Settings.SaveLogsPing = ReadBool();
            Settings.SaveLogsBattle = ReadBool();
            Settings.SaveLogsPackets = ReadBool();
            Settings.OnlyGM = ReadBool();
            Settings.ChatCommandsActive = ReadBool();
            Settings.InventoryActive = ReadBool();
            Settings.VoteKickActive = ReadBool();
            Settings.OutpostActive = ReadBool();
            Settings.MissionActive = ReadBool();
            Settings.BattleWinCashActive = ReadBool();
            Settings.BattleWinCashShowAnnounce = ReadBool();
            Settings.TournamentRulesActive = ReadBool();
            Settings.GiftSystem = ReadBool();
            Settings.LoginRequirements = ReadBool();
            Settings.BattleStartShowAnnounce = ReadBool();
            Settings.UseMaxAmmoInDrop = ReadBool();

            Settings.LauncherKey = ReadUlong();
            Settings.MaxDrop = ReadInt();
        }

        public override void RunImplement()
        {
            Logger.Warning("Settings updated " + DateTime.Now);
            ApiManager.SendPacketToAllClients(new API_SETTINGS_INFO_ACK());
        }
    }
}