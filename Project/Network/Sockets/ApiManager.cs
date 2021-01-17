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
    public class ApiManager
    {
        public static Socket mainSocket;
        public static ConcurrentDictionary<int, ApiClient> SocketSessions = new ConcurrentDictionary<int, ApiClient>();
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
                Logger.White($" [ApiManager] Connection Open. [{ip}:{port}]");

                Thread threadApi = new Thread(() =>
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
                threadApi.Priority = ThreadPriority.Highest;
                threadApi.Start();
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
                if ((DateTime.Now - Logger.LastSaveLogTcpAuth1).Minutes >= 1)
                {
                    Logger.Error($" [ApiManager] [AcceptCallback] Date ({DateTime.Now}) Exception: {ex.Message}");
                    Logger.LastSaveLogTcpAuth1 = DateTime.Now;
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
                ApiClient client = new ApiClient(handler);
                for (int idx = 1; idx <= 10; idx++)
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
                Logger.Error($" [ApiManager] [AddSession] Não foi possivel adicionar a lista de sessões. IPAddress ({address}) Date: {date}");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public static int SendPacketToAllClients(ApiPacketWriter packet)
        {
            int count = 0;
            try
            {
                if (SocketSessions.Count == 0)
                {
                    return count;
                }
                byte[] code = packet.GetCompleteBytes("ApiManager.SendPacketToAllClients");
                foreach (ApiClient client in SocketSessions.Values)
                {
                    if (client.SessionSocket != null && client.SessionSocket.Connected)
                    {
                        client.SendCompletePacket(code);
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
        public static bool RemoveSession(ApiClient client)
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
    }
}