using Npgsql;
using System;
using System.Collections.Concurrent;

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
    public class UserBlock
    {
        public long blockId;
        public string ipAddress;
        public string macAddress;
        public string hardwareId;
        public string biosId;
        public string diskId;
        public string videoId;
        public DateTime startDate;
        public DateTime endDate;
        public string reason;
        public string linkVideo;
        public string linkPrintScreen;
        public string comment;
        public long userId;
        public long adminId;
        public string adminMac;
    }
    public class ServerBlockManager
    {
        public static ConcurrentDictionary<string, UserBlock> UsersBlock = new ConcurrentDictionary<string, UserBlock>();
        public static void Load()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "SELECT * FROM server_block";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            UserBlock block = new UserBlock
                            {
                                blockId = data.GetInt64(0),
                                ipAddress = data.GetString(1),
                                macAddress = data.GetString(2),
                                hardwareId = data.GetString(3),
                                biosId = data.GetString(4),
                                diskId = data.GetString(5),
                                videoId = data.GetString(6),
                                startDate = data.GetDateTime(7),
                                endDate = data.GetDateTime(8)
                            };
                            UsersBlock.TryAdd(block.ipAddress, block);
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

        public static void GetBlock(UserBlock userSystem, out bool IP_IsBlocked, out bool MAC_IsBlocked, out bool HWID_IsBlocked)
        {
            IP_IsBlocked = false;
            MAC_IsBlocked = false;
            HWID_IsBlocked = false;
            try
            {
                foreach (UserBlock blocked in UsersBlock.Values)
                {
                    if (blocked.endDate < DateTime.Now)
                    {
                        continue;
                    }
                    if (blocked.ipAddress != "" && blocked.ipAddress == userSystem.ipAddress)
                    {
                        IP_IsBlocked = true;
                    }
                    else if (blocked.macAddress != "" && blocked.macAddress == userSystem.macAddress)
                    {
                        MAC_IsBlocked = true;
                    }
                    else if (blocked.hardwareId != "" && blocked.hardwareId == userSystem.hardwareId)
                    {
                        HWID_IsBlocked = true;
                    }
                    else if (blocked.biosId != "" && blocked.biosId == userSystem.biosId)
                    {
                        HWID_IsBlocked = true;
                    }
                    else if (blocked.diskId != "" && blocked.diskId == userSystem.diskId)
                    {
                        HWID_IsBlocked = true;
                    }
                    else if (blocked.videoId != "" && blocked.videoId == userSystem.videoId)
                    {
                        HWID_IsBlocked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public static bool AddBlock(UserBlock user, Account admin)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@address", user.ipAddress);
                    command.Parameters.AddWithValue("@mac", user.macAddress);
                    command.Parameters.AddWithValue("@hardware_id", user.hardwareId);
                    command.Parameters.AddWithValue("@bios_id", user.biosId);
                    command.Parameters.AddWithValue("@disk_id", user.diskId);
                    command.Parameters.AddWithValue("@video_id", user.videoId);
                    command.Parameters.AddWithValue("@start_date", user.startDate);
                    command.Parameters.AddWithValue("@end_date", user.endDate);
                    command.Parameters.AddWithValue("@reason", user.reason);
                    command.Parameters.AddWithValue("@link_video", user.linkVideo);
                    command.Parameters.AddWithValue("@link_printscreen", user.linkPrintScreen);
                    command.Parameters.AddWithValue("@comment", user.comment);
                    command.Parameters.AddWithValue("@user_id", user.userId);
                    command.Parameters.AddWithValue("@admin_id", admin.playerId);
                    command.Parameters.AddWithValue("@admin_mac", admin.macAddress.ToString());
                    command.CommandText = $"INSERT INTO server_block(address, mac, hardware_id, bios_id, disk_id, video_id, start_date, end_date, reason, link_video, link_printscreen, comment, user_id, admin_id, admin_mac)VALUES(@address, @mac, @hardware_id, @bios_id, @disk_id, @video_id, @start_date, @end_date, @reason, @link_video, @link_printscreen, @comment, @user_id, @admin_id, @admin_mac) RETURNING block_id";
                    user.blockId = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
                if (user.blockId > 0)
                {
                    UsersBlock.TryAdd(user.ipAddress, user);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return false;
        }
    }
}
