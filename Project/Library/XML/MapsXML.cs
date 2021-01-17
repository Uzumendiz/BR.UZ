using System;
using System.Collections.Generic;
using System.IO;
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
    public class MapsXML
    {
        public static SortedList<int, string> maps = new SortedList<int, string>();
        public static List<byte> TagList = new List<byte>();
        public static List<ushort> ModeList = new List<ushort>();
        private static readonly string path = "Data/Maps/Modes.xml";
        public static uint maps1, maps2, maps3, maps4;
        public static byte[] ModeBytes;
        public static byte[] TagBytes;
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [MapsXML] {path} no exists.");
                return;
            }
            GenerateList();
            using (PacketWriter Writer = new PacketWriter())
            {
                for (int i = 0; i < ModeList.Count; i++)
                {
                    Writer.WriteH(ModeList[i]);
                }
                ModeBytes = Writer.memorystream.ToArray();
            }
            TagBytes = TagList.ToArray();
        }

        public static void ReGenerateList()
        {
            TagList.Clear();
            ModeList.Clear();
            Load();
        }
 
        private static void GenerateList()
        {
            try
            {
                int idx = 0;
                int list2 = 1;
                XmlDocument document = new XmlDocument();
                document.Load(path);
                for (XmlNode xmlNode1 = document.FirstChild; xmlNode1 != null; xmlNode1 = xmlNode1.NextSibling)
                {
                    if ("list".Equals(xmlNode1.Name))
                    {
                        for (XmlNode xmlNode2 = xmlNode1.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
                        {
                            if ("map".Equals(xmlNode2.Name))
                            {
                                XmlNamedNodeMap xml = xmlNode2.Attributes;
                                uint flag = (uint)(1 << idx++);
                                //Logger.Warning(" [MAP] Flag: " + flag + " Idx: " + idx + " Id: " + int.Parse(xml.GetNamedItem("Id").Value) + " Name: " + xml.GetNamedItem("Name").Value);
                                int list = list2;
                                if (idx == 32)
                                {
                                    list2++;
                                    idx = 0;
                                }
                                TagList.Add(byte.Parse(xml.GetNamedItem("Tag").Value));
                                ModeList.Add(ushort.Parse(xml.GetNamedItem("Mode").Value));
                                bool enable = bool.Parse(xml.GetNamedItem("Enable").Value);
                                if (!enable)
                                {
                                    continue;
                                }
                                if (list == 1)
                                {
                                    maps1 += flag;
                                }
                                else if (list == 2)
                                {
                                    maps2 += flag;
                                }
                                else if (list == 3)
                                {
                                    maps3 += flag;
                                }
                                else if (list == 4)
                                {
                                    maps4 += flag;
                                }
                                else
                                {
                                    Logger.Warning($" [MapsXML] [Lista Indefinida] Flag: {flag} List: {list}");
                                }
                                maps.Add(int.Parse(xml.GetNamedItem("Id").Value), xml.GetNamedItem("Name").Value);
                            }
                        }
                    }
                }
                //Logger.Warning("maps1: " + maps1);
                //Logger.Warning("maps2: " + maps2);
                //Logger.Warning("maps3: " + maps3);
                //Logger.Warning("maps4: " + maps4);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public static bool CheckInfo(int mapId, string mapName)
        {
            if (maps.ContainsKey(mapId) && maps.ContainsValue(mapName))
            {
                if (mapName == maps[mapId])
                {
                    return true;
                }
                else
                {
                    Logger.Warning($" [MapsIdXML] Name invalid [!] Name: {mapName} NameList: {maps[mapId]}");
                }
            }
            else
            {
                Logger.Warning($" [MapsIdXML] MapInfo no exists [!] Id: {mapId} Name: {mapName}");
            }
            return false;
        }
        public static bool CheckId(int mapId)
        {
            if (maps.ContainsKey(mapId))
            {
                return true;
            }
            else
            {
                Logger.Warning($" [MapsIdXML] Id no exists [!] Id: {mapId}");
                return false;
            }
        }
    }
}