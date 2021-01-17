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
    public class DefaultInventoryManager
    {
        public static readonly List<ItemsModel> defaults = new List<ItemsModel>();
        public static readonly List<ItemsModel> awards = new List<ItemsModel>();
        private static readonly string path = "Data/DefaultInventory.xml";
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [DefaultInventory] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [DefaultInventory] Loaded {defaults.Count} default items.");
            Logger.Informations($" [DefaultInventory] Loaded {awards.Count} awards items.");
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
                            if ("Item".Equals(xmlNode2.Name))
                            {
                                XmlNamedNodeMap xml = xmlNode2.Attributes;
                                bool Awarded = bool.Parse(xml.GetNamedItem("Awarded").Value);
                                ItemsModel item = new ItemsModel(int.Parse(xml.GetNamedItem("Id").Value))
                                {
                                    name = xml.GetNamedItem("Name").Value,
                                    count = int.Parse(xml.GetNamedItem("Count").Value),
                                    equip = byte.Parse(xml.GetNamedItem("Equip").Value)
                                };
                                if (Awarded)
                                {
                                    awards.Add(item);
                                }
                                else
                                {
                                    defaults.Add(item);
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

        public static void ReGenerateList()
        {
            lock (defaults)
            {
                defaults.Clear();
                awards.Clear();
                Load();
            }
        }
    }
}