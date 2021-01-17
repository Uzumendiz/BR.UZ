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
    public class RandomBoxXML
    {
        private static readonly SortedList<int, RandomBoxModel> boxes = new SortedList<int, RandomBoxModel>();
        public static void LoadBoxes()
        {
            DirectoryInfo folder = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\Data\Cupons\CuponsItems");
            if (!folder.Exists)
            {
                return;
            }
            FileInfo[] array = folder.GetFiles();
            for (int i = 0; i < array.Length; i++)
            {
                FileInfo file = array[i];
                try
                {
                    LoadBox(int.Parse(file.Name.Substring(0, file.Name.Length - 4)));
                }
                catch
                {
                }
            }
            Logger.Informations($" [RandomBox] Loaded {boxes.Count} boxes.");
        }
        public static void ReGenerateList()
        {
            lock (boxes)
            {
                boxes.Clear();
                LoadBoxes();
            }
        }
        private static void LoadBox(int id)
        {
            string path = $"Data/Cupons/CuponsItems/{id}.xml";
            if (!File.Exists(path))
            {
                Logger.Warning($" [RandomBox] {path} no exists.");
                return;
            }
            Parse(path, id);
        }
        private static void Parse(string path, int cupomId)
        {
            XmlDocument xmlDocument = new XmlDocument();
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                try
                {
                    xmlDocument.Load(fileStream);
                    for (XmlNode xmlNode1 = xmlDocument.FirstChild; xmlNode1 != null; xmlNode1 = xmlNode1.NextSibling)
                    {
                        if ("list".Equals(xmlNode1.Name))
                        {
                            XmlNamedNodeMap xml2 = xmlNode1.Attributes;
                            RandomBoxModel box = new RandomBoxModel
                            {
                                itemsCount = int.Parse(xml2.GetNamedItem("count").Value)
                            };
                            for (XmlNode xmlNode2 = xmlNode1.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
                            {
                                if ("item".Equals(xmlNode2.Name))
                                {
                                    XmlNamedNodeMap xml = xmlNode2.Attributes;
                                    ItemsModel item = null;
                                    int itemId = int.Parse(xml.GetNamedItem("id").Value);
                                    int count = int.Parse(xml.GetNamedItem("count").Value);
                                    if (itemId > 0)
                                    {
                                        item = new ItemsModel(itemId)
                                        {
                                            name = "Randombox",
                                            equip = byte.Parse(xml.GetNamedItem("equip").Value),
                                            count = count
                                        };
                                    }
                                    box.items.Add(new RandomBoxItem
                                    {
                                        index = int.Parse(xml.GetNamedItem("index").Value),
                                        percent = int.Parse(xml.GetNamedItem("percent").Value),
                                        special = bool.Parse(xml.GetNamedItem("special").Value),
                                        count = count,
                                        item = item
                                    });
                                }
                            }
                            box.SetTopPercent();
                            boxes.Add(cupomId, box);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
                fileStream.Close();
            }
        }

        public static bool ContainsBox(int id)
        {
            return boxes.ContainsKey(id);
        }

        public static RandomBoxModel GetBox(int id)
        {
            try
            {
                return boxes[id];
            }
            catch
            {
                return null;
            }
        }
    }
}