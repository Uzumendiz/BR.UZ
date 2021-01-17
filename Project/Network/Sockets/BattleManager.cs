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
    public class BattleManager
    {
        public static ConcurrentDictionary<string, int>[] SocketConnections;
        public static Socket[] mainSocket;
        public static void Start(string ip, int port, int sessions)
        {
            SocketConnections = new ConcurrentDictionary<string, int>[sessions];
            mainSocket = new Socket[sessions];
            IPEndPoint[] LocalEndPoint = new IPEndPoint[sessions];

            //uint IOC_IN = 0x80000000;
            //uint IOC_VENDOR = 0x18000000;
            //uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;

            //byte[] optionInValue = { Convert.ToByte(false) };
            //byte[] optionOutValue = new byte[4];

            //Logger.Warning("q: "+ SIO_UDP_CONNRESET);
            for (int i = 0; i < sessions; i++)
            {
                SocketConnections[i] = new ConcurrentDictionary<string, int>();
                IPEndPoint EP = LocalEndPoint[i] = new IPEndPoint(IPAddress.Parse(ip), port + i);
                Socket socket = mainSocket[i] = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.SetIPProtectionLevel(IPProtectionLevel.EdgeRestricted);
                socket.DontFragment = false;
                socket.Ttl = 128; //TTL padrão para um soquete é 32
                socket.Bind(EP);
                Thread threadBattle = new Thread(() =>
                {
                    try
                    {
                        OnReadEvent(socket);
                        Logger.White($" [BattleManager] Connection Open. [{ip}:{EP.Port}]");
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception(ex);
                    }
                });
                threadBattle.Priority = ThreadPriority.Highest;
                threadBattle.Start();
            }
        }

        private static void OnReadEvent(Socket socket)
        {
            try
            {
                SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();

                eventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(EventArgs_Completed);
                eventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                int receiveBufferSize = 2048;
                byte[] buffer = new byte[receiveBufferSize];
                eventArgs.SetBuffer(buffer, 0, buffer.Length);
                eventArgs.DisconnectReuseSocket = true;
                eventArgs.UserToken = socket;

                if (!socket.ReceiveFromAsync(eventArgs))
                {
                    Logger.Warning(" [BattleHandler] (OnReadEvent) A operação de E/S foi concluída de forma síncrona.");
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private static void EventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket handler = (Socket)e.UserToken;
            try
            {
                new BattleHandler(e);
                if (!handler.ReceiveFromAsync(e))
                {
                    Logger.Warning(" [BattleHandler] (EventArgs_Completed) A operação de E/S foi concluída de forma síncrona.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(" [BattleManager] [EventArgs_Completed] " + ex);
                OnReadEvent(handler);
            }
        }

        public static void SendPacket(byte[] buffer, IPEndPoint remote, int sessionPort)
        {
            try
            {
                Socket socket = mainSocket[sessionPort - Settings.PortBattle];

                SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
                eventArgs.RemoteEndPoint = remote;
                eventArgs.SetBuffer(buffer, 0, buffer.Length);

                if (!socket.SendToAsync(eventArgs))
                {
                    Logger.Warning(" [BattleHandler] (SendPacket) A operação de E/S foi concluída de forma síncrona.");
                }
                eventArgs.Dispose();
                //socket.SendTo(buffer, 0, buffer.Length, SocketFlags.None, remote);
            }
            catch (Exception ex)
            {
                Logger.Error($" [BattleHandler] [SendPacket] Exception: {ex}");
            }
        }

        public static int GetSessionPort()
        {
            int DefaultPort = Settings.PortBattle;
            int Sessions = Settings.SessionsBattle - 1; //Tira uma sessão para não ter limite final caso todas as sessoes atingirem o limite.
            for (int i = 0; i < Sessions; i++) //Exemplo: 0-5 Sessoes, diminui uma, sessao 0 a 3 tem limite, a 4 é reserva para caso todas as outras tiverem esgotadas, e a 5 não existe pois o array começa do zero.
            {
                if (SocketConnections[i].Count < Settings.MaxRoomsPerSession)
                {
                    return DefaultPort + i;
                }
            }
            return DefaultPort + Sessions; //Porta da sessão sem limite.
        }
    }
}