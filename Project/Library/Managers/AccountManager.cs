using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
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
    public static class AccountManager
    {
        public static ConcurrentDictionary<long, Account> accounts = new ConcurrentDictionary<long, Account>();
        public static bool CheckNickLengthInvalid(string nickname) => nickname.Length < Settings.NickMinLength || nickname.Length > Settings.NickMaxLength;
        public static bool CheckNicknameExist2(string nickname)
        {
            try
            {
                int value = 0;
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@nickname", nickname);
                    command.CommandText = $"SELECT COUNT(*) FROM accounts WHERE nickname=@nickname";
                    value = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
                return value > 0;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return true;
        }
        public static async Task<bool> CheckNicknameExist(string nickname)
        {
            try
            {
                int value = 0;
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    await connection.OpenAsync();
                    command.Parameters.AddWithValue("@nickname", nickname);
                    command.CommandText = $"SELECT COUNT(*) FROM accounts WHERE nickname=@nickname";
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
        public static void GetAccountsByIP(string Ip, string Mac, out int AccountsByIp, out int AccountsByMac)
        {
            AccountsByIp = 0;
            AccountsByMac = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "SELECT last_ip, last_mac FROM accounts";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            string lastIp = data.GetString(0);
                            string lastMac = data.GetString(1);
                            if (lastIp == Ip)
                            {
                                AccountsByIp++;
                            }
                            if (lastIp == Mac)
                            {
                                AccountsByMac++;
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

        public static bool AddAccount(Account account)
        {
            lock (accounts)
            {
                if (!accounts.ContainsKey(account.playerId))
                {
                    return accounts.TryAdd(account.playerId, account);
                }
            }
            return false;
        }

        /// <summary>
        /// Procura na Database uma conta. É possível escolher se será feita a procura dos Títulos, Amigos, Bônus, Eventos, Configurações.
        /// </summary>
        /// <param name="valor">Valor para procura</param>
        /// <param name="type">Tipo de procura\n0 = Login\n1 = Apelido\n2 = Id</param>
        /// <param name="searchDBFlag">Detalhes de procura (DB;Flag)\n0 = Nada\n1 = Títulos\n2 = Bônus\n4 = Amigos (Completo)\n8 = Eventos\n16 = Configurações\n32 = Amigos (Básico)</param>
        /// <returns></returns>
        public static Account GetAccountDB(object valor, int type, int searchDBFlag)
        {
            if (type == 2 && (long)valor == 0 || (type == 0 || type == 1) && (string)valor == "")
            {
                return null;
            }
            Account account = null;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@value", valor);
                    command.CommandText = $"SELECT {(searchDBFlag == -1 ? "id" : "*")} FROM accounts WHERE {(type == 0 ? "username" : type == 1 ? "nickname" : "id")}=@value";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            account = new Account();
                            account.playerId = data.GetInt64(0);
                            if (searchDBFlag == -1)
                            {
                                break;
                            }
                            account.LoadData(searchDBFlag);
                            account.token = data.GetString(1);
                            account.key = data.GetInt64(2);
                            account.login = data.GetString(3);
                            account.password = data.GetString(4);
                            //5 = email
                            account.nickname = data.GetString(6);
                            account.nickcolor = (byte)data.GetInt32(7);
                            account.exp = data.GetInt32(8);
                            account.rankId = (byte)data.GetInt32(9);
                            account.gold = data.GetInt32(10);
                            account.cash = data.GetInt32(11);
                            account.isOnline = data.GetBoolean(12);
                            account.access = (AccessLevelEnum)data.GetInt32(13);
                            account.pccafe = (byte)data.GetInt32(14);
                            account.pccafeDate = data.GetInt32(15);

                            account.equipments.primary = data.GetInt32(16);
                            account.equipments.secondary = data.GetInt32(17);
                            account.equipments.melee = data.GetInt32(18);
                            account.equipments.grenade = data.GetInt32(19);
                            account.equipments.special = data.GetInt32(20);
                            account.equipments.red = data.GetInt32(21);
                            account.equipments.blue = data.GetInt32(22);
                            account.equipments.helmet = data.GetInt32(23);
                            account.equipments.beret = data.GetInt32(24);
                            account.equipments.dino = data.GetInt32(25);

                            account.statistics.fights = data.GetInt32(26);
                            account.statistics.fightsWin = data.GetInt32(27);
                            account.statistics.fightsLost = data.GetInt32(28);
                            account.statistics.fightsDraw = data.GetInt32(29);
                            account.statistics.escapes = data.GetInt32(30);
                            account.statistics.kills = data.GetInt32(31);
                            account.statistics.deaths = data.GetInt32(32);
                            account.statistics.headshots = data.GetInt32(33);
                            account.statistics.totalfights = data.GetInt32(34);
                            account.statistics.totalkills = data.GetInt32(35);
                            account.brooch = data.GetInt32(36);
                            account.insignia = data.GetInt32(37);
                            account.medal = data.GetInt32(38);
                            account.blueorder = data.GetInt32(39);

                            account.clanId = data.GetInt32(40);
                            account.clanAuthority = (ClanAuthorityEnum)data.GetInt32(41);
                            account.clanDate = data.GetInt32(42);
                            account.statistics.clanFights = data.GetInt32(43);
                            account.statistics.clanWins = data.GetInt32(44);

                            account.status.SetData((uint)data.GetInt64(45), account.playerId);
                            account.lastRankUpDate = (uint)data.GetInt64(46);
                            //AddAccount(account);
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
            return account;
        }

        /// <summary>
        /// Procura no cache uma conta pelo Id. Caso não encontrada, é feita uma procura na Database.
        /// </summary>
        /// <param name="playerId">Id da conta</param>
        /// <param name="searchFlag">Detalhes de procura (DB;Flag)\n0 = Nada\n1 = Títulos\n2 = Bônus\n4 = Amigos (Completo)\n8 = Eventos\n16 = Configurações\n32 = Amigos (Básico)</param>
        /// <returns></returns>
        public static Account GetAccount(long playerId, int searchFlag)
        {
            if (playerId == 0)
            {
                return null;
            }
            try
            {
                return accounts.TryGetValue(playerId, out Account player) ? player : GetAccountDB(playerId, 2, searchFlag);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// Procura no cache uma conta pelo Id.
        /// <para>É possível escolher se, caso não encontrada, procurar a conta na Database.</para>
        /// <para>Flag padrão de busca: 35. (Títulos, Amigos e Bônus.)</para>
        /// </summary>
        /// <param name="playerId">Id da conta</param>
        /// <param name="noUseDB">Não buscar Database?</param>
        /// <returns></returns>
        public static Account GetAccount(long playerId, bool noUseDB)
        {
            if (playerId == 0)
            {
                return null;
            }
            try
            {
                return accounts.TryGetValue(playerId, out Account player) ? player : (noUseDB ? null : GetAccountDB(playerId, 2, 35));
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return null;
        }

        /// <summary>
        /// Pesquisa no Cache do servidor uma conta, caso não encontrada, é feita uma pesquisa na Database.
        /// </summary>
        /// <param name="text">Texto</param>
        /// <param name="type">Tipo de procura. 0 = Login; 1 = Apelido</param>
        /// <param name="searchFlag">Detalhes de procura (DB;Flag)\n0 = Nada\n1 = Títulos\n2 = Bônus\n4 = Amigos (Completo)\n8 = Eventos\n16 = Configurações\n32 = Amigos (Básico)</param>
        /// <returns></returns>
        public static Account GetAccount(string text, int searchFlag)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    return null;
                }
                foreach (Account player in accounts.Values)
                {
                    if (player != null && player.nickname == text)
                    {
                        return player;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return GetAccountDB(text, 1, searchFlag);
        }
        
        /// <summary>
         /// Pesquisa no Cache do servidor uma conta pelo nickname.
         /// </summary>
        public static Account GetAccountByNick(string nickname)
        {
            try
            {
                foreach (Account player in accounts.Values)
                {
                    if (player != null && player.nickname == nickname)
                    {
                        return player;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return null;
        }
        public static bool CreateAccount(out Account playerReturn, string login, string password)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", password);
                    command.CommandText = $"INSERT INTO accounts (username, password) VALUES (@login, @password)";
                    command.ExecuteNonQuery();

                    command.CommandText = $"SELECT * FROM accounts WHERE username=@login";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        Account account = null;
                        while (data.Read())
                        {
                            account = new Account();
                            account.playerId = data.GetInt64(0);
                            account.token = data.GetString(1);
                            account.key = data.GetInt64(2);
                            account.login = data.GetString(3);
                            account.password = data.GetString(4);
                            //5 = email
                            account.nickname = data.GetString(6);
                            account.nickcolor = (byte)data.GetInt32(7);
                            account.exp = data.GetInt32(8);
                            account.rankId = (byte)data.GetInt32(9);
                            account.gold = data.GetInt32(10);
                            account.cash = data.GetInt32(11);
                            account.isOnline = data.GetBoolean(12);
                            account.access = (AccessLevelEnum)data.GetInt32(13);
                            account.pccafe = (byte)data.GetInt32(14);
                            account.pccafeDate = data.GetInt32(15);

                            account.equipments.primary = data.GetInt32(16);
                            account.equipments.secondary = data.GetInt32(17);
                            account.equipments.melee = data.GetInt32(18);
                            account.equipments.grenade = data.GetInt32(19);
                            account.equipments.special = data.GetInt32(20);
                            account.equipments.red = data.GetInt32(21);
                            account.equipments.blue = data.GetInt32(22);
                            account.equipments.helmet = data.GetInt32(23);
                            account.equipments.beret = data.GetInt32(24);
                            account.equipments.dino = data.GetInt32(25);

                            account.statistics.fights = data.GetInt32(26);
                            account.statistics.fightsWin = data.GetInt32(27);
                            account.statistics.fightsLost = data.GetInt32(28);
                            account.statistics.fightsDraw = data.GetInt32(29);
                            account.statistics.escapes = data.GetInt32(30);
                            account.statistics.kills = data.GetInt32(31);
                            account.statistics.deaths = data.GetInt32(32);
                            account.statistics.headshots = data.GetInt32(33);
                            account.statistics.totalfights = data.GetInt32(34);
                            account.statistics.totalkills = data.GetInt32(35);
                            account.brooch = data.GetInt32(36);
                            account.insignia = data.GetInt32(37);
                            account.medal = data.GetInt32(38);
                            account.blueorder = data.GetInt32(39);

                            account.clanId = data.GetInt32(40);
                            account.clanAuthority = (ClanAuthorityEnum)data.GetInt32(41);
                            account.clanDate = data.GetInt32(42);
                            account.statistics.clanFights = data.GetInt32(43);
                            account.statistics.clanWins = data.GetInt32(44);

                            account.status.SetData((uint)data.GetInt64(45), account.playerId);
                            account.lastRankUpDate = (uint)data.GetInt64(46);
                        }
                        playerReturn = account;
                        AddAccount(account);
                        data.Close();
                        connection.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            playerReturn = null;
            return false;
        }

        public static Account SearchAndLoadAccountDB(string login)
        {
            Account account = null;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@login", login);
                    command.CommandText = $"SELECT * FROM accounts WHERE username=@login LIMIT 1";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            account = new Account();
                            account.playerId = data.GetInt64(0);
                            account.token = data.GetString(1);
                            account.key = data.GetInt64(2);
                            account.login = data.GetString(3);
                            account.password = data.GetString(4);
                            //5 = email
                            account.nickname = data.GetString(6);
                            account.nickcolor = (byte)data.GetInt32(7);
                            account.exp = data.GetInt32(8);
                            account.rankId = (byte)data.GetInt32(9);
                            account.gold = data.GetInt32(10);
                            account.cash = data.GetInt32(11);
                            account.isOnline = data.GetBoolean(12);
                            account.access = (AccessLevelEnum)data.GetInt32(13);
                            account.pccafe = (byte)data.GetInt32(14);
                            account.pccafeDate = data.GetInt32(15);

                            account.equipments.primary = data.GetInt32(16);
                            account.equipments.secondary = data.GetInt32(17);
                            account.equipments.melee = data.GetInt32(18);
                            account.equipments.grenade = data.GetInt32(19);
                            account.equipments.special = data.GetInt32(20);
                            account.equipments.red = data.GetInt32(21);
                            account.equipments.blue = data.GetInt32(22);
                            account.equipments.helmet = data.GetInt32(23);
                            account.equipments.beret = data.GetInt32(24);
                            account.equipments.dino = data.GetInt32(25);

                            account.statistics.fights = data.GetInt32(26);
                            account.statistics.fightsWin = data.GetInt32(27);
                            account.statistics.fightsLost = data.GetInt32(28);
                            account.statistics.fightsDraw = data.GetInt32(29);
                            account.statistics.escapes = data.GetInt32(30);
                            account.statistics.kills = data.GetInt32(31);
                            account.statistics.deaths = data.GetInt32(32);
                            account.statistics.headshots = data.GetInt32(33);
                            account.statistics.totalfights = data.GetInt32(34);
                            account.statistics.totalkills = data.GetInt32(35);
                            account.brooch = data.GetInt32(36);
                            account.insignia = data.GetInt32(37);
                            account.medal = data.GetInt32(38);
                            account.blueorder = data.GetInt32(39);

                            account.clanId = data.GetInt32(40);
                            account.clanAuthority = (ClanAuthorityEnum)data.GetInt32(41);
                            account.clanDate = data.GetInt32(42);
                            account.statistics.clanFights = data.GetInt32(43);
                            account.statistics.clanWins = data.GetInt32(44);

                            account.status.SetData((uint)data.GetInt64(45), account.playerId);
                            account.lastRankUpDate = (uint)data.GetInt64(46);
                            AddAccount(account);
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
            return account;
        }

        public static Account GetAccountToken(string Token)
        {
            Account account = null;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@token", Token);
                    command.CommandText = $"SELECT * FROM accounts WHERE token=@token LIMIT 1";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            account = new Account();
                            account.playerId = data.GetInt64(0);
                            account.token = data.GetString(1);
                            account.key = data.GetInt64(2);
                            account.login = data.GetString(3);
                            account.password = data.GetString(4);
                            //5 = email
                            account.nickname = data.GetString(6);
                            account.nickcolor = (byte)data.GetInt32(7);
                            account.exp = data.GetInt32(8);
                            account.rankId = (byte)data.GetInt32(9);
                            account.gold = data.GetInt32(10);
                            account.cash = data.GetInt32(11);
                            account.isOnline = data.GetBoolean(12);
                            account.access = (AccessLevelEnum)data.GetInt32(13);
                            account.pccafe = (byte)data.GetInt32(14);
                            account.pccafeDate = data.GetInt32(15);

                            account.equipments.primary = data.GetInt32(16);
                            account.equipments.secondary = data.GetInt32(17);
                            account.equipments.melee = data.GetInt32(18);
                            account.equipments.grenade = data.GetInt32(19);
                            account.equipments.special = data.GetInt32(20);
                            account.equipments.red = data.GetInt32(21);
                            account.equipments.blue = data.GetInt32(22);
                            account.equipments.helmet = data.GetInt32(23);
                            account.equipments.beret = data.GetInt32(24);
                            account.equipments.dino = data.GetInt32(25);

                            account.statistics.fights = data.GetInt32(26);
                            account.statistics.fightsWin = data.GetInt32(27);
                            account.statistics.fightsLost = data.GetInt32(28);
                            account.statistics.fightsDraw = data.GetInt32(29);
                            account.statistics.escapes = data.GetInt32(30);
                            account.statistics.kills = data.GetInt32(31);
                            account.statistics.deaths = data.GetInt32(32);
                            account.statistics.headshots = data.GetInt32(33);
                            account.statistics.totalfights = data.GetInt32(34);
                            account.statistics.totalkills = data.GetInt32(35);
                            account.brooch = data.GetInt32(36);
                            account.insignia = data.GetInt32(37);
                            account.medal = data.GetInt32(38);
                            account.blueorder = data.GetInt32(39);

                            account.clanId = data.GetInt32(40);
                            account.clanAuthority = (ClanAuthorityEnum)data.GetInt32(41);
                            account.clanDate = data.GetInt32(42);
                            account.statistics.clanFights = data.GetInt32(43);
                            account.statistics.clanWins = data.GetInt32(44);

                            account.status.SetData((uint)data.GetInt64(45), account.playerId);
                            account.lastRankUpDate = (uint)data.GetInt64(46);
                            if (account.isOnline)
                            {
                                account.SetOnlineStatus(false);
                            }
                            AddAccount(account);
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
            return account;
        }
    }
}