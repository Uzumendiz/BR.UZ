using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PointBlank
{
    public class Clan
    {
        public int id = 0;
        public int creationDate;
        public int partidas;
        public int vitorias;
        public int derrotas;
        public ClanAuthorityConfigEnum authorityConfig;
        public int limitRankId;
        public int limitAgeBigger;
        public int limitAgeSmaller;
        public int exp;
        public byte rank;
        public byte nameColor;
        public int maxPlayers = 50;
        public string name = "";
        public string informations = "";
        public string notice = "";
        public long ownerId;
        public uint logo = 4294967295;
        public float pontos = 1000;
        public ClanBestPlayers BestPlayers = new ClanBestPlayers();
        /// <summary>
        /// Gera o tipo de unidade do clã através da quantia de jogadores adquiridas pela Database.
        /// </summary>
        /// <returns></returns>
        public byte GetClanUnit()
        {
            return GetClanUnit(GetClanPlayers());
        }

        /// <summary>
        /// Gerar o tipo de unidade do clã através da quantia de jogadores fornecida pelo aplicativo.
        /// </summary>
        /// <param name="count">Quantia de jogadores no clã.</param>
        /// <returns></returns>
        public byte GetClanUnit(int count)
        {
            //Possível 8 - "Top"
            if (count >= 250)
            {
                return 7; //Corpo
            }
            else if (count >= 200)
            {
                return 6; //Divisão
            }
            else if (count >= 150)
            {
                return 5; //Brigada
            }
            else if (count >= 100)
            {
                return 4; //Regimento
            }
            else if (count >= 50)
            {
                return 3; //Batalhão
            }
            else if (count >= 30)
            {
                return 2; //Companhia
            }
            else if (count >= 10)
            {
                return 1; //Pelotão
            }
            else return 0; //Esquadra
        }

        public bool UpdateClanInfo(int authorityConfig, int limite_rank, int limite_idade, int limite_idade2)
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"UPDATE clan_data SET autoridade='{authorityConfig}', limite_rank='{limite_rank}', limite_idade='{limite_idade}', limite_idade2='{limite_idade2}' WHERE clan_id='{id}'";
                    success = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }

        public async Task<bool> UpdateName(string txt)
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand Command = connection.CreateCommand())
                {
                    await connection.OpenAsync();
                    Command.CommandText = $"UPDATE clan_data SET clan_name='{txt}' WHERE clan_id='{id}'";
                    success = await Command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }

        public async Task<bool> UpdateLogo(uint logo)
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand Command = connection.CreateCommand())
                {
                    await connection.OpenAsync();
                    Command.CommandText = $"UPDATE clan_data SET logo='{(long)logo}' WHERE clan_id='{id}'";
                    success = await Command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }

        public bool UpdatePoints()
        {
            return Utilities.ExecuteQuery($"UPDATE clan_data SET pontos='{pontos}' WHERE clan_id='{id}'");
        }

        public bool UpdateExp()
        {
            return Utilities.ExecuteQuery($"UPDATE clan_data SET clan_exp='{exp}' WHERE clan_id='{id}'");
        }

        public bool UpdateRank()
        {
            return Utilities.ExecuteQuery($"UPDATE clan_data SET clan_rank='{rank}' WHERE clan_id='{id}'");
        }

        public bool UpdateBattles()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"UPDATE clan_data SET partidas='{partidas}', vitorias='{vitorias}', derrotas='{derrotas}' WHERE clan_id='{id}'";
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return false;
        }
        public bool UpdateBattlesReset()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"UPDATE clan_data SET partidas='0', vitorias='0', derrotas='0' WHERE clan_id='{id}'";
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return false;
        }

        public void UpdateBestPlayers()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"UPDATE clan_data SET best_exp='{BestPlayers.Exp.GetSplit()}', best_participation='{BestPlayers.Participation.GetSplit()}', best_wins='{BestPlayers.Wins.GetSplit()}', best_kills='{BestPlayers.Kills.GetSplit()}', best_headshot='{BestPlayers.Headshot.GetSplit()}' WHERE clan_id='{id}'";
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public List<Account> GetPlayers(long exception, bool useCache)
        {
            List<Account> players = new List<Account>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"SELECT id,nickname,nickcolor,rank,online,clan_authority,clan_date,status FROM accounts WHERE clan_id='{id}'";
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
                                nickcolor = (byte)data.GetInt32(2),
                                clanId = id,
                                rankId = (byte)data.GetInt32(3),
                                isOnline = data.GetBoolean(4),
                                clanAuthority = (ClanAuthorityEnum)data.GetInt32(5),
                                clanDate = data.GetInt32(6)
                            };
                            account.status.SetData((uint)data.GetInt64(7), account.playerId);
                            if (useCache)
                            {
                                Account p2 = AccountManager.GetAccount(account.playerId, true);
                                if (p2 != null)
                                {
                                    account.client = p2.client;
                                }
                            }
                            players.Add(account);
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
            return players;
        }

        public byte GetClanPlayers()
        {
            byte players = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"SELECT COUNT(*) FROM accounts WHERE clan_id='{id}'";
                    players = Convert.ToByte(command.ExecuteScalar());
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return players;
        }

        public List<ClanInvite> GetClanRequestList()
        {
            List<ClanInvite> invites = new List<ClanInvite>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"SELECT * FROM clan_invites WHERE clan_id='{id}'";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            invites.Add(new ClanInvite
                            {
                                clanId = id,
                                playerId = data.GetInt64(1),
                                inviteDate = data.GetInt32(2),
                                text = data.GetString(3)
                            });
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
            return invites;
        }

        public int GetRequestCount()
        {
            int count = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"SELECT COUNT(*) FROM clan_invites WHERE clan_id='{id}'";
                    count = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return count;
        }
        public int GetRequestClanId(long playerId)
        {
            int resultado = 0;
            if (playerId == 0)
            {
                return resultado;
            }
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"SELECT clan_id FROM clan_invites WHERE player_id='{playerId}'";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        if (data.Read())
                        {
                            resultado = data.GetInt32(0);
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
            return resultado;
        }

        public string GetRequestText(long playerId)
        {
            if (playerId == 0)
            {
                return null;
            }
            try
            {
                string resultado = null;
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"SELECT text FROM clan_invites WHERE clan_id='{id}' AND player_id='{playerId}'";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        if (data.Read())
                        {
                            resultado = data.GetString(0);
                        }
                        data.Close();
                        connection.Close();
                    }
                }
                return resultado;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return null;
        }

        public async Task<bool> CreateClan()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    await connection.OpenAsync();
                    command.Parameters.AddWithValue("@clanName", name);
                    command.Parameters.AddWithValue("@clanOwnerId", ownerId);
                    command.Parameters.AddWithValue("@clanDate", creationDate);
                    command.Parameters.AddWithValue("@clanInfo", informations);
                    command.CommandText = $"INSERT INTO clan_data(clan_name,owner_id,create_date,clan_info,best_exp,best_participation,best_wins,best_kills,best_headshot) VALUES (@clanName, @clanOwnerId, @clanDate, @clanInfo, '0-0', '0-0', '0-0', '0-0', '0-0') RETURNING clan_id";
                    id = Convert.ToInt32(await command.ExecuteScalarAsync());
                    await connection.CloseAsync();
                }
                return id > 0;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return false;
        }
    }
}