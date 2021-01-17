using System.Collections.Concurrent;
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
    public class TitlesManager
    {
        private static readonly ConcurrentDictionary<int, TitleQ> titles = new ConcurrentDictionary<int, TitleQ>();
        public static readonly List<TitleA> awards = new List<TitleA>();
        private static readonly string path = "Data/Titles.xml";

        #region AWARDS
        /// <summary>
        /// Gera uma lista das premiações de um título.
        /// </summary>
        /// <param name="titleId">Título</param>
        /// <returns></returns>
        public static List<ItemsModel> GetAwards(int titleId)
        {
            List<ItemsModel> items = new List<ItemsModel>();
            lock (awards)
            {
                for (int i = 0; i < awards.Count; i++)
                {
                    TitleA title = awards[i];
                    if (title.id == titleId)
                    {
                        items.Add(title.item);
                    }
                }
            }
            return items;
        }
        /// <summary>
        /// Checa se existe um item de um título que possua um id igual.
        /// </summary>
        /// <param name="titleId">Título</param>
        /// <param name="itemId">Id do item</param>
        /// <returns></returns>
        public static bool Contains(int titleId, int itemId)
        {
            if (itemId == 0)
            {
                return false;
            }
            for (int i = 0; i < awards.Count; i++)
            {
                TitleA title = awards[i];
                if (title.id == titleId && title.item.id == itemId)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region TITLES
        /// <summary>
        /// Procura informações de aquisição do título na Lista.
        /// </summary>
        /// <param name="titleId">Número do título</param>
        /// <param name="ReturnNull">Se o número do título for 0 é retornado um modelo vazio</param>
        /// <returns></returns>
        public static TitleQ GetTitle(int titleId)
        {
            if (titleId == 0)
            {
                return null;
            }
            return titles.TryGetValue(titleId, out TitleQ title) ? title : null;
        }

        public static void Get2Titles(int titleId1, int titleId2, out TitleQ title1, out TitleQ title2)
        {
            title1 = new TitleQ();
            title2 = new TitleQ();
            if (titleId1 == 0 && titleId2 == 0)
            {
                return;
            }
            if (titles.TryGetValue(titleId1, out TitleQ titleQ1))
            {
                title1 = titleQ1;
            }
            if (titles.TryGetValue(titleId2, out TitleQ titleQ2))
            {
                title2 = titleQ2;
            }
        }
        public static void GetTitlesEquipped(int titleId1, int titleId2, int titleId3, out TitleQ title1, out TitleQ title2, out TitleQ title3)
        {
            title1 = new TitleQ();
            title2 = new TitleQ();
            title3 = new TitleQ();
            if (titleId1 == 0 && titleId2 == 0 && titleId3 == 0)
            {
                return;
            }
            if (titles.TryGetValue(titleId1, out TitleQ titleQ1))
            {
                title1 = titleQ1;
            }
            if (titles.TryGetValue(titleId2, out TitleQ titleQ2))
            {
                title2 = titleQ2;
            }
            if (titles.TryGetValue(titleId3, out TitleQ titleQ3))
            {
                title3 = titleQ3;
            }
        }
        #endregion

        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [Titles] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [Titles] Loaded {titles.Count} titles informations.");
            Logger.Informations($" [Titles] Loaded {awards.Count} titles awards.");
        }

        public static void ReGenerateList()
        {
            titles.Clear();
            Load();
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
                        for (XmlNode SecondaryNode = PrimaryNode.FirstChild; SecondaryNode != null; SecondaryNode = SecondaryNode.NextSibling)
                        {
                            if ("Title".Equals(SecondaryNode.Name))
                            {
                                XmlNamedNodeMap PrimaryMap = SecondaryNode.Attributes;
                                int titleId = int.Parse(PrimaryMap.GetNamedItem("Id").Value);
                                titles.TryAdd(titleId, new TitleQ(titleId)
                                {
                                    classId = int.Parse(PrimaryMap.GetNamedItem("List").Value),
                                    medals = int.Parse(PrimaryMap.GetNamedItem("Medals").Value),
                                    brooch = int.Parse(PrimaryMap.GetNamedItem("Brooch").Value),
                                    blueOrder = int.Parse(PrimaryMap.GetNamedItem("BlueOrder").Value),
                                    insignia = int.Parse(PrimaryMap.GetNamedItem("Insignia").Value),
                                    rank = int.Parse(PrimaryMap.GetNamedItem("Rank").Value),
                                    slot = byte.Parse(PrimaryMap.GetNamedItem("Slot").Value),
                                    req1 = int.Parse(PrimaryMap.GetNamedItem("RequestT1").Value),
                                    req2 = int.Parse(PrimaryMap.GetNamedItem("RequestT2").Value)
                                });
                                for (XmlNode ThirdNode = SecondaryNode.FirstChild; ThirdNode != null; ThirdNode = ThirdNode.NextSibling)
                                {
                                    if ("Item".Equals(ThirdNode.Name))
                                    {
                                        XmlNamedNodeMap SecondaryMap = ThirdNode.Attributes;
                                        awards.Add(new TitleA
                                        {
                                            id = titleId,
                                            item = new ItemsModel(int.Parse(SecondaryMap.GetNamedItem("Id").Value), SecondaryMap.GetNamedItem("Name").Value, byte.Parse(SecondaryMap.GetNamedItem("Equip").Value), int.Parse(SecondaryMap.GetNamedItem("Count").Value))
                                        });
                                    }
                                }
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