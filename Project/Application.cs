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

using NetFwTypeLib;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PointBlank
{
    public class Application
    {
        public static Mutex mutex = new Mutex(true, "{77REGERGE58D-7A21-76C-7E6A-7FE832F9834675346785367893678923456793456789367893567895694735678935679345696934567894569456937834567896943954C2}");
        public static DateTime StartDate = new DateTime();
        public static int Counts;
        public static int recordOnline = 0;
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainOnUnhandledException);
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelKeyPressEvent);
            try
            {
                //if (!IsAdministrator() || Console.NumberLock || Console.CapsLock)
                //{
                //    Environment.Exit(Environment.ExitCode);
                //    return;
                //}
                Console.SetWindowSize(125, 30);
                Console.CursorVisible = false;
                Console.TreatControlCAsInput = false; //true=Entrada de combinação de tecla comum. false=Interrupção pelo sistema.
                Console.Title = "</> BR.UZ </>";
                Process ProcessApplication = Process.GetCurrentProcess();
                if (mutex.WaitOne(TimeSpan.Zero, true))
                {
                    try
                    {
                        DateTime LiveDate = GetDate();
                        bool IsInvalidLicense = LiveDate == new DateTime() || long.Parse(LiveDate.ToString("yyMMddHHmmss")) >= 210620000000;
                        if (IsInvalidLicense)
                        {
                            Environment.Exit(Environment.ExitCode);
                            return;
                        }
                        Settings.Load();
                        Logger.Start();
                        Debugger.ShowLogo();

                        SQLManager.Load();
                        ServerBlockManager.Load();
                        EventLoader.LoadAll();
                        MissionsXML.Load();
                        ClanManager.Load();

                        ShopManager.Load();
                        DefaultInventoryManager.Load();
                        ServersManager.Load();
                        TitlesManager.Load();
                        MissionCardXML.LoadBasicCards();
                        RankManager.Load();
                        MapsXML.Load();
                        ClanRankXML.Load();
                        MissionAwards.Load();
                        TournamentRulesManager.Load();
                        RandomBoxXML.LoadBoxes();
                        CupomEffectManager.Load();
                        PackageDataManager.Load();
                        StringFilter.Load();
                        AddressFilter.Load();

                        //BATTLE
                        HalfUtils.Load();
                        WeaponsXML.Load();
                        CharaXML.Load();
                        MappingXML.Load();

                        FirewallSecurity.LoadInstances(ProcessApplication.ProcessName, Settings.SessionsBattle);
                        FirewallSecurity.CreateRuleAllow(FirewallSecurity.FirewallRuleNameApiTCP, "127.0.0.1/255.255.255.255", Settings.PortApi, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP); //Regra de permissão de acesso a especificos especificos endereços de ip na porta do Api.
                        FirewallSecurity.CreateRuleAllow(FirewallSecurity.FirewallRuleNameAuthTCP, "", Settings.PortAuth, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP); //Regra de permissão de acesso a todos endereços de ip na porta do Auth.
                        FirewallSecurity.CreateRuleAllow(FirewallSecurity.FirewallRuleNameGameTCP, "127.0.0.1/255.255.255.255", Settings.PortGame, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP); //Regra de permissão de acesso a todos endereços de ip na porta do Game que passaram na autenticação.
                        for (int i = 0; i < Settings.SessionsBattle; i++)
                        {
                            FirewallSecurity.CreateRuleAllow(FirewallSecurity.FirewallRuleNameBattleUDP[i], "127.0.0.1/255.255.255.255", Settings.PortBattle + i, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP); //Regra de permissão de acesso a todos endereços de ip na porta do Battle que passaram na autenticação e no Game.
                        }
                        Utilities.ExecuteQuery($"UPDATE accounts SET online='{false}'"); //Seta o online de todas as contas como false

                        AuthManager.Start(Settings.IPInternal, Settings.PortAuth);
                        GameManager.Start(Settings.IPInternal, Settings.PortGame);
                        BattleManager.Start(Settings.IPInternal, Settings.PortBattle, Settings.SessionsBattle);
                        ApiManager.Start(Settings.IPInternal, Settings.PortApi);

                        //new Thread(CodeInDevelopment).Start();

                        if (Logger.Problem)
                        {
                            Logger.Red(" [Application] Startup failed.");
                        }
                        else
                        {
                            Logger.White($" [Application] Date time of server: {StartDate = DateTime.Now}");
                            Logger.Success(" [Application] Loaded all components.");
                            new Thread(new ThreadStart(TitleInfo)).Start();
                        }
                        ProcessApplication.WaitForExit();
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }
                else
                {
                    MessageBox.Show("Console Application is already running.", ProcessApplication.ProcessName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" FATAL ERROR: " + ex);
                Console.ReadKey();
            }
        }

        private static void TitleInfo()
        {
            recordOnline = ServerInfoManager.GetRecordOnline();
            while (true)
            {
                Console.Title = $" BR.UZ   []   Auth: {AuthManager.SocketSessions.Count} Sessions: {GameManager.SocketSessions.Count}   []   Accounts: {AccountManager.accounts.Count}   []   Clans: {ClanManager.clans.Count}   []   Memory: {GC.GetTotalMemory(true) / 1024} KB   []   RecordOnline: {recordOnline}   []   ArrayIP: {Counts}   []   StartDate: {StartDate}   []";
                if (GameManager.SocketSessions.Count > recordOnline)
                {
                    recordOnline = GameManager.SocketSessions.Count;
                    Logger.Warning($" [Application] Server has reached a new goal for {recordOnline} online players");
                    Utilities.ExecuteQuery($"UPDATE server_info SET record_online='{recordOnline}'"); //Fazer o Update talvez só quando fechar o servidor?
                }
                Thread.Sleep(TimeSpan.FromSeconds(Settings.ConsoleTitleUpdateTimeSeconds));
            }
        }

        private static void CancelKeyPressEvent(object sender, ConsoleCancelEventArgs e)
        {
            Settings.Load();
            Logger.White(" [Application] Settings reloaded.");
            e.Cancel = true; //False = Exit
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            //processStartInfo.RedirectStandardInput = true;
            //processStartInfo.RedirectStandardOutput = true;
            //processStartInfo.UseShellExecute = false;
            //Process process = Process.Start(processStartInfo);
            //process.StandardInput.WriteLine(Environment.CurrentDirectory + @" start BR.UZ.exe");
            Logger.Error($" [Application] [CurrentDomainOnUnhandledException] [{DateTime.Now}] Sender: {sender} Terminating: {e.IsTerminating} {(Exception)e.ExceptionObject}");
        }

        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static DateTime GetDate()
        {
            try
            {
                using (WebResponse response = WebRequest.Create("http://www.google.com").GetResponse())
                {
                    return DateTime.ParseExact(response.Headers["date"], "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                }
            }
            catch
            {
                return new DateTime();
            }
        }
        private static object _sync = new object();
        private static void CodeInDevelopment()
        {
            string[] nicks = new string[] { "oi", "bebe", "kaka", "juju", "sim", "sei", "animal", "macaco", "coyote", "meudog" };
            foreach (string nickname in nicks)
            {
                new Thread(() =>
                {
                    //Monitor.Enter(AccountManager._sync);
                    //lock (AccountManager._sync)
                    {
                        Account ac1 = new Account();
                        {
                            ac1.playerId = 33;
                            bool exist = AccountManager.CheckNicknameExist2(nickname);
                            if (!exist)
                            {
                                Task<bool> taskUpdate = ac1.UpdateNick(nickname);
                                if (/*taskUpdate.IsCompleted && */taskUpdate.Result)
                                {
                                    Logger.Warning("AC1: NAO EXISTE O NICK E FOI ATUALIZADO: " + nickname);
                                }
                                else
                                {
                                    Logger.Warning("AC1: ERRO PARA ATUALIZAR: " + nickname);
                                }
                            }
                            else
                            {
                                Logger.Warning("AC1: EXISTE O NICKNAME: " + nickname);
                            }
                        }
                    }
                    //Monitor.Exit(AccountManager._sync);
                }).Start();
                new Thread(()=>
                {
                    //Monitor.Enter(AccountManager._sync);
                    //lock (AccountManager._sync)
                    {
                        Account ac2 = new Account();
                        {
                            ac2.playerId = 36;
                            bool exist = AccountManager.CheckNicknameExist2(nickname);
                            if (!exist)
                            {
                                Task<bool> taskUpdate = ac2.UpdateNick(nickname);
                                if (taskUpdate.Result)
                                {
                                    Logger.Warning("AC2: NAO EXISTE O NICK E FOI ATUALIZADO: " + nickname);
                                }
                                else
                                {
                                    Logger.Warning("AC2: ERRO PARA ATUALIZAR: " + nickname);
                                }
                            }
                            else
                            {
                                Logger.Warning("AC2: EXISTE O NICKNAME: " + nickname);
                            }
                        }
                    }
                    //Monitor.Exit(AccountManager._sync);
                }).Start();
            }

            //while (true)
            //{
            //    FirewallSecurity.AddRuleTcp(Debugger.RandomizeIP());
            //}
            //Stopwatch time = new Stopwatch();
            //time.Start();
            //time.Stop();
            //Logger.Warning("Tempo: " + time.ElapsedMilliseconds);

            //int LoadType = 3;
            //if ((LoadType & 1) == 1)
            //{
            //    Logger.Warning("1");
            //}
            //if ((LoadType & 2) == 2)
            //{
            //    Logger.Warning("2");
            //}
            //if ((LoadType & 4) == 4)
            //{
            //    Logger.Warning("4");
            //}
            //if ((LoadType & 8) == 8)
            //{
            //    Logger.Warning("8");
            //}
            //if ((LoadType & 16) == 16)
            //{
            //    Logger.Warning("16");
            //}
            //if ((LoadType & 32) == 32)
            //{
            //    Logger.Warning("32");
            //}
        }
    }
}