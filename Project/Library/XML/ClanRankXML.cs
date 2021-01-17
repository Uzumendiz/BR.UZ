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
    public class ClanRankXML
    {
        private static readonly List<RankModel> ranks = new List<RankModel>();
        private static readonly string path = "Data/RankTemplate/RankClanTemplate.xml";
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [ClanRankXML] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [ClanRankXML] Loaded {ranks.Count} clan ranks.");
        }

        public static RankModel GetRank(int rankId)
        {
            lock (ranks)
            {
                //for (int i = 0; i < ranks.Count; i++)
                //{
                //    RankModel rank = ranks[i];
                //    if (rank.rankId == rankId)
                //    {
                //        return rank;
                //    }
                //}
                //return null;

                return ranks.Where(x => x.rankId == rankId).FirstOrDefault();
            }
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
                            if ("rank".Equals(xmlNode2.Name))
                            {
                                XmlNamedNodeMap xml = xmlNode2.Attributes;
                                ranks.Add(new RankModel(byte.Parse(xml.GetNamedItem("id").Value),
                                    int.Parse(xml.GetNamedItem("onNextLevel").Value), 0,
                                    int.Parse(xml.GetNamedItem("onAllExp").Value)));
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
    }
}