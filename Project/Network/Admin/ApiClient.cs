using Microsoft.Win32.SafeHandles;
using PointBlank.Api;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
    public class ApiClient : ClientApi, IDisposable
    {
        public Socket SessionSocket;
        public DateTime SessionDate;
        public int SessionId;
        private int SessionShift;
        public MachineModel machine;
        public Account admin;
        public bool ConnectionIsClosed = false;
        private bool Disposed = false;
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public ApiClient(Socket SessionSocket)
        {
            this.SessionSocket = SessionSocket;
        }

        public void StartSession()
        {
            try
            {
                SessionShift = SessionId % 7 + 1;
                new Thread(new ThreadStart(Initialize)).Start();
                new Thread(new ThreadStart(OnReadCallback)).Start();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                Close();
            }
        }

        private void Initialize()
        {
            try
            {
                GetAddressPort(out IPAddress Address, out int Port);
                SendPacket(new API_AUTH_ACK());
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                Close();
            }
        }

        #region RECEIVE

        private void OnReadCallback()
        {
            try
            {
                TcpPacket packet = new TcpPacket
                {
                    WorkSocket = SessionSocket
                };
                SessionSocket.BeginReceive(packet.Buffer, 0, TcpPacket.BufferSize, SocketFlags.None, new AsyncCallback(OnReceiveCallback), packet);
            }
            catch
            {
                Close();
            }
        }

        private void OnReceiveCallback(IAsyncResult result)
        {
            TcpPacket packet = (TcpPacket)result.AsyncState;
            try
            {
                int BytesSize = packet.WorkSocket.EndReceive(result);
                if (BytesSize > 0)
                {
                    byte[] BufferTotalEncrypted = new byte[BytesSize];
                    Array.Copy(packet.Buffer, 0, BufferTotalEncrypted, 0, BytesSize);

                    ushort PacketId = BitConverter.ToUInt16(BufferTotalEncrypted, 0);
                    //ushort PacketSeed = BitConverter.ToUInt16(BufferTotalEncrypted, 2);

                    byte[] BufferPacket = new byte[BytesSize - 2];
                    Array.Copy(BufferTotalEncrypted, 2, BufferPacket, 0, BufferPacket.Length);

                    if (ConnectionIsClosed)
                    {
                        return;
                    }
                    RunPacket(PacketId, BufferPacket);
                    new Thread(new ThreadStart(OnReadCallback)).Start();
                }
            }
            catch
            {
                Close();
            }
        }
        #endregion

        #region SEND
        public override void SendCompletePacket(byte[] data)
        {
            try
            {
                SessionSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), SessionSocket);
            }
            catch
            {
                Close();
            }
        }

        public override void SendPacket(ApiPacketWriter packet)
        {
            try
            {
                Logger.PacketACK($" [ApiClient] [SendPacket] {packet}");
                SendCompletePacket(packet.GetCompleteBytes(null));
                packet.memorystream.Close();
                packet.Dispose();
                packet = null;
            }
            catch
            {
                Close();
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            try
            {
                Socket handler = (Socket)result.AsyncState;
                if (handler != null && handler.Connected)
                {
                    handler.EndSend(result);
                }
            }
            catch
            {
                Close();
            }
        }
        #endregion

        private void RunPacket(ushort Opcode, byte[] Buffer)
        {
            try
            {
                ApiPacketReader Packet = null;
                switch (Opcode)
                {
                    case 1: Packet = new API_LOGIN_ADMIN_REQ(); break;
                    case 2: Packet = new API_GET_ONLINE_PLAYERS_REQ(); break;
                    case 3: Packet = new API_GET_SETTINGS_INFO_REQ(); break;
                    case 4: Packet = new API_FUNCTION_REQ(); break;
                    case 5: Packet = new API_SETTINGS_CHANGE_REQ(); break;
                    case 6: Packet = new API_RELOAD_CACHE_REQ(); break;
                    case 7: Packet = new API_GET_AUTH_REQ(); break;

                    case 20: Packet = new API_BATTLE_TEST_REQ(); break;

                    default:
                        {
                            Logger.Warning($" [ApiClient] Unknown REQ ID: {Opcode}.");
                            Close(5);
                            break;
                        }
                }
                if (Packet == null)
                {
                    return;
                }
                Logger.PacketREQ($" [ApiClient] [RunPacket] {Packet} SessionDate: {SessionDate} ReceiveDate: {DateTime.Now}");
                Packet.client = this;
                Packet.buffer = Buffer;
                try
                {
                    Packet.ReadImplement();
                    new Thread(new ThreadStart(() =>
                    {
                        try
                        {
                            Packet.RunImplement();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(" [ApiClient] RunImplement Catch [!] Exception: " + ex);
                            Close(50);
                        }
                    })).Start();
                }
                catch (Exception ex)
                {
                    Logger.Error(" [ApiClient] ReadImplement Catch [!] Exception: " + ex);
                    Close(50);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #region CLOSE/DISPOSE
        public override void Close(int time = 0)
        {
            if (ConnectionIsClosed)
            {
                return;
            }
            ConnectionIsClosed = true;
            try
            {
                ApiManager.RemoveSession(this);
                if (SessionSocket != null)
                {
                    SessionSocket.Close(time);
                    Thread.Sleep(time);
                }
                Dispose();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }
            Disposed = true;
            if (SessionSocket != null)
            {
                SessionSocket.Dispose();
                SessionSocket = null;
            }
            machine = null;
            if (disposing)
            {
                handle.Dispose();
                handle = null;
            }
        }
        #endregion

        #region IP MANAGER
        public override string GetIPAddress()
        {
            try
            {
                if (SessionSocket != null && SessionSocket.RemoteEndPoint != null)
                {
                    return ((IPEndPoint)SessionSocket.RemoteEndPoint).Address.ToString();
                }
            }
            catch
            {
                Close();
            }
            return "";
        }

        public IPAddress GetAddress()
        {
            try
            {
                if (SessionSocket != null && SessionSocket.RemoteEndPoint != null)
                {
                    return ((IPEndPoint)SessionSocket.RemoteEndPoint).Address;
                }
            }
            catch
            {
                Close();
            }
            return null;
        }

        public void GetAddressPort(out IPAddress address, out int port)
        {
            try
            {
                if (SessionSocket != null && SessionSocket.RemoteEndPoint != null)
                {
                    IPEndPoint end = (IPEndPoint)SessionSocket.RemoteEndPoint;
                    address = end.Address;
                    port = end.Port;
                    return;
                }
            }
            catch
            {
                Close();
            }
            address = IPAddress.Parse("127.0.0.1");
            port = 29890;
        }

        public bool IsSocketConnected()
        {
            try
            {
                return !((SessionSocket.Poll(1000, SelectMode.SelectRead) && (SessionSocket.Available == 0)) || !SessionSocket.Connected);
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}