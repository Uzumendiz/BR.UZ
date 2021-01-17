using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

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
    public class ServersManager
    {
        public static readonly List<GameServerModel> servers = new List<GameServerModel>();
        public static readonly List<Channel> channels = new List<Channel>();
        private static readonly string path = "Data/Servers.xml";
        public static DateTime LastSyncPlayers;
        public static byte[] AuthServerListBytes;
        public static byte[] GameServerListBytes;
        public static byte[] ServerRefreshBytes;
        public static GameServerModel GetServer()
        {
            lock (servers)
            {
                try
                {
                    return servers[Settings.ServerId - 1]; //-1 por causa que o xml tem apenas o servidor id: 1
                }
                catch
                {
                    return null;
                }
            }
        }
        public static Channel GetChannel(int id)
        {
            lock (channels)
            {
                try
                {
                    //for (int i = 0; i < channels.Count; i++)
                    //{
                    //    Channel channel = channels[i];
                    //    if (channel.id == id)
                    //    {
                    //        return channel;
                    //    }
                    //}
                    return channels.Where(x => x.id == id).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
                return null;
            }
        }
        public static int GetPlayers()
        {
            lock (servers)
            {
                try
                {
                    return servers[Settings.ServerId - 1].lastCount;
                }
                catch
                {
                    return 0;
                }
            }
        }
        public static void UpdateServerPlayers()
        {
            try
            {
                DateTime now = DateTime.Now;
                if ((now - LastSyncPlayers).TotalSeconds >= Settings.UpdateIntervalPlayersServer)
                {
                    int players = 0;
                    for (int i = 0; i < channels.Count; i++)
                    {
                        players += channels[i].players.Count; //Soma os players dos canais
                    }
                    //atualiza os players do gameserver
                    GetServer().lastCount = players;
                    LastSyncPlayers = now;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [Servers] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [Servers] Loaded {servers.Count} servers.");
            Logger.Informations($" [Channels] Loaded {channels.Count} channels.");
            LoadServerListBytes();
        }

        private static void GenerateList()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                for (XmlNode PrimaryNode = document.FirstChild; PrimaryNode != null; PrimaryNode = PrimaryNode.NextSibling)
                {
                    if ("list".Equals(PrimaryNode.Name))
                    {
                        XmlNamedNodeMap PrimaryMap = PrimaryNode.Attributes;
                        int ServerId = int.Parse(PrimaryMap.GetNamedItem("Server").Value);

                        servers.Add(new GameServerModel(PrimaryMap.GetNamedItem("Address").Value)
                        {
                            id = ServerId,
                            state = int.Parse(PrimaryMap.GetNamedItem("State").Value),
                            type = byte.Parse(PrimaryMap.GetNamedItem("Type").Value),
                            port = ushort.Parse(PrimaryMap.GetNamedItem("Port").Value),
                            maxPlayers = ushort.Parse(PrimaryMap.GetNamedItem("MaximumPlayers").Value)
                        });

                        for (XmlNode SecondaryNode = PrimaryNode.FirstChild; SecondaryNode != null; SecondaryNode = SecondaryNode.NextSibling)
                        {
                            if ("Channel".Equals(SecondaryNode.Name))
                            {
                                XmlNamedNodeMap SecondaryMap = SecondaryNode.Attributes;
                                channels.Add(new Channel()
                                {
                                    serverId = ServerId,
                                    id = int.Parse(SecondaryMap.GetNamedItem("Id").Value),
                                    type = int.Parse(SecondaryMap.GetNamedItem("Type").Value),
                                    announce = SecondaryMap.GetNamedItem("Announce").Value
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private static void LoadServerListBytes()
        {
            using (PacketWriter sendRefresh = new PacketWriter())
            {
                sendRefresh.WriteD(2); //Servers Count
                //Server: 0
                sendRefresh.WriteD(0);
                sendRefresh.WriteIP("127.0.0.1");
                sendRefresh.WriteH(8080);
                sendRefresh.WriteC(0);
                sendRefresh.WriteH(0);
                sendRefresh.WriteD(0);
                //Server: 1
                GameServerModel server = GetServer();
                sendRefresh.WriteD(server.state);
                sendRefresh.WriteIP(server.ip);
                sendRefresh.WriteH(server.port);
                sendRefresh.WriteC(server.type);
                sendRefresh.WriteH(server.maxPlayers);
                server = null;

                ServerRefreshBytes = sendRefresh.memorystream.ToArray();
            }
            using (PacketWriter sendGame = new PacketWriter())
            {
                for (int i = 0; i < channels.Count; i++)
                {
                    sendGame.WriteC((byte)channels[i].type);
                }
                sendGame.WriteC(1);
                sendGame.WriteB(ServerRefreshBytes);

                GameServerListBytes = sendGame.memorystream.ToArray();
            }
            using (PacketWriter sendAuth = new PacketWriter())
            {
                sendAuth.WriteB(new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
                sendAuth.WriteB(ServerRefreshBytes);

                AuthServerListBytes = sendAuth.memorystream.ToArray();
            }
        }
    }
}