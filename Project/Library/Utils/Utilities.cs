using Npgsql;
using System;
using System.Collections.Generic;

namespace PointBlank
{
    public class Utilities
    {
        //MAKE_FRIEND_STATE                                  =  (state, server, channel, room) ((((state) & 0x0000000F) << 28) | (((server) & 0x000000FF) << 20) | (((channel) & 0x000000FF) << 12) | ((room) & 0x00000FFF))
        //MAKE_FRIEND_MATCH_STATE                            =  (match, state, server, channel, room) (((UINT64)(match) & 0x00000000FF) << 32) | ((((state) & 0x000000000F) << 28) | (((server) & 0x00000000FF) << 20) | (((channel) & 0x00000000FF) << 12) | ((room) & 0x0000000FFF))
        //GET_FRIEND_MATCH                                   =  (state) (((state) >> 32) & 0x00000000000000FF)
        //GET_FRIEND_STATE                                   =  (state) (((state) >> 28) & 0x0000000F)
        //GET_FRIEND_SERVER                                  =  (state) (((state) >> 20) & 0x000000FF)
        //GET_FRIEND_CHANNEL                                 =  (state) (((state) >> 12) & 0x000000FF)
        //GET_FRIEND_ROOMIDX                                 =  (state) ((state) & 0x00000FFF)
        //SET_FRIEND_MATCH                                   =  (state, match) (state | (((UINT64) match & 0x00000000000000FF ) << 32))
        //CLEAR_FRIEND_MATCH                                 =  (state) (state & 0xFFFFFF00FFFFFFFF)
        public static bool ExecuteQuery(string query)
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand Command = connection.CreateCommand())
                {
                    connection.Open();
                    Command.CommandText = query;
                    success = Command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }
        public static bool UpdateDB(string TABELA, string req1, object valorReq1, string[] COLUNAS, params object[] VALORES)
        {
            int success = 0;
            if (COLUNAS.Length > 0 && VALORES.Length > 0 && COLUNAS.Length != VALORES.Length)
            {
                return false;
            }
            else if (COLUNAS.Length == 0 || VALORES.Length == 0)
            {
                return false;
            }
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    string loaded = "";
                    List<string> parameters = new List<string>();
                    for (int i = 0; i < VALORES.Length; i++)
                    {
                        object obj = VALORES[i];
                        string column = COLUNAS[i];
                        string param = "@valor" + i;
                        command.Parameters.AddWithValue(param, obj);
                        parameters.Add($"{column}={param}");
                    }
                    loaded = string.Join(",", parameters.ToArray());
                    command.Parameters.AddWithValue("@req1", valorReq1);
                    command.CommandText = $"UPDATE {TABELA} SET {loaded} WHERE {req1}=@req1";
                    success = command.ExecuteNonQuery();
                    connection.Close();
                    if (success > 1)
                    {
                        Logger.Warning($" [Utilities] (UpdateDB) Query: {command.CommandText} Success: {success}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }
        public static int GetItemCategory(int id)
        {
            int value = GetIdStatics(id, 1);
            if (value >= 1 && value <= 9)
            {
                return 1;
            }
            else if (value >= 10 && value <= 11)
            {
                return 2;
            }
            else if (value >= 12 && value <= 19)
            {
                return 3;
            }
            else
            {
                Logger.Warning(" [ComDiv] [GetItemCategory] Invalid itemId: " + id);
            }
            return 0;
        }

        public static bool DeleteDB(string TABELA, string req1, object[] valorReq1, string req2, object valorReq2)
        {
            int success = 0;
            if (valorReq1.Length == 0)
            {
                return false;
            }
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    string loaded = "";
                    List<string> parameters = new List<string>();
                    for (int i = 0; i < valorReq1.Length; i++)
                    {
                        object obj = valorReq1[i];
                        string param = "@valor" + i;
                        command.Parameters.AddWithValue(param, obj);
                        parameters.Add(param);
                    }
                    loaded = string.Join(",", parameters.ToArray());
                    command.Parameters.AddWithValue("@req2", valorReq2);
                    command.CommandText = "DELETE FROM " + TABELA + " WHERE " + req1 + " in (" + loaded + ") AND " + req2 + "=@req2";
                    success = command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }

        public static uint GetPlayerStatus(AccountStatus status, bool isOnline)
        {
            GetPlayerLocation(status, isOnline, out FriendStateEnum state, out int roomId, out int channelId, out int serverId);
            return GetPlayerStatus(roomId, channelId, serverId, (int)state);
        }

        public static uint GetPlayerStatus(int roomId, int channelId, int serverId, int stateId)
        {
            int p1 = (stateId & 0xFF) << 28,
                 p2 = (serverId & 0xFF) << 20,
                 p3 = (channelId & 0xFF) << 12,
                 p4 = roomId & 0xFFF;
            return (uint)(p1 | p2 | p3 | p4);
        }

        public static ulong GetPlayerStatus(int clanFId, int roomId, int channelId, int serverId, int stateId)
        {
            long p1 = (clanFId & 0xFFFFFFFF) << 32,
                p2 = (stateId & 0xFF) << 28,
                p3 = (serverId & 0xFF) << 20,
                p4 = (channelId & 0xFF) << 12,
                p5 = roomId & 0xFFF;
            return (ulong)(p1 | p2 | p3 | p4 | p5);
        }

        public static ulong GetClanStatus(AccountStatus status, bool isOnline)
        {
            GetPlayerLocation(status, isOnline, out FriendStateEnum state, out int roomId, out int channelId, out int serverId, out int clanFId);
            return GetPlayerStatus(clanFId, roomId, channelId, serverId, (int)state);
        }

        public static ulong GetClanStatus(FriendStateEnum state)
        {
            return GetPlayerStatus(0, 0, 0, 0, (int)state);
        }

        public static uint GetFriendStatus(Friend friend)
        {
            PlayerInfo info = friend.player;
            if (info == null)
            {
                return 0;
            }
            int serverId = 0, channelId = 0, roomId = 0;
            FriendStateEnum state = FriendStateEnum.None;
            if (friend.removed)
            {
                state = FriendStateEnum.Offline;
            }
            else if (friend.state > 0)
            {
                state = (FriendStateEnum)friend.state;
            }
            else
            {
                GetPlayerLocation(info.status, info.isOnline, out state, out roomId, out channelId, out serverId);
            }
            return GetPlayerStatus(roomId, channelId, serverId, (int)state);
        }

        public static uint GetFriendStatus(Friend friend, FriendStateEnum stateN)
        {
            PlayerInfo info = friend.player;
            FriendStateEnum state = stateN;
            int serverId = 0, channelId = 0, roomId = 0;
            if (friend.removed)
            {
                state = FriendStateEnum.Offline;
            }
            else if (friend.state > 0)
            {
                state = (FriendStateEnum)friend.state;
            }
            else if (stateN == 0)
            {
                GetPlayerLocation(info.status, info.isOnline, out state, out roomId, out channelId, out serverId);
            }
            return GetPlayerStatus(roomId, channelId, serverId, (int)state);
        }
        public static void GetPlayerLocation(AccountStatus status, bool isOnline, out FriendStateEnum state, out int roomId, out int channelId, out int serverId)
        {
            roomId = 0;
            channelId = 0;
            serverId = 0;
            if (isOnline)
            {
                if (status.roomId != 255)
                {
                    roomId = status.roomId;
                    channelId = status.channelId;
                    state = FriendStateEnum.Room;
                }
                else if (status.roomId == 255 && status.channelId != 255)
                {
                    channelId = status.channelId;
                    state = FriendStateEnum.Lobby;
                }
                else if (status.roomId == 255 && status.channelId == 255)
                {
                    state = FriendStateEnum.Online;
                }
                else
                {
                    state = FriendStateEnum.Offline;
                }
                if (status.serverId != 255)
                {
                    serverId = status.serverId;
                }
            }
            else
            {
                state = FriendStateEnum.Offline;
            }
        }
        public static void GetPlayerLocation(AccountStatus status, bool isOnline, out FriendStateEnum state, out int roomId, out int channelId, out int serverId, out int clanFId)
        {
            roomId = 0;
            channelId = 0;
            serverId = 0;
            clanFId = 0;
            if (isOnline)
            {
                if (status.roomId != 255)
                {
                    roomId = status.roomId;
                    channelId = status.channelId;
                    state = FriendStateEnum.Room;
                }
                else if ((status.clanFId != 255 || status.roomId == 255) && status.channelId != 255)
                {
                    channelId = status.channelId;
                    state = FriendStateEnum.Lobby;
                }
                else if (status.roomId == 255 && status.channelId == 255)
                {
                    state = FriendStateEnum.Online;
                }
                else
                {
                    state = FriendStateEnum.Offline;
                }
                if (status.serverId != 255)
                {
                    serverId = status.serverId;
                }
                if (status.clanFId != 255)
                {
                    clanFId = status.clanFId + 1;
                }
            }
            else
            {
                state = FriendStateEnum.Offline;
            }
        }

        /// <summary>
        /// Gera informações do Id de um item.
        /// </summary>
        /// <param name="weaponId">Id do item</param>
        /// <param name="type">Tipo de informação. 1 = Inicio (ITEM_CLASS); 2 = Usage; 3 = Meio (ClassType); 4 = Final (Number)</param>
        /// <returns></returns>
        public static int GetIdStatics(int weaponId, int type)
        {
            try
            {
                if (type == 1)
                {
                    return weaponId / 100000000; //primeiros valores - classtype
                }
                else if (type == 2)
                {
                    return weaponId % 100000000 / 1000000; //usage
                }
                else if (type == 3)
                {
                    return weaponId % 1000000 / 1000; //valores do meio - type
                }
                else if (type == 4)
                {
                    return weaponId % 1000; //ultimos 3 valores - number
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return 0;
        }

        public static int GetItemIdClass(int id)
        {
            try
            {
                return id / 10000;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return 0;
        }

        /// <summary>
        /// Retorna a classe de um equipamento.
        /// </summary>
        /// <param name="id">Id do item</param>
        /// <returns></returns>
        public static ClassTypeEnum GetIdClassType(int id)
        {
            try
            {
                return (ClassTypeEnum)(id % 1000000 / 1000);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return 0;
        }
    }
}