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
    public class MissionAwards
    {
        private static readonly List<MisAwards> awards = new List<MisAwards>();
        private static readonly string path = "Data/Cards/MissionAwards.xml";
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [MissionAwards] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [MissionAwards] Loaded {awards.Count} missions awards.");
        }

        private static void GenerateList()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                for (XmlNode xmlNode1 = document.FirstChild; xmlNode1 != null; xmlNode1 = xmlNode1.NextSibling)
                {
                    if ("list".Equals(xmlNode1.Name))
                    {
                        for (XmlNode xmlNode2 = xmlNode1.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
                        {
                            if ("mission".Equals(xmlNode2.Name))
                            {
                                XmlNamedNodeMap xml = xmlNode2.Attributes;
                                int id = int.Parse(xml.GetNamedItem("id").Value);
                                int blueOrder = int.Parse(xml.GetNamedItem("blueOrder").Value);
                                int exp = int.Parse(xml.GetNamedItem("exp").Value);
                                int gold = int.Parse(xml.GetNamedItem("gold").Value);
                                awards.Add(new MisAwards(id, blueOrder, exp, gold));
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                Logger.Exception(ex);
            }
        }
        public static MisAwards GetAward(int mission)
        {
            lock (awards)
            {
                //for (int i = 0; i < awards.Count; i++)
                //{
                //    MisAwards mis = awards[i];
                //    if (mis.id == mission)
                //    {
                //        return mis;
                //    }
                //}
                //return null;

                return awards.Where(x => x.id == mission).FirstOrDefault();
            }
        }
    }
}