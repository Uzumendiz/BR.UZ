using Npgsql;
using System;
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
    public class TopRank
    {
        public long playerId;
        public byte rankId;
        public int exp;
    }
    public class RankManager
    {
        private static List<TopRank> TopRankings = new List<TopRank>(370);
        private static readonly List<RankModel> ranks = new List<RankModel>();
        private static readonly SortedList<int, List<ItemsModel>> awards = new SortedList<int, List<ItemsModel>>();
        private static readonly string path = "Data/RankTemplate/RankPlayerTemplate.xml";
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [RankXML] {path} no exists.");
                return;
            }
            GenerateList();
            LoadTopsRanking();
            Logger.Informations($" [Rank] Loaded {ranks.Count} ranks.");
            Logger.Informations($" [Rank] Loaded {awards.Count} ranks awards.");
            Logger.Informations($" [Rank] Loaded {TopRankings.Count} ranks stars.");
        }

        public static void ReGenerateList()
        {
            lock (ranks)
            {
                ranks.Clear();
                Load();
            }
        }
        public static void GetRankCounts(out int ranks47, out int ranks48, out int ranks49, out int ranks50, out int ranks51)
        {
            ranks47 = 0;
            ranks48 = 0;
            ranks49 = 0;
            ranks50 = 0;
            ranks51 = 0;
            for (int i = 0; i < TopRankings.Count; i++)
            {
                TopRank rank = TopRankings[i];
                if (rank.playerId == 0)
                {
                    continue;
                }
                if (rank.rankId == 47)
                {
                    ranks47++;
                }
                else if (rank.rankId == 48)
                {
                    ranks48++;
                }
                else if (rank.rankId == 49)
                {
                    ranks49++;
                }
                else if (rank.rankId == 50)
                {
                    ranks50++;
                }
                else if (rank.rankId == 51)
                {
                    ranks51++;
                }
            }
        }
        public static bool CheckUp(long playerId, byte rankId, int experience, out long playerIdReduceRank)
        {
            lock (TopRankings)
            {
                playerIdReduceRank = 0;
                byte Ranks47 = 0;
                byte Ranks48 = 0;
                byte Ranks49 = 0;
                byte Ranks50 = 0;
                byte Ranks51 = 0;
                for (int i = 0; i < TopRankings.Count; i++)
                {
                    TopRank rank = TopRankings[i];
                    if (rank == null)
                    {
                        continue;
                    }
                    if (rank.rankId == 47)
                    {
                        Ranks47++;
                    }
                    else if (rank.rankId == 48)
                    {
                        Ranks48++;
                    }
                    else if (rank.rankId == 49)
                    {
                        Ranks49++;
                    }
                    else if (rank.rankId == 50)
                    {
                        Ranks50++;
                    }
                    else if (rank.rankId == 51)
                    {
                        Ranks51++;
                    }
                }
                if ((rankId == 46 && Ranks47 >= Settings.MaxRanks47) || (rankId == 47 && Ranks48 >= Settings.MaxRanks48) || (rankId == 48 && Ranks49 >= Settings.MaxRanks49) || (rankId == 49 && Ranks50 >= Settings.MaxRanks50) || (rankId == 50 && Ranks51 >= Settings.MaxRanks51))
                {
                    for (int j = 0; j < TopRankings.Count; j++)
                    {
                        TopRank rank = TopRankings[j];
                        if (rank != null && rank.playerId != playerId && rank.rankId > rankId && rank.exp < experience)
                        {
                            if (rank.rankId == 47 && TopRankings.Remove(rank))
                            {
                                TopRankings.Add(new TopRank
                                {
                                    playerId = playerId,
                                    exp = experience,
                                    rankId = rankId++
                                });
                            }
                            playerIdReduceRank = rank.playerId;
                            return true;
                        }
                    }
                }
                else
                {
                    TopRank rank = new TopRank
                    {
                        playerId = playerId,
                        exp = experience,
                        rankId = rankId++
                    };
                    TopRankings.Add(rank);
                }
                return false;
            }
        }

        public static void LoadTopsRanking()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "SELECT id, exp, rank FROM accounts WHERE rank<'52' AND rank>'46' AND exp>'0' ORDER BY exp DESC";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            TopRank rank = new TopRank
                            {
                                playerId = data.GetInt64(0),
                                exp = data.GetInt32(1),
                                rankId = (byte)data.GetInt32(2)
                            };
                            if (rank.rankId > 46)
                            {
                                TopRankings.Add(rank);
                            }
                        }
                        data.Close();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
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
                for (XmlNode PrimaryNode = document.FirstChild; PrimaryNode != null; PrimaryNode = PrimaryNode.NextSibling)
                {
                    if ("list".Equals(PrimaryNode.Name))
                    {
                        for (XmlNode SecondaryNode = PrimaryNode.FirstChild; SecondaryNode != null; SecondaryNode = SecondaryNode.NextSibling)
                        {
                            if ("Rank".Equals(SecondaryNode.Name))
                            {
                                XmlNamedNodeMap PrimaryMap = SecondaryNode.Attributes;
                                byte rankId = byte.Parse(PrimaryMap.GetNamedItem("Id").Value);
                                ranks.Add(new RankModel(rankId,
                                    int.Parse(PrimaryMap.GetNamedItem("NextLevel").Value),
                                    int.Parse(PrimaryMap.GetNamedItem("Gold").Value),
                                    int.Parse(PrimaryMap.GetNamedItem("AllExp").Value),
                                    int.Parse(PrimaryMap.GetNamedItem("Brooch").Value),
                                    int.Parse(PrimaryMap.GetNamedItem("Insignia").Value),
                                    int.Parse(PrimaryMap.GetNamedItem("Medal").Value),
                                    int.Parse(PrimaryMap.GetNamedItem("BlueOrder").Value)));

                                for (XmlNode ThirdNode = SecondaryNode.FirstChild; ThirdNode != null; ThirdNode = ThirdNode.NextSibling)
                                {
                                    if ("Item".Equals(ThirdNode.Name))
                                    {
                                        XmlNamedNodeMap SecondaryMap = ThirdNode.Attributes;
                                        ItemsModel item = new ItemsModel(int.Parse(SecondaryMap.GetNamedItem("Id").Value))
                                        {
                                            name = SecondaryMap.GetNamedItem("Name").Value,
                                            count = int.Parse(SecondaryMap.GetNamedItem("Count").Value),
                                            equip = byte.Parse(SecondaryMap.GetNamedItem("Equip").Value),
                                        };
                                        AddItemToList(rankId, item);
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

        public static List<ItemsModel> GetAwards(int rank)
        {
            lock (awards)
            {
                if (awards.TryGetValue(rank, out List<ItemsModel> model))
                {
                    return model;
                }
            }
            return new List<ItemsModel>();
        }
 
        private static void AddItemToList(int rank, ItemsModel item)
        {
            if (awards.ContainsKey(rank))
            {
                awards[rank].Add(item);
            }
            else
            {
                List<ItemsModel> items = new List<ItemsModel>
                {
                    item
                };
                awards.Add(rank, items);
            }
        }
    }
}