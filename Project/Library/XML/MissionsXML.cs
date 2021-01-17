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
    public class MissionsXML
    {
        private static readonly List<MissionModel> missions = new List<MissionModel>();
        private static readonly string path = "Data/Missions/Missions.xml";
        public static uint missionPage1;
        public static uint missionPage2;
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [MissionsXML] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [MissionsXML] Loaded {missions.Count} missions.");
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
                        for (XmlNode SecundaryNode = PrimaryNode.FirstChild; SecundaryNode != null; SecundaryNode = SecundaryNode.NextSibling)
                        {
                            XmlNamedNodeMap itemMap = SecundaryNode.Attributes;
                            if ("mission".Equals(SecundaryNode.Name))
                            {
                                MissionModel mission = new MissionModel
                                {
                                    id = int.Parse(itemMap.GetNamedItem("id").Value),
                                    price = int.Parse(itemMap.GetNamedItem("price").Value)
                                };
                                bool enable = bool.Parse(itemMap.GetNamedItem("enable").Value);
                                uint flag = (uint)(1 << mission.id);
                                int listId = (int)Math.Ceiling(mission.id / 32.0);
                                if (enable)
                                {
                                    if (listId == 1)
                                    {
                                        missionPage1 += flag;
                                    }
                                    else if (listId == 2)
                                    {
                                        missionPage2 += flag;
                                    }
                                }
                                missions.Add(mission);
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

        public static int GetMissionPrice(int id)
        {
            for (int i = 0; i < missions.Count; i++)
            {
                MissionModel mission = missions[i];
                if (mission.id == id)
                {
                    return mission.price;
                }
            }
            return -1;
        }
    }
    public class MissionModel
    {
        public int id;
        public int price;
    }
}