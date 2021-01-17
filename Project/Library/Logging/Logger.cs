using System;
using System.IO;

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
    public class Logger
    {
        public static DateTime LastSaveLogTcpAuth1;
        public static DateTime LastSaveLogTcpAuth2;
        public static DateTime LastSaveLogTcpGame1;
        public static DateTime LastSaveLogTcpGame2;
        public static DateTime LastSaveLogUdpBattle1;
        public static DateTime LastSaveLogUdpBattle2;
        public static DateTime LastSaveLogUdpBattle3;
        public static bool Problem = false;
        private static StreamWriter SW_LOGIN;
        private static StreamWriter SW_WANING;
        private static StreamWriter SW_ERROR;
        private static StreamWriter SW_EXCEPTION;
        private static StreamWriter SW_ANALYZE;
        private static StreamWriter SW_ROOM;
        private static StreamWriter SW_CHAT_COMMANDS;
        private static StreamWriter SW_CHAT_ALL;
        private static StreamWriter SW_DEBUG;
        private static StreamWriter SW_BATTLE;
        private static StreamWriter SW_PING;
        private static StreamWriter SW_PACKETS;
        private static StreamWriter SW_ATTACKS;
        public static void Start()
        {
            try
            {
                DateTime date = new DateTime();
                LastSaveLogTcpAuth1 = date;
                LastSaveLogTcpAuth2 = date;
                LastSaveLogTcpGame1 = date;
                LastSaveLogTcpGame2 = date;
                LastSaveLogUdpBattle1 = date;
                LastSaveLogUdpBattle2 = date;
                LastSaveLogUdpBattle3 = date;
                string[] Directorys = new string[] { "Logs/", "Logs/Error", "Logs/Login", "Logs/Room", "Logs/Chat", "Logs/Analyze", "Logs/Debug", "Logs/Battle", "Logs/Packets", "Logs/Attacks" };
                for (int i = 0; i < Directorys.Length; i++)
                {
                    string local = Directorys[i];
                    if (!Directory.Exists(local))
                    {
                        Directory.CreateDirectory(local);
                    }
                }
                SW_LOGIN = new StreamWriter("Logs/Login//LOGINS.log", true);
                SW_WANING = new StreamWriter("Logs/Error//WARNINGS.log", true);
                SW_ERROR = new StreamWriter("Logs/Error//ERRORS.log", true);
                SW_EXCEPTION = new StreamWriter("Logs/Error//EXCEPTIONS.log", true);
                SW_ANALYZE = new StreamWriter("Logs/Analyze//ANALYZES.log", true);
                SW_ROOM = new StreamWriter("Logs/Room//ROOMS.log", true);
                SW_CHAT_COMMANDS = new StreamWriter("Logs/Chat//CHAT_COMMANDS.log", true);
                SW_CHAT_ALL = new StreamWriter("Logs/Chat//CHAT_ALL.log", true);
                SW_DEBUG = new StreamWriter("Logs/Debug//DEBUG.log", true);
                SW_BATTLE = new StreamWriter("Logs/Battle//BATTLE.log", true);
                SW_PING = new StreamWriter("Logs/Battle//PING.log", true);
                SW_PACKETS = new StreamWriter("Logs/Packets//PACKETS.log", true);
                SW_ATTACKS = new StreamWriter("Logs/Attacks//ALL.log", true);
            }
            catch (Exception ex)
            {
                Red(" [Logger] " + ex.ToString());
            }
        }

        public static void White(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void Yellow(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void Red(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void Blue(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void Gray(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void Cyan(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void Black(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void DarkMagenta(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void DarkYellow(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void DarkGray(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void DarkGreen(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void DarkRed(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void DarkBlue(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void DarkCyan(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void Info(string text)
        {
            try
            {
                Console.WriteLine(text);
            }
            catch
            {
            }
        }

        public static void Success(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void LineColor(string text, ConsoleColor back, ConsoleColor fore)
        {
            try
            {
                Console.BackgroundColor = back;
                Console.WriteLine(text, fore);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void Warning(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(text);
                Console.ResetColor();
                if (!Settings.SaveLogs)
                {
                    return;
                }
                SW_WANING.WriteLine($"[{DateTime.Now}] {text}");
                SW_WANING.Flush();
            }
            catch
            {
            }
        }

        public static void Informations(string text)
        {
            if (Settings.LogInitialize)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }

        public static void PacketREQ(string text)
        {
            try
            {
                if (Settings.LogREQ)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(text);
                    Console.ResetColor();
                }
                if (Settings.SaveLogsPackets)
                {
                    SW_PACKETS.WriteLine(text);
                    SW_PACKETS.Flush();
                }
            }
            catch
            {
            }
        }

        public static void PacketACK(string text)
        {
            try
            {
                if (Settings.LogACK)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(text);
                    Console.ResetColor();
                }
                if (Settings.SaveLogsPackets)
                {
                    SW_PACKETS.WriteLine(text);
                    SW_PACKETS.Flush();
                }
            }
            catch
            {
            }
        }

        public static void InfoPing(string text)
        {
            try
            {
                if (Settings.LogPing)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(text);
                    Console.ResetColor();
                }
                if (Settings.SaveLogsPing)
                {
                    SW_PING.WriteLine($"[{DateTime.Now}] {text}");
                    SW_PING.Flush();
                }
            }
            catch
            {
            }
        }

        public static void Error(string text)
        {
            try
            {
                Problem = true;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(text);
                Console.ResetColor();
                if (Settings.SaveLogs)
                {
                    SW_ERROR.WriteLine($"[{DateTime.Now}] {text}");
                    SW_ERROR.Flush();
                }
            }
            catch
            {
            }
        }

        public static void Exception(Exception ex)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
                if (Settings.SaveLogs)
                {
                    SW_EXCEPTION.WriteLine($"[{DateTime.Now}] {ex}");
                    SW_EXCEPTION.Flush();
                }
            }
            catch
            {
            }
        }

        public static void Login(string text)
        {
            try
            {
                if (Settings.LogLogin)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(text);
                    Console.ResetColor();
                }
                if (Settings.SaveLogs)
                {
                    SW_LOGIN.WriteLine($"[{DateTime.Now}] {text}");
                    SW_LOGIN.Flush();
                }
            }
            catch
            {
            }
        }

        public static void Room(string text)
        {
            try
            {
                //Console.ForegroundColor = ConsoleColor.DarkYellow;
                //Console.WriteLine(text);
                //Console.ResetColor();
                if (Settings.SaveLogs)
                {
                    SW_ROOM.WriteLine(text);
                    SW_ROOM.Flush();
                }
            }
            catch
            {
            }
        }

        public static void ChatCommands(string text)
        {
            try
            {
                if (Settings.SaveLogs)
                {
                    SW_CHAT_COMMANDS.WriteLine($"[{DateTime.Now}] {text}");
                    SW_CHAT_COMMANDS.Flush();
                }
            }
            catch
            {
            }
        }

        public static void ChatAll(string text)
        {
            try
            {
                if (Settings.SaveLogsChatAll)
                {
                    SW_CHAT_ALL.WriteLine($"[{DateTime.Now}] {text}");
                    SW_CHAT_ALL.Flush();
                }
            }
            catch
            {
            }
        }

        public static void Attacks(string text)
        {
            try
            {
                Console.WriteLine(text);
                SW_ATTACKS.WriteLine(text);
                SW_ATTACKS.Flush();
            }
            catch
            {
            }
        }


        public static void Analyze(string text)
        {
            try
            {
                //Console.ForegroundColor = ConsoleColor.DarkYellow;
                //Console.WriteLine(text);
                //Console.ResetColor();
                if (Settings.SaveLogs)
                {
                    SW_ANALYZE.WriteLine($"[{DateTime.Now}] {text}");
                    SW_ANALYZE.Flush();
                }
            }
            catch
            {
            }
        }

        public static void Battle(string text)
        {
            try
            {
                if (Settings.LogBattle)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(text);
                    Console.ResetColor();
                }
                if (Settings.SaveLogsBattle)
                {
                    SW_BATTLE.WriteLine(text);
                    SW_BATTLE.Flush();
                }
            }
            catch
            {
            }
        }

        public static void Temporary(string text)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(text);
                Console.ResetColor();
            }
            catch
            {
            }
        }

        public static void Debug(string text)
        {
            try
            {
                string log = $" [{DateTime.Now}] [Debug] {text}";
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(log);
                Console.ResetColor();
                if (Settings.SaveLogs)
                {
                    SW_DEBUG.WriteLine(log);
                    SW_DEBUG.Flush();
                }
            }
            catch
            {
            }
        }

        public static void DebugPacket(string packet, string text)
        {
            try
            {
                string log = $" [{DateTime.Now}] [Debug] [{packet}] {text}";
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(log);
                Console.ResetColor();
                if (Settings.SaveLogs)
                {
                    SW_DEBUG.WriteLine(log);
                    SW_DEBUG.Flush();
                }
            }
            catch
            {
            }
        }
    }
}