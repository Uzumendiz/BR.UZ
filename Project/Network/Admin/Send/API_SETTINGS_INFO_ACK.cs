namespace PointBlank.Api
{
    public class API_SETTINGS_INFO_ACK : ApiPacketWriter
    {
        public override void Write()
        {
            WriteH(2); //Opcode
            WriteC((byte)Settings.IPInternal.Length);
            WriteS(Settings.IPInternal, Settings.IPInternal.Length);
            WriteC((byte)Settings.IPExternal.Length);
            WriteS(Settings.IPExternal, Settings.IPExternal.Length);
            WriteC((byte)Settings.Rede.Length);
            WriteS(Settings.Rede, Settings.Rede.Length);
            WriteC((byte)Settings.ExitUrl.Length);
            WriteS(Settings.ExitUrl, Settings.ExitUrl.Length);
            WriteC((byte)Settings.UserFileList.Length);
            WriteS(Settings.UserFileList, Settings.UserFileList.Length);
            WriteC((byte)Settings.ClientVersion.Length);
            WriteS(Settings.ClientVersion, Settings.ClientVersion.Length);
            WriteC((byte)Settings.ServerPassword.Length);
            WriteS(Settings.ServerPassword, Settings.ServerPassword.Length);
            WriteC((byte)Settings.UdpVersion.Length);
            WriteS(Settings.UdpVersion, Settings.UdpVersion.Length);
            WriteC((byte)Settings.UdpType);
            WriteC((byte)Settings.ClientLocale);

            WriteD(Settings.PortApi);
            WriteD(Settings.PortAuth);
            WriteD(Settings.PortGame);
            WriteD(Settings.PortBattle);
            WriteD(Settings.DBPort);
            WriteD(Settings.BackLog);
            WriteD(Settings.ServerId);
            WriteD(Settings.LoginType);
            WriteD(Settings.MaxPlayersChannel);

            WriteH(Settings.MaxBattleExp);
            WriteH(Settings.MaxBattleGold);
            WriteH(Settings.MaxBattleCash);
            WriteH(Settings.MaxChallengeExp);
            WriteH(Settings.MaxChallengeGold);

            WriteD(Settings.MaxRoomsPerChannel);
            WriteD(Settings.MaxStartVoteKick);
            WriteD(Settings.MinRankStartVoteKick);
            WriteD(Settings.MinRankMasterClan);
            WriteD(Settings.NextVoteKickMinutes);
            WriteD(Settings.PingUpdateTimeSeconds);
            WriteD(Settings.PlayersServerUpdateTimeSeconds);
            WriteD(Settings.AuthConnectionIntervalSeconds);
            WriteD(Settings.GameConnectionIntervalSeconds);
            WriteD(Settings.UpdateIntervalPlayersServer);
            WriteD(Settings.EmptyRoomRemovalInterval);
            WriteD(Settings.MaxBuyItemDays);
            WriteD(Settings.MaxBuyItemUnits);
            WriteD(Settings.MaxRepeatLatency);
            WriteD(Settings.MaxBattleLatency);
            WriteD(Settings.LimitAccountIp);
            WriteD(Settings.MaxClanCreate);
            WriteD(Settings.MaxClanActive);
            WriteD(Settings.ClanCreateRank);
            WriteD(Settings.ClanCreateGold);
            WriteD(Settings.MaxRanks47);
            WriteD(Settings.MaxRanks48);
            WriteD(Settings.MaxRanks49);
            WriteD(Settings.MaxRanks50);
            WriteD(Settings.MaxRanks51);
            WriteD(Settings.PCCAFEBasicPorcentageExp);
            WriteD(Settings.PCCAFEBasicPorcentageGold);
            WriteD(Settings.PCCAFEBasicPorcentageCash);
            WriteD(Settings.PCCAFEPlusPorcentageExp);
            WriteD(Settings.PCCAFEPlusPorcentageGold);
            WriteD(Settings.PCCAFEPlusPorcentageCash);

            WriteC(Settings.AutoAccount);
            WriteC(Settings.DebugMode);
            WriteC(Settings.LogLogin);
            WriteC(Settings.LogPing);
            WriteC(Settings.LogInitialize);
            WriteC(Settings.LogREQ);
            WriteC(Settings.LogACK);
            WriteC(Settings.LogBattle);
            WriteC(Settings.SaveLogs);
            WriteC(Settings.SaveLogsChatAll);
            WriteC(Settings.SaveLogsPing);
            WriteC(Settings.SaveLogsBattle);
            WriteC(Settings.SaveLogsPackets);
            WriteC(Settings.OnlyGM);
            WriteC(Settings.ChatCommandsActive);
            WriteC(Settings.InventoryActive);
            WriteC(Settings.VoteKickActive);
            WriteC(Settings.OutpostActive);
            WriteC(Settings.MissionActive);
            WriteC(Settings.BattleWinCashActive);
            WriteC(Settings.BattleWinCashShowAnnounce);
            WriteC(Settings.TournamentRulesActive);
            WriteC(Settings.GiftSystem);
            WriteC(Settings.LoginRequirements);
            WriteC(Settings.BattleStartShowAnnounce);
            WriteC(Settings.UseMaxAmmoInDrop);

            WriteQ(Settings.LauncherKey);
            WriteD(Settings.MaxDrop);
        }
    }
}