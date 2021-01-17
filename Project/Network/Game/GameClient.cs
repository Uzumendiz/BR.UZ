using Microsoft.Win32.SafeHandles;
using PointBlank.Api;
using PointBlank.Game;
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
    public class GameClient : Client, IDisposable
    {
        private Socket SessionSocket;
        public Account SessionPlayer;
        public DateTime SessionDate;
        public int SessionId;
        public ushort SessionSeed;
        private ushort NextSessionSeed;
        private int SessionShift;
        public int SessionPort;
        public bool ConnectionIsClosed = false;
        private bool Disposed = false;
        public DateTime LastServerListRefresh = new DateTime();
        public bool PacketGetRoomList;
        public bool PacketUserEnter;
        public bool PacketFirst;
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public GameClient(Socket SessionSocket)
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
                    if (SessionSocket != null && !PacketFirst)
                    {
                        Logger.Attacks($" [GameClient] Connection destroyed due to no responses (UserEnter). IPAddress ({GetIPAddress()})");
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

                    if (!PacketFirst && PacketId != 2579 && PacketId != 2644 && PacketId != 77)
                    {
                        Logger.Attacks($" [GameClient] Connection destroyed due to unknown first packet. PacketId ({PacketId}) IPAddress ({GetIPAddress()})");
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
                Logger.PacketACK($" [GameClient] [SendPacket] {packet}");
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
            Logger.Attacks($" [GameClient] [CheckSeed] Connection blocked. IP: {GetIPAddress()} Date ({DateTime.Now}) SessionId ({SessionId}) PacketSeed ({PacketSeed}) / NextSessionSeed ({NextSessionSeed}) PrimarySeed ({SessionSeed})");
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
                GamePacketReader Packet = null;
                switch (Opcode)
                {
                    case 275: Packet = new PROTOCOL_FRIEND_INVITE_FOR_ROOM_REQ(); break;
                    case 280: Packet = new PROTOCOL_FRIEND_ACCEPT_REQ(); break;
                    case 282: Packet = new PROTOCOL_FRIEND_INVITE_REQ(); break;
                    case 284: Packet = new PROTOCOL_FRIEND_DELETE_REQ(); break;
                    case 290: Packet = new PROTOCOL_BASE_SEND_WHISPER_REQ(); break;
                    case 292: Packet = new PROTOCOL_BASE_SEND_WHISPER2_REQ(); break;
                    case 297: Packet = new PROTOCOL_BASE_FIND_USER_REQ(); break;
                    case 417: Packet = new PROTOCOL_BOX_MESSAGE_CREATE_REQ(); break;
                    case 419: Packet = new PROTOCOL_BOX_MESSAGE_REPLY_REQ(); break;
                    case 422: Packet = new PROTOCOL_BOX_MESSAGE_VIEW_REQ(); break;
                    case 424: Packet = new PROTOCOL_BOX_MESSAGE_DELETE_REQ(); break;
                    case 530: Packet = new PROTOCOL_SHOP_BUY_ITEM_REQ(); break;
                    case 534: Packet = new PROTOCOL_INVENTORY_ITEM_EQUIP_REQ(); break;
                    case 536: Packet = new PROTOCOL_INVENTORY_ITEM_EFFECT_REQ(); break;
                    case 540: Packet = new PROTOCOL_BOX_MESSAGE_GIFT_TAKE_REQ(); break;
                    case 542: Packet = new PROTOCOL_INVENTORY_ITEM_EXCLUDE_REQ(); break;
                    case 544: Packet = new PROTOCOL_BASE_WEB_CASH_REQ(); break;
                    case 548: Packet = new PROTOCOL_INVENTORY_ITEM_CHECK_NICKNAME_REQ(); break;

                    case 1304: Packet = new PROTOCOL_CLAN_GET_INFO_REQ(); break;
                    case 1306: Packet = new PROTOCOL_CLAN_MEMBER_CONTEXT_REQ(); break;
                    case 1308: Packet = new PROTOCOL_CLAN_MEMBER_LIST_REQ(); break;
                    case 1310: Packet = new PROTOCOL_CLAN_CREATE_REQ(); break;
                    case 1312: Packet = new PROTOCOL_CLAN_DELETE_REQ(); break;
                    case 1314: Packet = new PROTOCOL_CLAN_CHECK_CREATE_INVITE_REQ(); break;
                    case 1316: Packet = new PROTOCOL_CLAN_CREATE_INVITE_REQ(); break;
                    case 1318: Packet = new PROTOCOL_CLAN_PLAYER_CLEAN_INVITES_REQ(); break;
                    case 1320: Packet = new PROTOCOL_CLAN_GET_CONTEXT_ENLISTMENTS_REQ(); break;
                    case 1322: Packet = new PROTOCOL_CLAN_GET_LIST_ENLISTMENTS_REQ(); break;
                    case 1324: Packet = new PROTOCOL_CLAN_REQUEST_INFO_REQ(); break;
                    case 1326: Packet = new PROTOCOL_CLAN_REQUEST_ACCEPT_REQ(); break;
                    case 1329: Packet = new PROTOCOL_CLAN_REQUEST_DENIAL_REQ(); break;
                    case 1332: Packet = new PROTOCOL_CLAN_MEMBER_LEAVE_REQ(); break;
                    case 1334: Packet = new PROTOCOL_CLAN_DEMOTE_KICK_REQ(); break;
                    case 1337: Packet = new PROTOCOL_CLAN_PROMOTE_MASTER_REQ(); break;
                    case 1340: Packet = new PROTOCOL_CLAN_PROMOTE_AUX_REQ(); break;
                    case 1343: Packet = new PROTOCOL_CLAN_DEMOTE_NORMAL_REQ(); break;
                    case 1358: Packet = new PROTOCOL_CLAN_CHATTING_REQ(); break;
                    case 1360: Packet = new PROTOCOL_CLAN_CHECK_LOGO_REQ(); break;
                    case 1362: Packet = new PROTOCOL_CLAN_CHANGE_NOTICE_REQ(); break;
                    case 1364: Packet = new PROTOCOL_CLAN_CHANGE_INFO_REQ(); break;
                    case 1372: Packet = new PROTOCOL_CLAN_SAVE_CONFIG_REQ(); break;
                    case 1381: Packet = new PROTOCOL_CLAN_ROOM_INVITED_REQ(); break;
                    case 1390: Packet = new PROTOCOL_CLAN_CHAT_1390_REQ(); break;
                    case 1392: Packet = new PROTOCOL_CLAN_MESSAGE_INVITE_REQ(); break;
                    case 1394: Packet = new PROTOCOL_CLAN_MESSAGE_REQUEST_INTERACT_REQ(); break;
                    case 1396: Packet = new PROTOCOL_CLAN_MSG_FOR_PLAYERS_REQ(); break;
                    case 1416: Packet = new PROTOCOL_CLAN_CREATE_REQUIREMENTS_REQ(); break;
                    case 1441: Packet = new PROTOCOL_CLAN_ENTER_REQ(); break;
                    case 1443: Packet = new PROTOCOL_CLAN_LEAVE_REQ(); break;
                    case 1445: Packet = new PROTOCOL_CLAN_LIST_REQ(); break;
                    case 1447: Packet = new PROTOCOL_CLAN_CHECK_NAME_REQ(); break;
                    case 1451: Packet = new PROTOCOL_CLAN_LIST_CONTEXT_REQ(); break;
                    case 1538: Packet = new PROTOCOL_CLAN_WAR_PARTY_CONTEXT_REQ(); break;
                    case 1540: Packet = new PROTOCOL_CLAN_WAR_PARTY_LIST_REQ(); break;
                    case 1542: Packet = new PROTOCOL_CLAN_WAR_MATCH_TEAM_CONTEXT_REQ(); break;
                    case 1544: Packet = new PROTOCOL_CLAN_WAR_MATCH_TEAM_LIST_REQ(); break;
                    case 1546: Packet = new PROTOCOL_CLAN_WAR_CREATE_TEAM_REQ(); break;
                    case 1548: Packet = new PROTOCOL_CLAN_WAR_JOIN_TEAM_REQ(); break;
                    case 1550: Packet = new PROTOCOL_CLAN_WAR_LEAVE_TEAM_REQ(); break;
                    case 1553: Packet = new PROTOCOL_CLAN_WAR_PROPOSE_REQ(); break;
                    case 1558: Packet = new PROTOCOL_CLAN_WAR_ACCEPT_BATTLE_REQ(); break;
                    case 1565: Packet = new PROTOCOL_CLAN_WAR_CREATE_ROOM_REQ(); break;
                    case 1567: Packet = new PROTOCOL_CLAN_WAR_JOIN_ROOM_REQ(); break;
                    case 1569: Packet = new PROTOCOL_CLAN_WAR_MATCH_TEAM_INFO_REQ(); break;
                    case 1571: Packet = new PROTOCOL_CLAN_WAR_UPTIME_REQ(); break;
                    case 1576: Packet = new PROTOCOL_CLAN_WAR_TEAM_CHATTING_REQ(); break;
                    case 2571: Packet = new PROTOCOL_BASE_CHANNEL_LIST_REQ(); break;
                    case 2573: Packet = new PROTOCOL_BASE_CHANNEL_ENTER_REQ(); break;
                    case 2575: break; //PROTOCOL_BASE_HEARTBEAT_REQ
                    case 2577: Packet = new PROTOCOL_BASE_SERVER_CHANGE_REQ(); break;
                    case 2579: Packet = new PROTOCOL_BASE_USER_ENTER_REQ(); break;
                    case 2581: Packet = new PROTOCOL_BASE_CONFIG_SAVE_REQ(); break;
                    case 2591: Packet = new PROTOCOL_BASE_GET_USER_STATS_REQ(); break;
                    case 2601: Packet = new PROTOCOL_BASE_MISSION_ENTER_REQ(); break;
                    case 2605: Packet = new PROTOCOL_BASE_QUEST_BUY_CARD_SET_REQ(); break;
                    case 2607: Packet = new PROTOCOL_BASE_QUEST_DELETE_CARD_SET_REQ(); break;
                    case 2619: Packet = new PROTOCOL_BASE_TITLE_GET_REQ(); break;
                    case 2621: Packet = new PROTOCOL_BASE_TITLE_USE_REQ(); break;
                    case 2623: Packet = new PROTOCOL_BASE_TITLE_DETACH_REQ(); break;
                    case 2627: Packet = new PROTOCOL_BASE_CHATTING_REQ(); break;
                    case 2635: break; //PROTOCOL_BASE_MISSION_SUCCESS_REQ
                    case 2639: Packet = new PROTOCOL_LOBBY_PLAYER_STATISTICS_REQ(); break;
                    case 2642: Packet = new PROTOCOL_BASE_SERVER_LIST_REFRESH_REQ(); break;
                    case 2644: Packet = new PROTOCOL_BASE_SERVER_PASSWORD_REQ(); break;
                    case 2654: Packet = new PROTOCOL_BASE_USER_EXIT_REQ(); break;
                    case 2661: Packet = new PROTOCOL_EVENT_VISIT_CONFIRM_REQ(); break;
                    case 2663: Packet = new PROTOCOL_EVENT_VISIT_REWARD_REQ(); break;
                    case 2684: Packet = new PROTOCOL_GM_LOG_LOBBY_REQ(); break;
                    case 2686: Packet = new PROTOCOL_GM_LOG_ROOM_REQ(); break;
                    case 2817: Packet = new PROTOCOL_SHOP_LEAVE_REQ(); break;
                    case 2819: Packet = new PROTOCOL_SHOP_ENTER_REQ(); break;
                    case 2821: Packet = new PROTOCOL_SHOP_LIST_REQ(); break;
                    case 3073: Packet = new PROTOCOL_LOBBY_GET_ROOMLIST_REQ(); break;
                    case 3077: Packet = new PROTOCOL_LOBBY_QUICKJOIN_ROOM_REQ(); break;
                    case 3079: Packet = new PROTOCOL_LOBBY_ENTER_REQ(); break;
                    case 3081: Packet = new PROTOCOL_LOBBY_JOIN_ROOM_REQ(); break;
                    case 3083: Packet = new PROTOCOL_LOBBY_LEAVE_REQ(); break;
                    case 3087: Packet = new PROTOCOL_LOBBY_GET_ROOMINFO_REQ(); break;
                    case 3089: Packet = new PROTOCOL_LOBBY_CREATE_ROOM_REQ(); break;
                    case 3094: Packet = new PROTOCOL_A_3094_REQ(); break;
                    case 3099: Packet = new PROTOCOL_LOBBY_PLAYER_EQUIPMENTS_REQ(); break;
                    case 3101: Packet = new PROTOCOL_LOBBY_CREATE_NICKNAME_REQ(); break;
                    case 3331: Packet = new PROTOCOL_BATTLE_READYBATTLE_REQ(); break;
                    case 3333: Packet = new PROTOCOL_BATTLE_STARTBATTLE_REQ(); break;
                    case 3337: Packet = new PROTOCOL_BATTLE_RESPAWN_REQ(); break;
                    case 3344: Packet = new PROTOCOL_BATTLE_SENDPING_REQ(); break;
                    case 3348: Packet = new PROTOCOL_BATTLE_PRESTARTBATTLE_REQ(); break;
                    case 3354: Packet = new PROTOCOL_BATTLE_DEATH_REQ(); break;
                    case 3356: Packet = new PROTOCOL_BATTLE_MISSION_BOMB_INSTALL_REQ(); break;
                    case 3358: Packet = new PROTOCOL_BATTLE_MISSION_BOMB_UNINSTALL_REQ(); break;
                    case 3368: Packet = new PROTOCOL_BATTLE_MISSION_GENERATOR_INFO_REQ(); break;
                    case 3372: Packet = new PROTOCOL_BATTLE_TIMERSYNC_REQ(); break;
                    case 3376: Packet = new PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_REQ(); break;
                    case 3378: Packet = new PROTOCOL_BATTLE_RESPAWN_FOR_AI_REQ(); break;
                    case 3384: Packet = new PROTOCOL_BATTLE_PLAYER_LEAVE_REQ(); break;
                    case 3386: Packet = new PROTOCOL_BATTLE_MISSION_DEFENCE_INFO_REQ(); break;
                    case 3390: Packet = new PROTOCOL_BATTLE_DINO_DEATHBLOW_REQ(); break;
                    case 3394: Packet = new PROTOCOL_BATTLE_ENDTUTORIAL_REQ(); break;
                    case 3396: Packet = new PROTOCOL_VOTEKICK_START_REQ(); break;
                    case 3400: Packet = new PROTOCOL_VOTEKICK_UPDATE_REQ(); break;
                    case 3421: Logger.Error(" [GameManager] PROTOCOL_BATTLE_PAUSE_REQ"); break;
                    case 3423: Logger.Error(" [GameManager] PROTOCOL_BATTLE_UNPAUSE_REQ"); break;
                    case 3428: Packet = new PROTOCOL_A_3428_REQ(); break;
                    case 3585: Packet = new PROTOCOL_INVENTORY_ENTER_REQ(); break;
                    case 3589: Packet = new PROTOCOL_INVENTORY_LEAVE_REQ(); break;
                    case 3841: Packet = new PROTOCOL_ROOM_GET_PLAYERINFO_REQ(); break;
                    case 3845: Packet = new PROTOCOL_ROOM_CHANGE_SLOT_REQ(); break;
                    case 3847: Packet = new PROTOCOL_BATTLE_ROOM_INFO_REQ(); break;
                    case 3849: Packet = new PROTOCOL_ROOM_CLOSE_SLOT_REQ(); break;
                    case 3854: Packet = new PROTOCOL_ROOM_GET_LOBBY_USER_LIST_REQ(); break;
                    case 3858: Packet = new PROTOCOL_ROOM_CHANGE_OPTIONS_REQ(); break;
                    case 3862: Packet = new PROTOCOL_BASE_PROFILE_ENTER_REQ(); break;
                    case 3864: Packet = new PROTOCOL_BASE_PROFILE_LEAVE_REQ(); break;
                    case 3866: Packet = new PROTOCOL_ROOM_REQUEST_HOST_REQ(); break;
                    case 3868: Packet = new PROTOCOL_ROOM_RANDOM_HOST2_REQ(); break;
                    case 3870: Packet = new PROTOCOL_ROOM_CHANGE_HOST_REQ(); break;
                    case 3872: Packet = new PROTOCOL_ROOM_RANDOM_HOST_REQ(); break;
                    case 3874: Packet = new PROTOCOL_ROOM_CHANGE_TEAM_REQ(); break;
                    case 3884: Packet = new PROTOCOL_ROOM_INVITE_PLAYERS_REQ(); break;
                    case 3886: Packet = new PROTOCOL_ROOM_CHANGE_INFO_REQ(); break;
                    case 3890: Packet = new PROTOCOL_GM_KICK_PLAYER_BYSLOT_REQ(); break;
                    case 3894: Packet = new PROTOCOL_A_3894_REQ(); break;
                    case 3900: Packet = new PROTOCOL_A_3900_REQ(); break;
                    case 3902: Packet = new PROTOCOL_A_3902_REQ(); break;
                    case 3904: Packet = new PROTOCOL_BATTLE_LOADING_REQ(); break;
                    case 3906: Packet = new PROTOCOL_ROOM_CHANGE_PASSWORD_REQ(); break;
                    case 3910: Packet = new PROTOCOL_EVENT_PLAYTIME_REWARD_REQ(); break;
                    case 77: Packet = new PROTOCOL_BASE_ADMIN_REQ(); break;
                    case 2694: break;
                    default:
                        {
                            //3329: Pode ser chamado caso state = 13| caso contrário é chamado o 3333
                            Logger.Warning($" [GameClient] Unknown REQ ID: {Opcode} Value: {value}.");
                            Close(5);
                            break;
                        }
                }
                if (Packet == null)
                {
                    return;
                }
                Logger.PacketREQ($" [GameClient] [RunPacket] {Packet} Value: {value} SessionDate: {SessionDate} ReceiveDate: {DateTime.Now}");
                Packet.client = this;
                Packet.buffer = Buffer;
                if (Settings.DebugMode)
                {
                    Logger.Debug($" [GameClient] PacketId: {Opcode} PacketLength: {Buffer.Length} PacketName: {Packet}\n{FormatHex(Buffer)}");
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
                            Logger.Error(" [GameClient] RunImplement Catch [!] Exception: " + ex);
                            Close(50);
                        }
                    })).Start();
                }
                catch (Exception ex)
                {
                    Logger.Error(" [GameClient] ReadImplement Catch [!] Exception: " + ex);
                    Close(50);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #region CLOSE/DISPOSE
        public override void Close(int time = 0, bool kicked = false)
        {
            if (ConnectionIsClosed)
            {
                return;
            }
            ConnectionIsClosed = true;
            try
            {
                FirewallSecurity.RemoveRuleUdp(GetIPAddress(), SessionPort);
                GameManager.RemoveSession(this);
                if (SessionPlayer != null)
                {
                    Channel channel = SessionPlayer.GetChannel();
                    Room room = SessionPlayer.room;
                    Match match = SessionPlayer.match;
                    SessionPlayer.SetOnlineStatus(false);
                    if (room != null)
                    {
                        room.RemovePlayer(SessionPlayer, false, kicked ? 1 : 0);
                    }
                    if (match != null)
                    {
                        match.RemovePlayer(SessionPlayer);
                    }
                    if (channel != null)
                    {
                        channel.RemovePlayer(SessionPlayer);
                    }

                    SessionPlayer.status.ResetData(SessionPlayer.playerId);

                    SessionPlayer.SyncPlayerToFriends(false);
                    SessionPlayer.SyncPlayerToClanMembers();

                    ApiManager.SendPacketToAllClients(new API_USER_DISCONNECT_ACK(SessionPlayer, 0));

                    SessionPlayer.GameClear();
                    SessionPlayer.UpdateCacheInfo();
                    SessionPlayer = null;
                }
                if (SessionSocket != null)
                {
                    SessionSocket.Close(time);
                    Thread.Sleep(time);
                }
                FirewallSecurity.RemoveRuleTcp(GetIPAddress());
                Dispose();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            ServersManager.UpdateServerPlayers(); //Auth
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