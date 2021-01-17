using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PointBlank
{
    public class AccountRank
    {
        public string Nickname;
        public int Position;
        public int RankId;
        public int Exp;
        public int Kill;
        public int Death;
        public int HeadShots;
        public int Fights;
        public int FightsWin;
        public int FightsLost;
        public int Escapes;
        public double KDPercent;
        public double HSPercent;
        public double WinPercent;
    }

    public class RankingManager
    {
        public static List<AccountRank> RankingCache = new List<AccountRank>();
        public static void UpdateRanking()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
            {
                connection.Open();
                while (true)
                {
                    RankingCache.Clear();
                    RankingCache = GetRankList(connection);
                    Logger.Success($" [RankingManager] Loaded {RankingCache.Count} ranklist.");
                    foreach (AccountRank rank in RankingCache)
                    {
                        decimal kill = Convert.ToDecimal(rank.Kill);
                        decimal death = Convert.ToDecimal(rank.Death);
                        decimal KD = kill + death;
                        decimal percent = kill * new decimal(100) / KD;

                        rank.KDPercent = Convert.ToDouble(Math.Round(percent, 2));

                        string KD_REAL = string.Concat(Convert.ToString(Math.Round(percent, 2)), "%");
                        string KD_ATUAL = string.Concat(Convert.ToString(Math.Round(percent, 0)), "%");

                        Logger.Warning($" Pos: {rank.Position} Nick: {rank.Nickname} Exp: {rank.Exp} Rank: {rank.RankId} Kill: {rank.Kill} Death: {rank.Death} HeadShots: {rank.HeadShots} Win: {rank.FightsWin} Lost: {rank.FightsLost} Escapes: {rank.Escapes} KD%: {rank.KDPercent} HS%: {rank.HSPercent} Win/Lost%: {rank.WinPercent}");
                    }
                    Thread.Sleep(TimeSpan.FromHours(1));
                }
            }
        }

        public static List<AccountRank> GetRankList(NpgsqlConnection connection)
        {
            List<AccountRank> ranklist = new List<AccountRank>();
            try
            {
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT* FROM accounts WHERE rank<'52' AND rank>'0' AND exp>'0' AND player_name<>'' ORDER BY exp DESC LIMIT 15";
                    using (NpgsqlDataReader DataReader = command.ExecuteReader())
                    {
                        int position = 1;
                        while (DataReader.Read())
                        {
                            AccountRank rank = new AccountRank
                            {
                                Position = position,
                                Nickname = DataReader.GetString(3),
                                RankId = DataReader.GetInt32(6),
                                Exp = DataReader.GetInt32(8),

                                Kill = DataReader.GetInt32(13),
                                Death = DataReader.GetInt32(14),
                                HeadShots = DataReader.GetInt32(15),

                                Fights = DataReader.GetInt32(10),
                                FightsWin = DataReader.GetInt32(11),
                                FightsLost = DataReader.GetInt32(12),
                                Escapes = DataReader.GetInt32(16)
                            };
                            rank.KDPercent = CalculeKD(rank.Kill, rank.Death);
                            rank.HSPercent = CalculeHS(rank.Kill, rank.HeadShots);
                            if (!string.IsNullOrEmpty(rank.Nickname) && !string.IsNullOrWhiteSpace(rank.Nickname) && rank.RankId > 0 && rank.RankId < 52 && rank.Exp > 0)
                            {
                                ranklist.Add(rank);
                            }
                            position++;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Logger.Exception(ex);
            }
            return ranklist;
        }

        public static double CalculeKD(int kill, int death)
        {
            try
            {
                decimal kills = Convert.ToDecimal(kill);
                decimal deaths = Convert.ToDecimal(death);
                decimal KD = kill + death;
                decimal percent = kills * new decimal(100) / KD;

                //string KD_REAL = string.Concat(Convert.ToString(Math.Round(percent, 2)), "%");
                //string KD_ATUAL = string.Concat(Convert.ToString(Math.Round(percent, 0)), "%");

                return Convert.ToDouble(Math.Round(percent, 2));
            }
            catch
            {
                return 0;
            }
        }

        public static double CalculeHS(int kill, int headshot)
        {
            try
            {
                decimal kills = Convert.ToDecimal(kill);
                decimal headshots = Convert.ToDecimal(headshot);
                decimal percent = headshots * new decimal(100) / kills;
                return Convert.ToDouble(Math.Round(percent, 2));
            }
            catch
            {
                return 0;
            }
        }
    }
}