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
    public class EventMapSyncer
    {
        private static readonly List<EventMapModel> list = new List<EventMapModel>();
        private static readonly string path = "Data/Events/EventsMap.xml";
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [EventMapSyncer] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [EventMapSyncer] Loaded {list.Count} events maps.");
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
                            if ("event".Equals(SecundaryNode.Name))
                            {
                                list.Add(new EventMapModel
                                {
                                    startDate = int.Parse(itemMap.GetNamedItem("start_date").Value),
                                    endDate = int.Parse(itemMap.GetNamedItem("end_date").Value),
                                    mapId = int.Parse(itemMap.GetNamedItem("map_id").Value),
                                    stageType = int.Parse(itemMap.GetNamedItem("stage_type").Value),
                                    percentXp = int.Parse(itemMap.GetNamedItem("percent_exp").Value),
                                    percentGp = int.Parse(itemMap.GetNamedItem("percent_gold").Value),
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

        public static void ReGenerateList()
        {
            list.Clear();
            Load();
        }

        public static EventMapModel GetRunningEvent()
        {
            try
            {
                int date = int.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                for (int i = 0; i < list.Count; i++)
                {
                    EventMapModel eventMap = list[i];
                    if (eventMap.startDate <= date && date < eventMap.endDate)
                    {
                        return eventMap;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return null;
        }

        public static bool EventIsValid(EventMapModel eventMap, int mapId, int stageType)
        {
            return eventMap != null && (eventMap.mapId == mapId || eventMap.stageType == stageType);
        }
    }

    public class EventMapModel
    {
        public int mapId, stageType;
        public int percentXp, percentGp;
        public int startDate, endDate;
    }
}