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
    public class EventQuestSyncer
    {
        private static readonly List<QuestModel> list = new List<QuestModel>();
        private static readonly string path = "Data/Events/EventsQuest.xml";
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [EventQuestSyncer] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [EventQuestSyncer] Loaded {list.Count} events quests.");
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
                                list.Add(new QuestModel
                                {
                                    startDate = uint.Parse(itemMap.GetNamedItem("start_date").Value),
                                    endDate = uint.Parse(itemMap.GetNamedItem("end_date").Value)
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

        public static QuestModel GetRunningEvent()
        {
            try
            {
                int date = int.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                for (int i = 0; i < list.Count; i++)
                {
                    QuestModel quest = list[i];
                    if (quest.startDate <= date && date < quest.endDate)
                    {
                        return quest;
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

    public class QuestModel
    {
        public uint startDate, endDate;
    }
}