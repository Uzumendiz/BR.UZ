using Microsoft.Win32.SafeHandles;
using PointBlank.Api;
using PointBlank.Auth;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
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
    public class AuthClient : Client, IDisposable
    {
        private Socket SessionSocket;
        public Account SessionPlayer;
        public DateTime SessionDate;
        public int SessionId;
        public ushort SessionSeed;
        private ushort NextSessionSeed;
        private int SessionShift;
        public bool ConnectionIsClosed = false;
        private bool Disposed = false;
        public DateTime LastServerListRefresh = new DateTime();
        public bool PacketGetRoomList;
        public bool PacketUserEnter;
        public bool PacketLogin;
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public AuthClient(Socket SessionSocket)
        {
            this.SessionSocket = SessionSocket;
        }

        public void StartSession()
        {
            try
            {
                SessionSeed = (ushort)new Random(SessionDate.Millisecond).Next(SessionId, short.MaxValue);
                NextSessionSeed = SessionSeed;
                SessionShift = SessionId % 7 + 1;
                new Thread(new ThreadStart(Initialize)).Start();
                new Thread(new ThreadStart(OnReadCallback)).Start();
                new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(10000);
                    if (SessionSocket != null && !PacketLogin)
                    {
                        Logger.Attacks($" [AuthClient] Connection destroyed due to no responses (Login). IPAddress ({GetIPAddress()})");
                        Close();
                    }
                })).Start();
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
                SendCompletePacket(PackageDataManager.BASE_COPYRIGTH_ACK);
                GetAddressPort(out IPAddress Address, out int Port);
                SendPacket(new PROTOCOL_BASE_SERVER_LIST_ACK(SessionId, SessionSeed, Address, Port));
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

                    int length = BitConverter.ToUInt16(BufferTotalEncrypted, 0) & 0x7FFF;

                    byte[] BufferPacket = new byte[length + 2];
                    Array.Copy(BufferTotalEncrypted, 2, BufferPacket, 0, BufferPacket.Length);

                    byte[] PacketDecrypted = Decrypt(BufferPacket);

                    ushort PacketId = BitConverter.ToUInt16(PacketDecrypted, 0);
                    ushort PacketSeed = BitConverter.ToUInt16(PacketDecrypted, 2);

                    if (!PacketLogin && PacketId != 77 && (Settings.LoginType == 1 && PacketId != 2561 || Settings.LoginType == 2 && PacketId != 2672))
                    {
                        Logger.Attacks($" [AuthClient] Connection destroyed due to unknown first packet. PacketId ({PacketId}) IPAddress ({GetIPAddress()})");
                        Close();
                        return;
                    }
                    if (!CheckSeed(PacketSeed, true))
                    {
                        Close();
                        return;
                    }
                    if (ConnectionIsClosed)
                    {
                        return;
                    }
                    RunPacket(PacketId, PacketDecrypted, "Primary");
                    CheckoutBuffer(BufferTotalEncrypted, length);
                    new Thread(new ThreadStart(OnReadCallback)).Start();
                }
            }
            catch
            {
                Close();
            }
        }

        /// <summary>
        /// Confere o restante dos dados recebidos, caso tiver dados a mais, lê o próximo pacote.
        /// </summary>
        public void CheckoutBuffer(byte[] BufferTotal, int FirstLength)
        {
            try
            {
                byte[] BufferTotalEncrypted = new byte[BufferTotal.Length - FirstLength - 4];
                Array.Copy(BufferTotal, FirstLength + 4, BufferTotalEncrypted, 0, BufferTotalEncrypted.Length);
                if (BufferTotalEncrypted.Length == 0)
                {
                    return;
                }
                int length = BitConverter.ToUInt16(BufferTotalEncrypted, 0) & 0x7FFF;

                byte[] BufferPacket = new byte[length + 2];
                Array.Copy(BufferTotalEncrypted, 2, BufferPacket, 0, BufferPacket.Length);

                byte[] PacketDecrypted = Decrypt(BufferPacket);

                ushort PacketId = BitConverter.ToUInt16(PacketDecrypted, 0);
                ushort PacketSeed = BitConverter.ToUInt16(PacketDecrypted, 2);

                if (!CheckSeed(PacketSeed, false))
                {
                    Close();
                    return;
                }
                RunPacket(PacketId, PacketDecrypted, "Secondary");
                CheckoutBuffer(BufferTotalEncrypted, length);
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
                if (data.Length < 4)
                {
                    return;
                }
                SessionSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), SessionSocket);
            }
            catch
            {
                Close();
            }
        }

        public override void SendPacket(GamePacketWriter packet)
        {
            try
            {
                Logger.PacketACK($" [AuthClient] [SendPacket] {packet}");
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

        public bool CheckSeed(ushort PacketSeed, bool isTheFirstPacket)
        {
            if (PacketSeed == GetNextSessionSeed())
            {
                return true;
            }
            Logger.Attacks($" [AuthClient] [CheckSeed] Connection blocked. IP: {GetIPAddress()} Date ({DateTime.Now}) SessionId ({SessionId}) PacketSeed ({PacketSeed}) / NextSessionSeed ({NextSessionSeed}) PrimarySeed ({SessionSeed})");
            if (isTheFirstPacket)
            {
                new Thread(new ThreadStart(OnReadCallback)).Start();
            }
            return false;
        }

        private ushort GetNextSessionSeed()
        {
            try
            {
                unchecked
                {
                    NextSessionSeed = (ushort)((((NextSessionSeed * 214013) + 2531011) >> 16) & short.MaxValue);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return NextSessionSeed;
        }

        private void RunPacket(ushort Opcode, byte[] Buffer, string value)
        {
            try
            {
                AuthPacketReader Packet = null;
                switch (Opcode)
                {
                    case 2672: Packet = new PROTOCOL_BASE_LOGIN_TH_REQ(); break;
                    case 2561: Packet = new PROTOCOL_BASE_LOGIN_REQ(); break;
                    case 528: Packet = new PROTOCOL_BASE_USER_GIFTLIST_REQ(); break;
                    case 544: Packet = new PROTOCOL_BASE_WEB_CASH_REQ(); break;
                    case 2565: Packet = new PROTOCOL_BASE_USER_INFO_REQ(); break;
                    //case 2666: Packet = new PROTOCOL_BASE_USER_INFO_REQ(); break;
                    case 2567: Packet = new PROTOCOL_BASE_USER_FRIENDS_REQ(); break;
                    case 2577: Packet = new PROTOCOL_BASE_SERVER_CHANGE_REQ(); break;
                    case 2579: Packet = new PROTOCOL_BASE_USER_ENTER_REQ(); break;
                    case 2581: Packet = new PROTOCOL_BASE_CONFIG_SAVE_REQ(); break;
                    case 2642: Packet = new PROTOCOL_BASE_SERVER_LIST_REFRESH_REQ(); break;
                    case 2654: Packet = new PROTOCOL_BASE_USER_EXIT_REQ(); break;
                    case 2678: Packet = new PROTOCOL_BASE_SOURCE_REQ(); break;
                    case 2698: Packet = new PROTOCOL_BASE_USER_INVENTORY_REQ(); break;
                    case 2575: break;
                    default:
                        {
                            Logger.Warning($" [AuthClient] Unknown REQ ID: {Opcode} Value: {value}.");
                            Close(5);
                            break;
                        }
                }
                if (Packet == null)
                {
                    return;
                }
                Logger.PacketREQ($" [AuthClient] [RunPacket] {Packet} Value: {value} SessionDate: {SessionDate} ReceiveDate: {DateTime.Now}");
                Packet.client = this;
                Packet.buffer = Buffer;
                if (Settings.DebugMode)
                {
                    Logger.Debug($" [AuthClient] PacketId: {Opcode} PacketLength: {Buffer.Length} PacketName: {Packet}\n{FormatHex(Buffer)}");
                }
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
                            Logger.Error(" [AuthClient] RunImplement Catch [!] Exception: " + ex);
                            Close(50);
                        }
                    })).Start();
                }
                catch (Exception ex)
                {
                    Logger.Error(" [AuthClient] ReadImplement Catch [!] Exception: " + ex);
                    Close(50);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public void PROTOCOL_BASE_HEARTBEAT_ACK()
        {
            TimerState tick = new TimerState();
            tick.StartTimer(TimeSpan.FromMinutes(20), (callbackState) =>
            {
                if (!ConnectionIsClosed)
                {
                    Logger.Attacks($" [AuthClient] Connection destroyed due to no response for 20 minutes (HeartBeat). IPAddress ({GetIPAddress()})");
                    Close();
                }
                lock (callbackState)
                {
                    tick.Timer = null;
                }
            });
        }

        #region CLOSE/DISPOSE
        public override void Close(int time = 0, bool destroy = true)
        {
            if (ConnectionIsClosed)
            {
                return;
            }
            ConnectionIsClosed = true;
            try
            {
                AuthManager.RemoveSession(this);
                if (destroy)
                {
                    if (SessionPlayer != null)
                    {
                        SessionPlayer.SetOnlineStatus(false);
                        SessionPlayer.status.ResetData(SessionPlayer.playerId);

                        SessionPlayer.SyncPlayerToFriends(false);
                        SessionPlayer.SyncPlayerToClanMembers();

                        ApiManager.SendPacketToAllClients(new API_SERVER_INFO_ACK());

                        SessionPlayer.GameClear();
                        SessionPlayer.UpdateCacheInfo();
                        SessionPlayer = null;
                    }
                    if (SessionSocket != null)
                    {
                        SessionSocket.Close(time);
                        Thread.Sleep(time);
                        FirewallSecurity.RemoveRuleTcp(GetIPAddress());
                    }
                    Dispose();
                }
                else if (SessionPlayer != null)
                {
                    SessionPlayer.client = null;
                    SessionPlayer.UpdateCacheInfo();
                    SessionPlayer = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            ServersManager.UpdateServerPlayers();
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
            SessionPlayer = null;
            if (SessionSocket != null)
            {
                SessionSocket.Dispose();
                SessionSocket = null;
            }
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

        public byte[] Decrypt(byte[] data)
        {
            try
            {
                int length = data.Length;
                byte last = data[length - 1];
                for (int i = length - 1; i > 0; i--)
                {
                    data[i] = (byte)(data[i - 1] << (8 - SessionShift) | data[i] >> SessionShift);
                }
                data[0] = (byte)(last << (8 - SessionShift) | data[0] >> SessionShift);
                return data;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return new byte[0];
        }

        public string FormatHex(byte[] buffer)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}|---------------------------------------------|  |--------------|{0}", Environment.NewLine);
            int index = 0, i;
            string hex, data;
            while (index < buffer.Length)
            {
                hex = data = string.Empty;
                for (i = 0; i < 16 && index + i < buffer.Length; i++)
                {
                    hex += buffer[index + i].ToString("x2") + " ";
                    if (buffer[i + index] > 31 && buffer[i + index] < 127)
                    {
                        data += (char)buffer[i + index];
                    }
                    else
                    {
                        data += ".";
                    }
                }
                while (i < 16)
                {
                    hex += "   ";
                    i++;
                }
                builder.AppendFormat("{0} {1}{2}", hex.ToUpper(), data, Environment.NewLine);
                index += 16;
            }
            builder.Append("|---------------------------------------------|  |--------------|");
            return builder.ToString();
        }
    }
}