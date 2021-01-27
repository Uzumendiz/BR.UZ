using System;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_LOGIN_TH_REQ : AuthPacketReader
    {
        private string Token;
        //private string UserFileListHash;
        //private string D3DX9Hash;
        private string ClientVersion;
        private string PublicIP;
        private byte[] LocalIP;
        private int Rede;
        //private ulong LauncherKey;
        //private PhysicalAddress MacAddress;
        private ClientLocaleEnum ClientLocale;
        public override void ReadImplement()
        {
            ClientLocale = ClientLocaleEnum.Indonesia;
            ClientVersion = $"{ReadByte()}.{ReadShort()}.{ReadShort()}"; //OK
            int tokenSize = ReadShort(); //OK
            Token = ReadString(tokenSize); //OK
            ReadB(8); //Suposto: MAC Recebe tudo 0000000000 fazer proteção
            Rede = ReadByte(); //OK
            LocalIP = ReadB(4); //OK
            ReadB(16); //Recebe tudo 0000000000 fazer proteção
            string Hash = ReadString(32); //DirectX ?
            byte[] buffer32length = ReadB(33);
            Logger.Warning($" PACKET LOGIN [!] Token: {Token} buffer32length: {BitConverter.ToString(buffer32length)} Hash: {Hash}");
        }

        public override void RunImplement()
        {
            try
            {
                client.PacketLogin = true;
                PublicIP = client.GetIPAddress();
                string ErrorInformation = "";
                if (Settings.LoginType != 2)
                {
                    ErrorInformation = $" [Login] Type inválido. IP ({PublicIP})";
                }
                else if (Token.Length != 32)
                {
                    ErrorInformation = $" [Login] Token inválida. Token ({Token})";
                }
                else if (LocalIP == new byte[4] || LocalIP[0] == 0 || LocalIP[3] == 0)
                {
                    ErrorInformation = $" [Login] Endereço de Ip local inválido. ({LocalIP}) Token ({Token})";
                }
                //else if (MacAddress.GetAddressBytes() == new byte[6])
                //{
                //    LogInfo = $" [Login] Endereço de Mac inválido. ({MacAddress.ToString()}) Token ({Token})";
                //}
                if (Settings.LoginRequirements)
                {
                    if (!ClientVersion.Equals(Settings.ClientVersion))
                    {
                        ErrorInformation = $" [Login] Versão da cliente inválida. ({ClientVersion}) Token ({Token})";
                    }
                    else if (Settings.ClientLocale != ClientLocaleEnum.None && !ClientLocale.Equals(Settings.ClientLocale))
                    {
                        ErrorInformation = $" [Login] Localização da cliente inválida. ({ClientLocale}) Token ({Token})";
                    }
                    //else if (!string.IsNullOrEmpty(Settings.UserFileList) && !UserFileListHash.Equals(Settings.UserFileList))
                    //{
                    //    ErrorInformation = $" [Login] Arquivo UserFileList.dat está desatualizado ou inválido. ({UserFileListHash}) Token ({Token})";
                    //}
                    //else if (Settings.LauncherKey != 0 && !LauncherKey.Equals(Settings.LauncherKey))
                    //{
                    //    ErrorInformation = $" [Login] Key do launcher inválida. ({LauncherKey}) Token ({Token})";
                    //}
                }

                if (ErrorInformation != "")
                {
                    Logger.Login(ErrorInformation);
                    client.Close(5000);
                    return;
                }

                Account player = client.SessionPlayer = AccountManager.GetAccountToken(Token);
                if (player != null)
                {
                    if (player.access == AccessLevelEnum.Disabled) //Account Disabled
                    {
                        Logger.Login(" [Login] Conta desativada. Login: " + Token);
                        client.SendCompletePacket(PackageDataManager.BASE_LOGIN_0x80000107_PAK);
                        client.Close(1000);
                        return;
                    }
                    else if (Settings.OnlyGM && player.access < AccessLevelEnum.Moderator)
                    {
                        Logger.Login($" [Login] Acesso permitido apenas para GameMaster. Login: {Token}");
                        GameManager.RemoveConnection(PublicIP);
                        client.Close(3000);
                        return;
                    }
                    bool RequestUpdateIP = player.lastIp != PublicIP;
                    if (RequestUpdateIP)
                    {
                        player.ExecuteQuery($"UPDATE accounts SET last_ip='{PublicIP}' WHERE id='{player.playerId}'");
                    }

                    Account playerCache = AccountManager.GetAccount(player.playerId, true);
                    if (!player.isOnline)
                    {
                        player.SetOnlineStatus(true);
                        player.LoadPlayerTitles();
                        player.LoadPlayerBonus();
                        player.LoadPlayerFriends(true);
                        player.LoadPlayerEvents();
                        //player.LoadPlayerConfigs();
                        player.status.SetData(4294967295, player.playerId);
                        player.status.UpdateServer(0); //0 = Auth(Ainda não selecionou o servidor) Game = Settings.ServerId
                        player.clanPlayers = ClanManager.GetClanPlayers(player.clanId, player.playerId);
                        client.SendPacket(new PROTOCOL_BASE_LOGIN_ACK(0, Token, player.playerId));
                        client.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
                        if (player.clanId > 0)
                        {
                            client.SendPacket(new PROTOCOL_BASE_USER_CLAN_MEMBERS_ACK(player.clanPlayers));
                        }
                        if (playerCache != null)
                        {
                            playerCache.client = client;
                        }
                        player.UpdateCommunity(true);
                        ServersManager.UpdateServerPlayers();
                        client.PROTOCOL_BASE_HEARTBEAT_ACK();
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.BASE_LOGIN_0x80000101_PAK);
                        if (playerCache != null && playerCache.client != null) //Close Auth
                        {
                            playerCache.SendCompletePacket(PackageDataManager.AUTH_ACCOUNT_KICK_TYPE1_PAK);
                            playerCache.SendCompletePacket(PackageDataManager.SERVER_MESSAGE_ERROR_2147487744_PAK);
                            playerCache.Close(1000);
                        }
                        else //Close Game
                        {
                            player.SendCompletePacket(PackageDataManager.GAME_ACCOUNT_KICK_TYPE1_PAK);
                            player.SendCompletePacket(PackageDataManager.SERVER_MESSAGE_ERROR_0x80001000_PAK);
                            player.Close(1000);
                        }
                        client.Close(1000);
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BASE_LOGIN_0x80000102_PAK);
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