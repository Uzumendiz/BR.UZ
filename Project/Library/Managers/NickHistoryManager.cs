using Npgsql;
using System;
using System.Collections.Generic;

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
    public static class NickHistoryManager
    {
        public static List<NHistoryModel> GetHistory(object valor, int type)
        {
            List<NHistoryModel> nicks = new List<NHistoryModel>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@valor", valor);
                    string moreCmd = type == 0 ? "WHERE to_nick=@valor" : "WHERE player_id=@valor";
                    command.CommandText = "SELECT * FROM nick_history " + moreCmd + " ORDER BY change_date LIMIT 30";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            nicks.Add(new NHistoryModel
                            {
                                player_id = data.GetInt64(0),
                                from_nick = data.GetString(1),
                                to_nick = data.GetString(2),
                                date = (uint)data.GetInt64(3),
                                motive = data.GetString(4)
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
            return nicks;
        }

        public static bool CreateHistory(long player_id, string old_nick, string new_nick, string motive)
        {
            NHistoryModel history = new NHistoryModel
            {
                player_id = player_id,
                from_nick = old_nick,
                to_nick = new_nick,
                date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm")),
                motive = motive
            };
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@owner", history.player_id);
                    command.Parameters.AddWithValue("@oldnick", history.from_nick);
                    command.Parameters.AddWithValue("@newnick", history.to_nick);
                    command.Parameters.AddWithValue("@date", (long)history.date);
                    command.Parameters.AddWithValue("@motive", history.motive);
                    command.CommandText = "INSERT INTO nick_history(player_id,from_nick,to_nick,change_date,motive)VALUES(@owner,@oldnick,@newnick,@date,@motive)";
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
    }

    public class NHistoryModel
    {
        public string from_nick, to_nick, motive;
        public long player_id;
        public uint date;
    }
}