using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
    public static class GameManager
    {
        public static Socket mainSocket;
        public static ConcurrentDictionary<int, GameClient> SocketSessions = new ConcurrentDictionary<int, GameClient>();
        public static ConcurrentDictionary<string, DateTime> SocketConnections = new ConcurrentDictionary<string, DateTime>();
        public static void Start(string ip, int port)
        {
            try
            {
                mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint Local = new IPEndPoint(IPAddress.Parse(ip), port);
                mainSocket.SetIPProtectionLevel(IPProtectionLevel.EdgeRestricted);
                mainSocket.DontFragment = false;
                mainSocket.NoDelay = true;
                mainSocket.Bind(Local);
                mainSocket.Listen(Settings.BackLog);
                Logger.White($" [GameManager] Connection Open. [{ip}:{port}]");

                Thread threadGame = new Thread(() =>
                {
                    try
                    {
                        mainSocket.BeginAccept(new AsyncCallback(AcceptCallback), mainSocket);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex);
                    }
                });
                threadGame.Priority = ThreadPriority.Highest;
                threadGame.Start();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private static void AcceptCallback(IAsyncResult result)
        {
            Socket clientSocket = (Socket)result.AsyncState;
            Socket handler = null;
            try
            {
                handler = clientSocket.EndAccept(result);
            }
            catch (Exception ex)
            {
                if ((DateTime.Now - Logger.LastSaveLogTcpGame1).Minutes >= 1)
                {
                    Logger.Error($" [GameManager] [AcceptCallback] Date ({DateTime.Now}) Exception: {ex.Message}");
                    Logger.LastSaveLogTcpGame1 = DateTime.Now;
                }
            }
            try
            {
                Thread.Sleep(5);
                mainSocket.BeginAccept(new AsyncCallback(AcceptCallback), mainSocket);
                if (handler != null)
                {
                    AddSession(handler);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public static void AddSession(Socket handler)
        {
            try
            {
                string address = GetIPAddress(handler);
                DateTime date = DateTime.Now;
                if (!SocketConnections.ContainsKey(address) && SocketConnections.TryAdd(address, date) || SocketConnections.TryGetValue(address, out DateTime getDate) && (date - getDate).TotalSeconds >= Settings.GameConnectionIntervalSeconds && SocketConnections.TryUpdate(address, date, getDate))
                {
                    GameClient client = new GameClient(handler);
                    for (int idx = 1; idx < 100000; idx++)
                    {
                        if (!SocketSessions.ContainsKey(idx) && SocketSessions.TryAdd(idx, client))
                        {
                            client.SessionId = idx;
                            client.SessionDate = date;
                            client.StartSession();
                            return;
                        }
                    }
                    client.Close(50);
                    Logger.Error($" [GameManager] [AddSession] Não foi possivel adicionar a lista de sessões. IPAddress ({address}) Date: {date}");
                }
                else
                {
                    if ((date - Logger.LastSaveLogTcpGame2).Minutes >= 1)
                    {
                        Logger.Attacks($" [GameManager] Está conexão está bloqueada por {Settings.GameConnectionIntervalSeconds} segundos. IP ({address}) Date ({date})");
                        Logger.LastSaveLogTcpGame2 = date;
                    }
                    handler.Close(50);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
        private static string GetIPAddress(Socket handler)
        {
            try
            {
                if (handler != null && handler.RemoteEndPoint != null)
                {
                    return ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return "";
        }
        public static bool RemoveSession(GameClient client)
        {
            try
            {
                if (client == null || client.SessionId == 0)
                {
                    return false;
                }
                if (SocketSessions.ContainsKey(client.SessionId) && SocketSessions.TryGetValue(client.SessionId, out client))
                {
                    return SocketSessions.TryRemove(client.SessionId, out client);
                }
                client = null;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return false;
        }

        public static bool RemoveConnection(string address)
        {
            try
            {
                if (SocketConnections.ContainsKey(address) && SocketConnections.TryGetValue(address, out DateTime date))
                {
                    return SocketConnections.TryRemove(address, out date);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return false;
        }

        public static int SendPacketToAllClients(GamePacketWriter packet)
        {
            int count = 0;
            try
            {
                if (SocketSessions.Count == 0)
                {
                    return count;
                }

                byte[] code = packet.GetCompleteBytes("GameManager.SendPacketToAllClients");
                foreach (GameClient client in SocketSessions.Values)
                {
                    Account player = client.SessionPlayer;
                    if (player != null && player.isOnline)
                    {
                        player.SendCompletePacket(code);
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return count;
        }
        public static Account SearchActiveClient(long accountId)
        {
            try
            {
                if (SocketSessions.Count == 0)
                {
                    return null;
                }
                foreach (GameClient client in SocketSessions.Values)
                {
                    Account player = client.SessionPlayer;
                    if (player != null && player.playerId == accountId)
                    {
                        return player;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return null;
        }
        public static Account SearchActiveClient(uint sessionId)
        {
            try
            {
                if (SocketSessions.Count == 0)
                {
                    return null;
                }

                foreach (GameClient client in SocketSessions.Values)
                {
                    if (client.SessionPlayer != null && client.SessionId == sessionId)
                    {
                        return client.SessionPlayer;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return null;
        }
        public static int KickActiveClient()
        {
            int count = 0;
            try
            {
                DateTime now = DateTime.Now;
                foreach (GameClient client in SocketSessions.Values)
                {
                    Account pl = client.SessionPlayer;
                    if (pl != null && pl.room == null && pl.channelId > -1 && !pl.IsGM() && (now - pl.lastLobbyEnter).TotalHours >= 1)
                    {
                        count++;
                        pl.Close(5000);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return count;
        }
        public static int KickCountActiveClient(double Hours)
        {
            int count = 0;
            try
            {
                DateTime now = DateTime.Now;
                foreach (GameClient client in SocketSessions.Values)
                {
                    Account pl = client.SessionPlayer;
                    if (pl != null && pl.room == null && pl.channelId > -1 && !pl.IsGM() && (now - pl.lastLobbyEnter).TotalHours >= Hours)
                    {
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return count;
        }
    }
}