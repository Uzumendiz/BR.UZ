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
    public class EventVisitSyncer
    {
        private static readonly List<EventVisitModel> list = new List<EventVisitModel>();
        private static readonly string path = "Data/Events/EventsVisit.xml";
        public static byte[] MyinfoBytes;
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [EventVisitSyncer] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [EventVisitSyncer] Loaded {list.Count} events checks.");
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
                                EventVisitModel eventVisit = new EventVisitModel
                                {
                                    id = int.Parse(itemMap.GetNamedItem("id").Value),
                                    startDate = int.Parse(itemMap.GetNamedItem("start_date").Value),
                                    endDate = int.Parse(itemMap.GetNamedItem("end_date").Value),
                                    title = itemMap.GetNamedItem("title").Value,
                                    checks = byte.Parse(itemMap.GetNamedItem("checks").Value)
                                };
                                string goods1 = itemMap.GetNamedItem("goods1").Value;
                                string counts1 = itemMap.GetNamedItem("counts1").Value;
                                string goods2 = itemMap.GetNamedItem("goods2").Value;
                                string counts2 = itemMap.GetNamedItem("counts2").Value;

                                string[] goodsarray1 = goods1.Split(',');
                                string[] goodsarray2 = goods2.Split(',');

                                for (int i = 0; i < goodsarray1.Length; i++)
                                {
                                    eventVisit.box[i].reward1.goodId = int.Parse(goodsarray1[i]);
                                }
                                for (int i = 0; i < goodsarray2.Length; i++)
                                {
                                    eventVisit.box[i].reward2.goodId = int.Parse(goodsarray2[i]);
                                }

                                string[] countarray1 = counts1.Split(',');
                                string[] countarray2 = counts2.Split(',');
                                for (int i = 0; i < countarray1.Length; i++)
                                {
                                    VisitItem item = eventVisit.box[i].reward1;
                                    item.SetCount(countarray1[i]);
                                }
                                for (int i = 0; i < countarray2.Length; i++)
                                {
                                    VisitItem item = eventVisit.box[i].reward2;
                                    item.SetCount(countarray2[i]);
                                }
                                eventVisit.SetBoxCounts();
                                list.Add(eventVisit);
                            }
                        }
                    }
                }
                EventVisitModel visit = GetRunningEvent();
                if (visit != null)
                {
                    using (PacketWriter send = new PacketWriter())
                    {
                        send.WriteH(0);
                        send.WriteD(visit.startDate);
                        send.WriteS(visit.title, 60);
                        send.WriteC(2);
                        send.WriteC(visit.checks);
                        send.WriteH(0);
                        send.WriteD(visit.id);
                        send.WriteD(visit.startDate);
                        send.WriteD(visit.endDate);
                        bool versiontype = Settings.ClientVersion == "1.15.39" || Settings.ClientVersion == "1.15.41" || Settings.ClientVersion == "1.15.42";
                        for (int i = 0; i < 8; i++)
                        {
                            VisitBox box = visit.box[i];
                            if (versiontype)
                            {
                                send.WriteC((byte)box.RewardCount);
                            }
                            else
                            {
                                send.WriteD(box.RewardCount);
                            }
                            send.WriteD(box.reward1.goodId);
                            send.WriteD(box.reward2.goodId);
                        }
                        MyinfoBytes = send.memorystream.ToArray();
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

        public static EventVisitModel GetEvent(int eventId)
        {
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    EventVisitModel eventVisit = list[i];
                    if (eventVisit.id == eventId)
                    {
                        return eventVisit;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return null;
        }

        public static EventVisitModel GetRunningEvent()
        {
            try
            {
                int date = int.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                for (int i = 0; i < list.Count; i++)
                {
                    EventVisitModel eventVisit = list[i];
                    if (eventVisit.startDate <= date && date < eventVisit.endDate)
                    {
                        return eventVisit;
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

    public class EventVisitModel
    {
        public int id;
        public byte checks = 7;
        public int startDate;
        public int endDate;
        public string title = "";
        public List<VisitBox> box = new List<VisitBox>();
        public EventVisitModel()
        {
            for (int i = 0; i < 7; i++)
            {
                box.Add(new VisitBox());
            }
        }

        public bool EventIsEnabled()
        {
            int date = int.Parse(DateTime.Now.ToString("yyMMddHHmm"));
            if (startDate <= date && date < endDate)
            {
                return true;
            }
            return false;
        }

        public VisitItem GetReward(int idx, int rewardIdx)
        {
            try
            {
                return rewardIdx == 0 ? box[idx].reward1 : box[idx].reward2;
            }
            catch
            {
                return null;
            }
        }

        public void SetBoxCounts()
        {
            for (int i = 0; i < 7; i++)
            {
                box[i].SetCount();
            }
        }
    }
}