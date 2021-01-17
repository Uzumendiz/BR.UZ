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
    public class TournamentRulesManager
    {
        public static readonly List<int> @CAMP = new List<int>();
        public static readonly List<int> @CNPB = new List<int>();
        public static readonly List<int> CuponsEffectsBlocked = new List<int>();
        private static readonly string path = "Data/TournamentRules.xml";
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [TournamentRules] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [TournamentRules] Loaded {CAMP.Count} rules @CAMP.");
            Logger.Informations($" [TournamentRules] Loaded {CNPB.Count} rules @CNPB.");
            Logger.Informations($" [TournamentRules] Loaded {CuponsEffectsBlocked.Count} CUPONS.");
        }

        public static void ReGenerateList()
        {
            @CAMP.Clear();
            @CNPB.Clear();
            CuponsEffectsBlocked.Clear();
            Load();
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
                            if ("Rule".Equals(xmlNode2.Name))
                            {
                                XmlNamedNodeMap xml = xmlNode2.Attributes;
                                string rule = xml.GetNamedItem("Name").Value;
                                int itemId = int.Parse(xml.GetNamedItem("Id").Value);
                                if (rule.Equals("@CAMP"))
                                {
                                    CAMP.Add(itemId);
                                }
                                else if (rule.Equals("@CNPB"))
                                {
                                    CNPB.Add(itemId);
                                }
                                else if (rule.Equals("COUPON"))
                                {
                                    CuponsEffectsBlocked.Add(itemId);
                                }
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

        public static bool IsBlocked(string roomName, int itemIdEquipped)
        {
            if (roomName.Contains("@CAMP"))
            {
                if (CAMP.Contains(itemIdEquipped))
                {
                    return true;
                }
            }
            //if (roomName.Contains("@CNPB"))
            //{
            //    if (CNPB.Contains(itemIdEquipped))
            //    {
            //        return true;
            //    }
            //}
            return false;
        }

        public static bool CheckRoomRule(string roomName)
        {
            if (!Settings.TournamentRulesActive)
            {
                return false;
            }
            if (roomName.Contains("@CAMP") || roomName.Contains("CAMP"))
            {
                return true;
            }
            //else if (roomName.Contains("@CNPB") || roomName.Contains("CNPB"))
            //{
            //    return true;
            //}
            return false;
        }
    }
}