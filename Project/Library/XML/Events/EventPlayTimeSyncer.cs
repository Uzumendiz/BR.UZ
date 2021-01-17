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
    public class EventPlayTimeSyncer
    {
        private static readonly List<PlayTimeModel> list = new List<PlayTimeModel>();
        private static readonly string path = "Data/Events/EventsPlayTime.xml";
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [EventPlayTimeSyncer] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [EventPlayTimeSyncer] Loaded {list.Count} events playtime.");
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
                                list.Add(new PlayTimeModel
                                {
                                    startDate = int.Parse(itemMap.GetNamedItem("start_date").Value),
                                    endDate = int.Parse(itemMap.GetNamedItem("end_date").Value),
                                    title = itemMap.GetNamedItem("title").Value,
                                    time = long.Parse(itemMap.GetNamedItem("time").Value),
                                    goodReward1 = int.Parse(itemMap.GetNamedItem("good_reward1").Value),
                                    goodReward2 = int.Parse(itemMap.GetNamedItem("good_reward2").Value),
                                    goodCount1 = int.Parse(itemMap.GetNamedItem("good_count1").Value),
                                    goodCount2 = int.Parse(itemMap.GetNamedItem("good_count2").Value)
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

        public static PlayTimeModel GetRunningEvent()
        {
            try
            {
                int date = int.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                for (int i = 0; i < list.Count; i++)
                {
                    PlayTimeModel playTime = list[i];
                    if (playTime.startDate <= date && date < playTime.endDate)
                    {
                        return playTime;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return null;
        }
    }

    public class PlayTimeModel
    {
        public int goodReward1, goodReward2, goodCount1, goodCount2;
        public int startDate, endDate;
        public string title = "";
        public long time;
        public bool EventIsEnabled()
        {
            int date = int.Parse(DateTime.Now.ToString("yyMMddHHmm"));
            if (startDate <= date && date < endDate)
            {
                return true;
            }
            return false;
        }

        public int GetRewardCount(int goodId)
        {
            return goodId == goodReward1 ? goodCount1 : (goodId == goodReward2 ? goodCount2 : 0);
        }
    }
}