using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    public static class ClanManager
    {
        public static List<Clan> clans = new List<Clan>();
        public static SafeList<uint> clanPlayers = new SafeList<uint>();
        public static void Load()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "SELECT * FROM clan_data";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            long owner = data.GetInt64(3);
                            if (owner == 0)
                            {
                                continue;
                            }
                            Clan clan = new Clan
                            {
                                id = data.GetInt32(0),
                                rank = (byte)data.GetInt32(1),
                                name = data.GetString(2),
                                ownerId = owner,
                                logo = (uint)data.GetInt64(4),
                                nameColor = (byte)data.GetInt32(5),
                                informations = data.GetString(6),
                                notice = data.GetString(7),
                                creationDate = data.GetInt32(8),
                                authorityConfig = (ClanAuthorityConfigEnum)data.GetInt32(9),
                                limitRankId = data.GetInt32(10),
                                limitAgeBigger = data.GetInt32(11),
                                limitAgeSmaller = data.GetInt32(12),
                                partidas = data.GetInt32(13),
                                vitorias = data.GetInt32(14),
                                derrotas = data.GetInt32(15),
                                pontos = data.GetFloat(16),
                                maxPlayers = data.GetInt32(17),
                                exp = data.GetInt32(18)
                            };
                            clan.BestPlayers.SetPlayers(data.GetString(19), data.GetString(20), data.GetString(21), data.GetString(22), data.GetString(23));
                            clans.Add(clan);
                        }
                        data.Close();
                        connection.Close();
                    }
                }
                Logger.Informations($" [ClanManager] Loaded {clans.Count} clans.");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public static List<Clan> GetClanListPerPage(int page)
        {
            List<Clan> clans = new List<Clan>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@page", 170 * page);
                    command.CommandText = "SELECT * FROM clan_data ORDER BY clan_id DESC OFFSET @page LIMIT 170";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            long owner = data.GetInt64(3);
                            if (owner == 0)
                            {
                                continue;
                            }
                            Clan clan = new Clan
                            {
                                id = data.GetInt32(0),
                                rank = (byte)data.GetInt32(1),
                                name = data.GetString(2),
                                ownerId = owner,
                                logo = (uint)data.GetInt64(4),
                                nameColor = (byte)data.GetInt32(5),
                                informations = data.GetString(6),
                                notice = data.GetString(7),
                                creationDate = data.GetInt32(8),
                                authorityConfig = (ClanAuthorityConfigEnum)data.GetInt32(9),
                                limitRankId = data.GetInt32(10),
                                limitAgeBigger = data.GetInt32(11),
                                limitAgeSmaller = data.GetInt32(12),
                                partidas = data.GetInt32(13),
                                vitorias = data.GetInt32(14),
                                derrotas = data.GetInt32(15),
                                pontos = data.GetFloat(16),
                                maxPlayers = data.GetInt32(17),
                                exp = data.GetInt32(18)
                            };
                            clan.BestPlayers.SetPlayers(data.GetString(19), data.GetString(20), data.GetString(21), data.GetString(22), data.GetString(23));
                            clans.Add(clan);
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
            return clans;
        }

        /// <summary>
        /// Procura um Clã no cache do servidor pelo número do clã.
        /// </summary>
        /// <param name="id">Número do clã</param>
        /// <returns></returns>
        public static Clan GetClan(int id)
        {
            if (id != 0)
            {
                lock (clans)
                {
                    for (int i = 0; i < clans.Count; i++)
                    {
                        Clan clan = clans[i];
                        if (clan.id == id)
                        {
                            return clan;
                        }
                    }
                }
            }
            return new Clan();
        }

        public static List<Account> GetClanPlayers(int clanId, long exception)
        {
            List<Account> friends = new List<Account>();
            if (clanId == 0)
            {
                return friends;
            }
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@clanId", clanId);
                    command.CommandText = $"SELECT id,nickname,rank,online,status FROM accounts WHERE clan_id=@clanId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            long pId = data.GetInt64(0);
                            if (pId == exception)
                            {
                                continue;
                            }
                            Account account = new Account
                            {
                                playerId = pId,
                                nickname = data.GetString(1),
                                rankId = (byte)data.GetInt32(2),
                                isOnline = data.GetBoolean(3)
                            };
                            account.status.SetData((uint)data.GetInt64(4), pId);
                            if (account.isOnline && !AccountManager.accounts.ContainsKey(pId))
                            {
                                account.SetOnlineStatus(false);
                                account.status.ResetData(account.playerId);
                            }
                            friends.Add(account);
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
            return friends;
        }

        public static bool RemoveClan(Clan clan)
        {
            lock (clans)
            {
                return clans.Remove(clan);
            }
        }

        public static void AddClan(Clan clan)
        {
            lock (clans)
            {
                clans.Add(clan);
            }
        }
        public static bool CheckNameLengthInvalid(string name) => name.Length < Settings.ClanNameMinLength || name.Length > Settings.ClanNameMaxLength;
        public static async Task<bool> IsClanNameExist(string name)
        {
            try
            {
                int value = 0;
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    await connection.OpenAsync();
                    command.Parameters.AddWithValue("@clanName", name);
                    command.CommandText = $"SELECT COUNT(*) FROM clan_data WHERE clan_name=@clanName";
                    value = Convert.ToInt32(await command.ExecuteScalarAsync());
                    await connection.CloseAsync();
                }
                return value > 0;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return true;
        }

        public static async Task<bool> IsClanLogoExist(uint logoId)
        {
            try
            {
                int value = 0;
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    await connection.OpenAsync();
                    command.Parameters.AddWithValue("@clanLogo", (long)logoId);
                    command.CommandText = $"SELECT COUNT(*) FROM clan_data WHERE logo=@clanLogo";
                    value = Convert.ToInt32(await command.ExecuteScalarAsync());
                    await connection.CloseAsync();
                }
                return value > 0;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return true;
        }

        /// <summary>
        /// Procura Clã na Database, seguindo alguns parâmetros.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="type">Tipo de procura. 0 = Nome; 1 = Id</param>
        /// <returns></returns>
        public static Clan GetClanDB(object valor, int type)
        {
            try
            {
                Clan clan = new Clan();
                if ((type == 1 && (int)valor <= 0) || (type == 0 && string.IsNullOrEmpty(valor.ToString())))
                {
                    return clan;
                }
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@value", valor);
                    command.CommandText = $"SELECT * FROM clan_data WHERE {(type == 0 ? "clan_name" : "clan_id")}=@value";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            clan.id = data.GetInt32(0);
                            clan.rank = (byte)data.GetInt32(1);
                            clan.name = data.GetString(2);
                            clan.ownerId = data.GetInt64(3);
                            clan.logo = (uint)data.GetInt64(4);
                            clan.nameColor = (byte)data.GetInt32(5);
                        }
                        data.Close();
                        connection.Close();
                    }
                }
                return clan;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return new Clan();
        }
    }
}