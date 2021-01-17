using Npgsql;
using PointBlank.Game;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace PointBlank
{
    public class Account
    {
        public long playerId;
        public string token;
        public long key;
        public string login;
        public string password;

        public long banId;
        public string lastIp;
        public uint lastRankUpDate;
        public uint lastLoginDate;
        public int lastRoomPage;
        public int lastPlayerPage;

        public int channelId = -1;

        public int clanId;
        public ClanAuthorityEnum clanAuthority;
        public int clanDate;

        public string nickname = "";
        public byte nickcolor;
        public byte rankId;
        public int exp;
        public int gold;
        public int cash;
        public int cashReceivedLastMatch; //Quantidade de cash recebido na ultima partida
        public byte tourneyLevel;
        public byte pccafe;
        public int pccafeDate;
        public byte age = 18;
        public byte country = 31; //Brazil=31

        public int brooch;
        public int insignia;
        public int medal;
        public int blueorder;

        public int slotId = -1;
        public int matchSlot = -1;
        public bool isOnline;
        public bool hideGMcolor;
        public bool antiKickGM;
        public bool loadedShop;
        public bool firstLobbyEnter;
        public bool showBoxWelcome;
        public bool checkEventVisitReward;
        public bool checkEventPlayTimeReward;
        public bool checkEventVisitConfirm;

        public bool checkSourceInfo;
        public bool checkUserFriends;
        public bool checkUserInfo;
        public bool checkUserInventory;
        public bool checkUserGiftList;
        public bool checkUserWebCash;
        public bool checkUserConfigsSave;

        public IPAddress ipAddress;
        public short Port;
        public PhysicalAddress macAddress;
        public byte[] localIP = new byte[4];
        public string hardwareId;

        public CupomEffects effects;
        public PlayerSession session;
        public AccessLevelEnum access;

        public PlayerEvent events = new PlayerEvent();
        public PlayerBonus bonus = new PlayerBonus();
        public PlayerConfig configs = new PlayerConfig();
        public PlayerEquipedItems equipments = new PlayerEquipedItems();
        public PlayerInventory inventory = new PlayerInventory();
        public PlayerMissions missions = new PlayerMissions();
        public PlayerStats statistics = new PlayerStats();
        public FriendSystem friends = new FriendSystem();
        public PlayerTitles titles = new PlayerTitles();
        public AccountStatus status = new AccountStatus();
        public List<Account> clanPlayers = new List<Account>();

        public DateTime lastSlotChange;
        public DateTime lastChannelList;
        public DateTime lastLobbyEnter; //Por enquanto esta sem uso
        public DateTime lastTimerSync;
        public DateTime lastChatting;
        public DateTime lastFriendInvite; //Enviar convite de amizade
        public DateTime lastFriendDelete; //Deletar um amigo
        public DateTime lastFriendInviteRoom; //Convidar amigo para uma sala
        public DateTime lastClanInvite;
        public DateTime lastCreateRoom;
        public DateTime lastRoomInvitePlayers;
        public DateTime lastRoomGetLobbyPlayers;
        public DateTime lastRoomList;
        public DateTime lastSaveConfigs;
        public DateTime lastReadyBattle;
        public DateTime lastFindUser;
        public DateTime lastProfileEnter;
        public DateTime lastProfileLeave;
        public DateTime lastShopEnter;
        public DateTime lastShopLeave;
        public DateTime lastInventoryEnter;
        public DateTime lastInventoryLeave;

        public Room room;
        public Match match;
        public Client client;
        public Account()
        {
            DateTime now = new DateTime();
            lastSlotChange = now;
            lastChannelList = now;
            lastLobbyEnter = now;
            lastTimerSync = now;
            lastChatting = now;
            lastFriendInvite = now;
            lastFriendDelete = now;
            lastFriendInviteRoom = now;
            lastClanInvite = now;
            lastCreateRoom = now;
            lastRoomInvitePlayers = now;
            lastRoomGetLobbyPlayers = now;
            lastRoomList = now;
            lastSaveConfigs = now;
            lastReadyBattle = now;
            lastFindUser = now;
            lastProfileLeave = now;
            lastShopEnter = now;
            lastShopLeave = now;
            lastInventoryEnter = now;
            lastInventoryLeave = now;
        }

        public void GameClear()
        {
            titles = new PlayerTitles();
            inventory = new PlayerInventory();
            status = new AccountStatus();
            missions = new PlayerMissions();
            equipments = new PlayerEquipedItems();
            clanPlayers = new List<Account>();

            friends.CleanList();

            configs = null;
            bonus = null;
            events = null;
            session = null;
            match = null;
            room = null;
            client = null;
        }

        public string GetDate()
        {
            return DateTime.Now.ToString("yyMMddHHmm");
        }

        public void SetOnlineStatus(bool online)
        {
            if (isOnline != online && ExecuteQuery($"UPDATE accounts SET online='{online}' WHERE id='{playerId}'"))
            {
                isOnline = online;
            }
        }

        public void SetPublicIP(IPAddress address)
        {
            if (address == null)
            {
                ipAddress = new IPAddress(new byte[4]);
            }
            ipAddress = address;
        }

        public Channel GetChannel() => ServersManager.GetChannel(channelId);
        public int GetRank() => bonus == null || bonus.fakeRank == 55 ? rankId : bonus.fakeRank;
        public int GetSessionId() => session != null ? session.sessionId : 0;
        public bool UseChatGM() => !hideGMcolor && (rankId == 53 || rankId == 54);
        public bool IsGM() => rankId == 53 || rankId == 54 || HaveGMLevel();
        public bool HaveGMLevel() => access > AccessLevelEnum.Moderator;
        public bool HaveAcessLevel() => access > AccessLevelEnum.Normal;
        public bool ComparePassword(string passsword_crypted) => passsword_crypted == password;
        public void ResetPages()
        {
            lastRoomPage = 0;
            lastPlayerPage = 0;
        }

        public bool GetChannel(out Channel channel)
        {
            channel = ServersManager.GetChannel(channelId);
            return channel != null;
        }

        public void UpdateCacheInfo()
        {
            if (playerId == 0)
            {
                return;
            }
            lock (AccountManager.accounts)
            {
                AccountManager.accounts[playerId] = this;
            }
        }

        public void Close(int time = 0, bool kicked = false)
        {
            if (client != null)
            {
                client.Close(time, kicked);
            }
        }

        public void SendPacket(GamePacketWriter sp)
        {
            if (client != null)
            {
                client.SendPacket(sp);
            }
        }

        public void SendCompletePacket(byte[] data)
        {
            if (client != null)
            {
                client.SendCompletePacket(data);
            }
        }

        public void SendPacketForPlayers(GamePacketWriter packet, List<Account> players)
        {
            if (players.Count == 0)
            {
                return;
            }

            byte[] code = packet.GetCompleteBytes("Account.SendPacket1");
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(code);
            }
        }

        public void SendPacketForPlayers(GamePacketWriter packet, List<Account> players, long exception)
        {
            if (players.Count == 0)
            {
                return;
            }

            byte[] code = packet.GetCompleteBytes("Account.SendPacket2");
            for (int i = 0; i < players.Count; i++)
            {
                Account member = players[i];
                if (member.playerId != exception)
                {
                    member.SendCompletePacket(code);
                }
            }
        }

        public void CheckRoomList()
        {
            new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        Channel channel = channelId >= 0 ? GetChannel() : null;
                        if (firstLobbyEnter && channel != null && channel.players.Contains(session) && room == null && (DateTime.Now - lastRoomList).Seconds > 5)
                        {
                            Logger.Attacks($" [Account] Connection destroyed due to no response from the room list for 2 seconds. (CheckRoomList). PlayerId: {playerId}");
                            Close();
                            break;
                        }
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }).Start();
        }

        public void LoadInventory()
        {
            lock (inventory.items)
            {
                inventory.items.AddRange(GetInventoryItems());
            }
        }

        public void LoadMissionList()
        {
            PlayerMissions playerMissions = GetMission();
            if (playerMissions != null)
            {
                missions = playerMissions;
            }
            else if (!InsertMission())
            {
                Logger.Warning(" [Account] Falha ao inserir o player missions na database.");
            }
        }
        public void LoadPlayerBonus()
        {
            PlayerBonus playerBonus = GetPlayerBonusDB();
            if (playerBonus != null)
            {
                bonus = playerBonus;
            }
            else if (!InsertBonus())
            {
                Logger.Warning(" [Account] Falha ao inserir o player bonus na database.");
            }
        }
        public void LoadPlayerConfigs()
        {
            PlayerConfig playerConfigs = GetConfigs();
            if (playerConfigs != null)
            {
                configs = playerConfigs;
            }
            else if (!InsertConfig())
            {
                Logger.Warning(" [Account] Falha ao inserir o player configs na database.");
            }
        }
        public void LoadPlayerTitles()
        {
            PlayerTitles playerTitles = GetTitles();
            if (playerTitles != null)
            {
                titles = playerTitles;
            }
            else if (!InsertTitle())
            {
                Logger.Warning(" [Account] Falha ao inserir o player titles na database.");
            }
        }
        public void LoadPlayerEvents()
        {
            PlayerEvent playerEvents = GetEvents();
            if (playerEvents != null)
            {
                events = playerEvents;
            }
            else if (!InsertEvent())
            {
                Logger.Warning(" [Account] Falha ao inserir o player events na database.");
            }
        }

        public bool LoadPlayerFriends(bool loadFull)
        {
            List<Friend> friendsList = GetFriendList();
            if (friendsList.Count > 0)
            {
                friends.friendsCache = friendsList;
                if (loadFull)
                {
                    return GetFriendlyAccounts();
                }
            }
            return false;
        }

        public void LoadData(int LoadType)
        {
            if (LoadType > 0)
            {
                if ((LoadType & 1) == 1)
                {
                    LoadPlayerTitles();
                }
                if ((LoadType & 2) == 2)
                {
                    LoadPlayerBonus();
                }
                if ((LoadType & 4) == 4)
                {
                    LoadPlayerFriends(true);
                }
                if ((LoadType & 8) == 8)
                {
                    LoadPlayerEvents();
                }
                if ((LoadType & 16) == 16)
                {
                    LoadPlayerConfigs();
                }
                if ((LoadType & 32) == 32)
                {
                    LoadPlayerFriends(false);
                }
            }
        }

        public void SetCuponsFlags()
        {
            lock (inventory.items)
            {
                effects = 0;
                bool IsRoule = room != null ? TournamentRulesManager.CheckRoomRule(room.roomName.ToUpper()) : false;
                for (int i = 0; i < inventory.items.Count; i++)
                {
                    ItemsModel item = inventory.items[i];
                    if (item.category == 3 && item.equip == 2)
                    {
                        CupomFlag cupom = CupomEffectManager.GetCupomEffect(item.id);
                        if (cupom != null && cupom.EffectFlag > 0 && !effects.HasFlag(cupom.EffectFlag) && (!IsRoule || IsRoule && !TournamentRulesManager.CuponsEffectsBlocked.Contains(item.id)))
                        {
                            effects |= cupom.EffectFlag;
                        }
                    }
                }
            }
        }

        public void DiscountPlayerItems()
        {
            int data_atual = int.Parse(GetDate());
            List<object> removedItems = new List<object>();
            int bonuses = bonus != null ? bonus.bonuses : 0;
            int freepass = bonus != null ? bonus.freepass : 0;
            lock (inventory.items)
            {
                for (int i = 0; i < inventory.items.Count; i++)
                {
                    ItemsModel item = inventory.items[i];
                    if (item.count <= data_atual & item.equip == 2)
                    {
                        if (item.category == 3)
                        {
                            if (bonus == null)
                            {
                                continue;
                            }
                            bool changed = bonus.RemoveBonuses(item.id);
                            if (!changed)
                            {
                                if (item.id == 1200014000)
                                {
                                    ExecuteQuery($"UPDATE player_bonus SET sightcolor='4' WHERE player_id='{playerId}'");
                                    bonus.sightColor = 4;
                                }
                                else if (item.id == 1200006000)
                                {
                                    ExecuteQuery($"UPDATE accounts SET nickcolor='0' WHERE id='{playerId}'");
                                    nickcolor = 0;
                                }
                                else if (item.id == 1200009000)
                                {
                                    ExecuteQuery($"UPDATE player_bonus SET fakerank='55' WHERE player_id='{playerId}'");
                                    bonus.fakeRank = 55;
                                }
                                else if (item.id == 1200010000)
                                {
                                    if (bonus.fakeNick.Length > 0)
                                    {
                                        ExecuteQuery($"UPDATE player_bonus SET fakenick='{""}' WHERE player_id='{playerId}'");
                                        ExecuteQuery($"UPDATE accounts SET nickname='{bonus.fakeNick}' WHERE id='{playerId}'");
                                        nickname = bonus.fakeNick;
                                        bonus.fakeNick = "";
                                    }
                                }
                            }
                        }
                        removedItems.Add(item.objectId);
                        inventory.items.RemoveAt(i--);
                    }
                    else if (item.count == 0)
                    {
                        removedItems.Add(item.objectId);
                        inventory.items.RemoveAt(i--);
                    }
                }
                Utilities.DeleteDB("player_items", "object_id", removedItems.ToArray(), "owner_id", playerId);
            }
            if (bonus != null && (bonus.bonuses != bonuses || bonus.freepass != freepass))
            {
                UpdatePlayerBonus();
            }
            inventory.LoadBasicItems();
            int type = CheckEquipedItems(equipments);
            if (type > 0)
            {
                if (type == 1) //Atualiza Characters
                {
                    ExecuteQuery($"UPDATE accounts SET character_red='{equipments.red}', character_blue='{equipments.blue}', character_helmet='{equipments.helmet}', character_beret='{equipments.beret}', character_dino='{equipments.dino}' WHERE id='{playerId}'");
                }
                else if (type == 2) //Atualiza Weapons
                {
                    ExecuteQuery($"UPDATE accounts SET equipment_primary='{equipments.primary}', equipment_secondary='{equipments.secondary}', equipment_melee='{equipments.melee}', equipment_grenade='{equipments.grenade}', equipment_special='{equipments.special}' WHERE id='{playerId}'");
                }
                else if (type == 3) //Atualiza Weapons e Characters
                {
                    ExecuteQuery($"UPDATE accounts SET equipment_primary='{equipments.primary}', equipment_secondary='{equipments.secondary}', equipment_melee='{equipments.melee}', equipment_grenade='{equipments.grenade}', equipment_special='{equipments.special}', character_red='{equipments.red}', character_blue='{equipments.blue}', character_helmet='{equipments.helmet}', character_beret='{equipments.beret}', character_dino='{equipments.dino}' WHERE id='{playerId}'");
                }
            }
        }

        public void DiscountPlayerItems(Slot slot)
        {
            int data_atual = int.Parse(GetDate());
            bool loadCode = false;
            List<ItemsModel> updateList = new List<ItemsModel>();
            List<object> removeList = new List<object>();
            int bonuses = bonus != null ? bonus.bonuses : 0;
            int freepass = bonus != null ? bonus.freepass : 0;
            lock (inventory.items)
            {
                for (int i = 0; i < inventory.items.Count; i++)
                {
                    ItemsModel item = inventory.items[i];
                    if (item.equip == 1 && slot.EquipmentsUsed.Contains(item.id) && !slot.specGM)
                    {
                        if (item.count-- < 1)
                        {
                            removeList.Add(item.objectId);
                            inventory.items.RemoveAt(i--);
                        }
                        else
                        {
                            updateList.Add(item);
                            ExecuteQuery($"UPDATE player_items SET count='{item.count}' WHERE object_id='{item.objectId}' AND owner_id='{playerId}'");
                        }
                    }
                    else if (item.count <= data_atual & item.equip == 2)
                    {
                        if (item.category == 3 && Utilities.GetIdStatics(item.id, 1) == 12)
                        {
                            if (bonus == null)
                            {
                                continue;
                            }
                            bool changed = bonus.RemoveBonuses(item.id);
                            if (!changed)
                            {
                                if (item.id == 1200014000)
                                {
                                    ExecuteQuery($"UPDATE player_bonus SET sightcolor='4' WHERE player_id='{playerId}'");
                                    bonus.sightColor = 4;
                                    loadCode = true;
                                }
                                else if (item.id == 1200009000)
                                {
                                    ExecuteQuery($"UPDATE player_bonus SET fakerank='55' WHERE player_id='{playerId}'");
                                    bonus.fakeRank = 55;
                                    loadCode = true;
                                }
                            }
                            else if (item.id == 1200006000)
                            {
                                ExecuteQuery($"UPDATE accounts SET nickcolor='0' WHERE id='{playerId}'");
                                nickcolor = 0;
                                if (room != null)
                                {
                                    using (PROTOCOL_ROOM_GET_NICKNAME_ACK packet = new PROTOCOL_ROOM_GET_NICKNAME_ACK(slot.Id, nickname, nickcolor))
                                    {
                                        room.SendPacketToPlayers(packet);
                                    }
                                }
                            }
                        }
                        removeList.Add(item.objectId);
                        inventory.items.RemoveAt(i--);
                    }
                }
            }
            if (bonus != null && (bonus.bonuses != bonuses || bonus.freepass != freepass))
            {
                UpdatePlayerBonus();
            }
            if (updateList.Count > 0)
            {
                SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(2, this, updateList));
            }
            for (int i = 0; i < removeList.Count; i++)
            {
                long objId = (long)removeList[i];
                SendPacket(new INVENTORY_ITEM_EXCLUDE_PAK(1, objId));
            }
            Utilities.DeleteDB("player_items", "object_id", removeList.ToArray(), "owner_id", playerId);
            if (loadCode)
            {
                SendPacket(new PROTOCOL_BASE_USER_EFFECTS_ACK(0, bonus));
            }
            int type = CheckEquipedItems(equipments);
            if (type > 0)
            {
                SendPacket(new INVENTORY_EQUIPED_ITEMS_PAK(this, type));
                slot.equipment = equipments;
            }
        }

        public bool GetFriendlyAccounts()
        {
            if (friends.friendsCache.Count == 0)
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
                    for (int idx = 0; idx < friends.friendsCache.Count; idx++)
                    {
                        Friend friend = friends.friendsCache[idx];
                        string param = "@valor" + idx;
                        command.Parameters.AddWithValue(param, friend.playerId);
                        parameters.Add(param);
                    }
                    loaded = string.Join(",", parameters.ToArray());
                    command.CommandText = "SELECT id,nickname,rank,online,status FROM accounts WHERE id in (" + loaded + ") ORDER BY id";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            Friend friend = friends.GetFriend(data.GetInt64(0));
                            if (friend != null)
                            {
                                friend.player.playerNickname = data.GetString(1);
                                friend.player.rank = (byte)data.GetInt32(2);
                                friend.player.isOnline = data.GetBoolean(3);
                                friend.player.status.SetData((uint)data.GetInt64(4), friend.playerId);
                            }
                        }
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
            return false;
        }

        public void UpdateCommunity(bool isConnect)
        {
            if (GetFriendlyAccounts())
            {
                for (int i = 0; i < friends.friendsCache.Count; i++)
                {
                    Friend friend = friends.friendsCache[i];
                    PlayerInfo info = friend != null ? friend.player : null;
                    if (info != null)
                    {
                        Account friendAcc = AccountManager.GetAccount(friend.playerId, true);
                        if (friendAcc != null)
                        {
                            Friend friendPlayer = friendAcc.friends.GetFriend(playerId, out int idx);
                            if (idx != -1 && friendPlayer != null && friendPlayer.state == 0)
                            {
                                friendAcc.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeStateEnum.Update, friendPlayer, isConnect ? FriendStateEnum.Online : FriendStateEnum.Offline, idx));
                            }
                        }
                    }
                }
            }
            if (clanId > 0)
            {
                for (int i = 0; i < clanPlayers.Count; i++)
                {
                    Account member = clanPlayers[i];
                    if (member != null && member.isOnline && member.status.serverId == Settings.ServerId)
                    {
                        member.SendPacket(new CLAN_MEMBER_INFO_CHANGE_PAK(this, isConnect ? FriendStateEnum.Online : FriendStateEnum.Offline));
                    }
                }
            }
        }

        #region MESSAGES MANAGER

        /// <summary>
        /// Cria uma mensagem na Database, e após a criação é adicionado o número do objeto ao modelo.
        /// </summary>
        /// <param name="message">Mensagem</param>
        /// <returns></returns>
        public bool InsertMessage(Message message)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@owner_id", playerId);
                    command.Parameters.AddWithValue("@sender_id", message.senderId);
                    command.Parameters.AddWithValue("@clan_id", message.clanId);
                    command.Parameters.AddWithValue("@sender_name", message.senderName);
                    command.Parameters.AddWithValue("@text", message.text);
                    command.Parameters.AddWithValue("@type", message.type);
                    command.Parameters.AddWithValue("@state", message.state);
                    command.Parameters.AddWithValue("@expire", message.expireDate);
                    command.Parameters.AddWithValue("@cb", (int)message.noteEnum);
                    command.CommandText = $"INSERT INTO player_messages(owner_id, sender_id, clan_id, sender_name, text, type, state, expire, cb)VALUES(@owner_id, @sender_id, @clan_id, @sender_name, @text, @type, @state, @expire, @cb) RETURNING object_id";
                    message.objectId = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
                return message.objectId > 0;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return false;
        }

        public Message GetMessage(int objectId)
        {
            if (objectId == 0)
            {
                return null;
            }
            Message message = null;
            try
            {
                DateTime today = DateTime.Now;
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.Parameters.AddWithValue("@objectId", objectId);
                    command.CommandText = $"SELECT * FROM player_messages WHERE object_id=@objectId AND owner_id=@ownerId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            message = new Message(data.GetInt32(8), today)
                            {
                                objectId = objectId,
                                senderId = data.GetInt64(2),
                                clanId = data.GetInt32(3),
                                senderName = data.GetString(4),
                                text = data.GetString(5),
                                type = (byte)data.GetInt32(6),
                                state = data.GetInt32(7),
                                noteEnum = (NoteMessageClanEnum)data.GetInt32(9)
                            };
                        }
                        data.Close();
                        connection.Close();
                    }
                }
                return message;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return null;
        }

        public List<Message> GetMessages()
        {
            List<Message> messages = new List<Message>();
            try
            {
                DateTime today = DateTime.Now;
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.CommandText = $"SELECT * FROM player_messages WHERE owner_id=@ownerId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            byte type = (byte)data.GetInt32(6);
                            if (type == 2)
                            {
                                continue;
                            }
                            Message message = new Message(data.GetInt32(8), today)
                            {
                                objectId = data.GetInt32(0),
                                senderId = data.GetInt64(2),
                                clanId = data.GetInt32(3),
                                senderName = data.GetString(4),
                                text = data.GetString(5),
                                type = type,
                                state = data.GetInt32(7),
                                noteEnum = (NoteMessageClanEnum)data.GetInt32(9),
                            };
                            messages.Add(message);
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
            return messages;
        }

        public int GetMessagesCount()
        {
            int messages = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.CommandText = $"SELECT COUNT(*) FROM player_messages WHERE owner_id=@ownerId";
                    messages = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return messages;
        }

        public bool DeleteMessages(List<object> objects)
        {
            if (objects.Count == 0)
            {
                return false;
            }
            return Utilities.DeleteDB("player_messages", "object_id", objects.ToArray(), "owner_id", playerId);
        }

        public bool DeleteMessage(int objectId)
        {
            return ExecuteQuery($"DELETE FROM player_messages WHERE object_id='{objectId}' AND owner_id='{playerId}'");
        }

        public void RecicleMessages(List<Message> messages)
        {
            List<object> objects = new List<object>();
            for (int i = 0; i < messages.Count; i++)
            {
                Message message = messages[i];
                if (message.DaysRemaining == 0)
                {
                    objects.Add(message.objectId);
                    messages.RemoveAt(i--);
                }
            }
            DeleteMessages(objects);
        }

        public List<Message> GetGifts()
        {
            List<Message> gifts = new List<Message>();
            try
            {
                DateTime today = DateTime.Now;
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.CommandText = $"SELECT * FROM player_messages WHERE owner_id=@ownerId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            byte type = (byte)data.GetInt32(6);
                            if (type != 2)
                            {
                                continue;
                            }
                            Message message = new Message(data.GetInt32(8), today)
                            {
                                objectId = data.GetInt32(0),
                                senderId = data.GetInt64(2),
                                clanId = data.GetInt32(3),
                                senderName = data.GetString(4),
                                text = data.GetString(5),
                                type = type,
                                state = data.GetInt32(7),
                                noteEnum = (NoteMessageClanEnum)data.GetInt32(9),
                            };
                            gifts.Add(message);
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
            return gifts;
        }
        #endregion

        #region MISSIONS MANAGER

        public bool InsertMission()
        {
            return ExecuteQuery($"INSERT INTO player_missions (owner_id) VALUES ('{playerId}')");
        }

        public PlayerMissions GetMission()
        {
            PlayerMissions mission = null;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.CommandText = $"SELECT * FROM player_missions WHERE owner_id=@ownerId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            mission = new PlayerMissions
                            {
                                actualMission = (byte)data.GetInt32(1),
                                card1 = (byte)data.GetInt32(2),
                                card2 = (byte)data.GetInt32(3),
                                card3 = (byte)data.GetInt32(4),
                                card4 = (byte)data.GetInt32(5),
                                mission1 = (byte)data.GetInt32(6),
                                mission2 = (byte)data.GetInt32(7),
                                mission3 = (byte)data.GetInt32(8),
                                mission4 = (byte)(events != null && events.LastQuestFinish == 0 && EventQuestSyncer.GetRunningEvent() != null ? 13 : 0),
                            };
                            data.GetBytes(9, 0, mission.list1, 0, 40);
                            data.GetBytes(10, 0, mission.list2, 0, 40);
                            data.GetBytes(11, 0, mission.list3, 0, 40);
                            data.GetBytes(12, 0, mission.list4, 0, 40);
                            mission.UpdateSelectedCard();
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
            return mission;
        }
        public bool UpdateMissionContent()
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.Parameters.AddWithValue("@array", missions.GetCurrentMissionList());
                    command.CommandText = $"UPDATE player_missions SET mission{missions.actualMission + 1}_content=@array WHERE owner_id=@ownerId";
                    success = command.ExecuteNonQuery();
                    connection.Close();
                    if (success > 1)
                    {
                        Logger.Error($" [Account] (UpdateMissionContent) Success: {success}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }
        public bool UpdateCardDelete(int missionIdx)
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.Parameters.AddWithValue("@array", new byte[0]);
                    command.CommandText = $"UPDATE player_missions SET card{missionIdx}='0', mission{missionIdx}_content=@array WHERE owner_id=@ownerId";
                    success = command.ExecuteNonQuery();
                    connection.Close();
                    if (success > 1)
                    {
                        Logger.Error($" [Account] (UpdateCardDelete) Success: {success}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }
        #endregion

        #region TITLES MANAGER

        public bool InsertTitle()
        {
            return ExecuteQuery($"INSERT INTO player_titles (owner_id) VALUES ('{playerId}')");
        }

        public PlayerTitles GetTitles()
        {
            PlayerTitles title = null;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.CommandText = $"SELECT * FROM player_titles WHERE owner_id=@ownerId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            title = new PlayerTitles();
                            title.Equiped1 = (byte)data.GetInt32(1);
                            title.Equiped2 = (byte)data.GetInt32(2);
                            title.Equiped3 = (byte)data.GetInt32(3);
                            title.Flags = data.GetInt64(4);
                            title.Slots = (byte)data.GetInt32(5);
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
            return title;
        }

        public bool UpdateEquipedTitle(int index, int titleId)
        {
            return ExecuteQuery($"UPDATE player_titles SET titleequiped{index + 1}='{titleId}' WHERE owner_id='{playerId}'");
        }

        public bool UpdateTitlesFlags(long flags)
        {
           return ExecuteQuery($"UPDATE player_titles SET titleflags='{flags}' WHERE owner_id='{playerId}'");
        }

        public bool UpdateTitleSlots(int slots)
        {
            return ExecuteQuery($"UPDATE player_titles SET titleslots='{slots}' WHERE owner_id='{playerId}'");
        }

        public bool UpdateTitleRequirements(int brooch, int insignia, int medal, int blueorder)
        {
            if (brooch < 0 || insignia < 0 || medal < 0 || blueorder < 0)
            {
                return false;
            }
            return ExecuteQuery($"UPDATE accounts SET broochs='{brooch}', insignias='{insignia}', medals='{medal}', blueorders='{blueorder}' WHERE id='{playerId}'");
        }

        #endregion

        #region UTILS

        public List<ItemsModel> GetInventoryItems()
        {
            List<ItemsModel> items = new List<ItemsModel>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.CommandText = $"SELECT * FROM player_items WHERE owner_id=@ownerId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            items.Add(new ItemsModel(data.GetInt32(2), data.GetInt32(5), data.GetString(3), (byte)data.GetInt32(6), data.GetInt32(4), data.GetInt64(0)));
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
            return items;
        }

        public PlayerEvent GetEvents()
        {
            PlayerEvent events = null;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.CommandText = $"SELECT * FROM player_events WHERE player_id=@ownerId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            events = new PlayerEvent
                            {
                                LastVisitEventId = data.GetInt32(1),
                                LastVisitSequence1 = (byte)data.GetInt32(2),
                                LastVisitSequence2 = (byte)data.GetInt32(3),
                                NextVisitDate = data.GetInt32(4),
                                LastXmasRewardDate = data.GetInt32(5),
                                LastPlaytimeDate = data.GetInt32(6),
                                LastPlaytimeValue = data.GetInt64(7),
                                LastPlaytimeFinish = data.GetInt32(8),
                                LastLoginDate = data.GetInt32(9),
                                LastQuestDate = data.GetInt32(10),
                                LastQuestFinish = data.GetInt32(11),
                                LastDailyCashDate = data.GetInt32(12),
                                LastDailyCashValue = data.GetInt64(13),
                            };
                        }
                        data.Close();
                        connection.Close();
                        if (events != null && (int.Parse(DateTime.Now.ToString("yyMMdd")) - events.LastDailyCashDate) >= 1)
                        {
                            events.LastDailyCashValue = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return events;
        }

        public PlayerConfig GetConfigs()
        {
            PlayerConfig config = null;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.CommandText = $"SELECT * FROM player_configs WHERE owner_id=@ownerId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            config = new PlayerConfig
                            {
                                config = data.GetInt32(1),
                                blood = (short)data.GetInt32(2),
                                sight = (byte)data.GetInt32(3),
                                hand = (byte)data.GetInt32(4),
                                audio = (byte)data.GetInt32(5),
                                music = (byte)data.GetInt32(6),
                                audioEnable = data.GetInt32(7),
                                sensibilidade = (byte)data.GetInt32(8),
                                fov = data.GetInt32(9),
                                invertedMouse = (byte)data.GetInt32(10),
                                messageInvitation = (byte)data.GetInt32(11),
                                chatPrivate = (byte)data.GetInt32(12),
                                macros = data.GetInt32(13),
                                macro_1 = data.GetString(14),
                                macro_2 = data.GetString(15),
                                macro_3 = data.GetString(16),
                                macro_4 = data.GetString(17),
                                macro_5 = data.GetString(18)
                            };
                            data.GetBytes(19, 0, config.keys, 0, 215);
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
            return config;
        }

        public List<Friend> GetFriendList()
        {
            List<Friend> friends = new List<Friend>();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.CommandText = $"SELECT * FROM friends WHERE owner_id=@ownerId ORDER BY friend_id";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            friends.Add(new Friend(data.GetInt64(1))
                            {
                                state = data.GetInt32(2),
                                removed = data.GetBoolean(3)
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
            return friends;
        }

        public PlayerBonus GetPlayerBonusDB()
        {
            PlayerBonus bonus = null;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.CommandText = $"SELECT * FROM player_bonus WHERE player_id=@ownerId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            bonus = new PlayerBonus
                            {
                                bonuses = data.GetInt32(1),
                                sightColor = (short)data.GetInt32(2),
                                freepass = data.GetInt32(3),
                                fakeRank = data.GetInt32(4),
                                fakeNick = data.GetString(5)
                            };
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
            return bonus;
        }
        public bool UpdatePlayTimeReward()
        {
            return ExecuteQuery($"UPDATE player_events SET last_playtime_finish='2' WHERE player_id='{playerId}'");
        }
        public bool UpdatePlayerBonus()
        {
            return ExecuteQuery($"UPDATE player_bonus SET bonuses='{bonus.bonuses}', freepass='{bonus.freepass}' WHERE player_id='{playerId}'");
        }
        public bool UpdateTotalFights()
        {
            return ExecuteQuery($"UPDATE accounts SET fights='0', fights_wins='0', fights_lost='0', fights_draw='0', all_fights='{statistics.totalfights}' WHERE id='{playerId}'");
        }

        public bool UpdateAccountCashing(int goldValue, int cashValue)
        {
            if (goldValue < 0 || cashValue < 0)
            {
                return false;
            }
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    string cmd = "";
                    if (goldValue >= 0)
                    {
                        cmd += $"gold='{goldValue}'";
                    }
                    if (cashValue >= 0)
                    {
                        cmd += (cmd != "" ? ", " : "") + $"cash='{cashValue}'";
                    }
                    command.CommandText = $"UPDATE accounts SET {cmd} WHERE id='{playerId}'";
                    success = command.ExecuteNonQuery();
                    connection.Close();
                    if (success > 1)
                    {
                        Logger.Error($" [Account] (UpdateAccountCashing) Success: {success}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }
        public bool UpdatePccafe(byte pccafeValue, int pccafeDateValue, int goldValue, int cashValue)
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.Parameters.AddWithValue("@pccafeValue", pccafeValue);
                    command.Parameters.AddWithValue("@pccafeDateValue", pccafeDateValue);
                    command.Parameters.AddWithValue("@goldValue", goldValue);
                    command.Parameters.AddWithValue("@cashValue", cashValue);
                    if (goldValue == 0 || cashValue == 0)
                    {
                        command.CommandText = $"UPDATE accounts SET pccafe=@pccafeValue, pccafe_date=@pccafeDateValue WHERE id=@ownerId";
                    }
                    else
                    {
                        command.CommandText = $"UPDATE accounts SET pccafe=@pccafeValue, pccafe_date=@pccafeDateValue, gold=@goldValue, cash=@cashValue WHERE id=@ownerId";
                    }
                    success = command.ExecuteNonQuery();
                    connection.Close();
                    if (success > 1)
                    {
                        Logger.Error($" [Account] (UpdatePccafe) Success: {success}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }

        public bool UpdateAccountGold(int goldValue)
        {
            if (goldValue < 0)
            {
                return false;
            }
            return ExecuteQuery($"UPDATE accounts SET gold='{goldValue}' WHERE id='{playerId}'");
        }
        public bool UpdateKDReset()
        {
            return ExecuteQuery($"UPDATE accounts SET kills='0', deaths='0', headshots='{statistics.headshots}', all_kills='{statistics.totalkills}' WHERE id='{playerId}'");
        }
        public bool UpdateMissionId(int value, int index)
        {
            return ExecuteQuery($"UPDATE player_missions SET mission_id{index + 1}='{value}' WHERE owner_id='{playerId}'");
        }
        public bool UpdateFriendState(Friend friend)
        {
            return ExecuteQuery($"UPDATE friends SET state='{friend.state}' WHERE owner_id='{playerId}' AND friend_id='{friend.playerId}'");
        }
        public bool UpdateFriendRemovedAndState(Friend friend)
        {
           return ExecuteQuery($"UPDATE friends SET removed='{friend.removed}', state='{friend.state}' WHERE owner_id='{playerId}' AND friend_id='{friend.playerId}'");
        }

        #endregion

        #region DELETES DB

        public bool DeleteItem(long objectId)
        {
            if (objectId == 0)
            {
                return false;
            }
            return ExecuteQuery($"DELETE FROM player_items WHERE object_id='{objectId}' AND owner_id='{playerId}'");
        }
        public bool DeleteFriend(long friendId)
        {
            return ExecuteQuery($"DELETE FROM friends WHERE friend_id='{friendId}' AND owner_id='{playerId}'");
        }
        public bool DeleteInvite(int clanId)
        {
            return ExecuteQuery($"DELETE FROM clan_invites WHERE clan_id='{clanId}' AND player_id='{playerId}'");
        }
        public bool DeleteClanInvite()
        {
            return ExecuteQuery($"DELETE FROM clan_invites WHERE player_id='{playerId}'");
        }

        #endregion

        #region UPDATES COMMANDS CHAT

        public bool UpdateAccountCash(int cashValue)
        {
            if (cashValue < 0)
            {
                return false;
            }
            return ExecuteQuery($"UPDATE accounts SET cash='{cashValue}' WHERE id='{playerId}'");
        }

        #endregion

        #region QUERYS

        public void UpdateMacros(DBQuery query, PlayerConfig config, int type)
        {
            if ((type & 0x100) == 0x100)
            {
                query.AddQuery("macro_1", config.macro_1);
            }
            if ((type & 0x200) == 0x200)
            {
                query.AddQuery("macro_2", config.macro_2);
            }
            if ((type & 0x400) == 0x400)
            {
                query.AddQuery("macro_3", config.macro_3);
            }
            if ((type & 0x800) == 0x800)
            {
                query.AddQuery("macro_4", config.macro_4);
            }
            if ((type & 0x1000) == 0x1000)
            {
                query.AddQuery("macro_5", config.macro_5);
            }
        }
        public void UpdateConfigs(DBQuery query, PlayerConfig config)
        {
            query.AddQuery("mira", config.sight);
            query.AddQuery("audio1", config.audio);
            query.AddQuery("audio2", config.music);
            query.AddQuery("sensibilidade", config.sensibilidade);
            query.AddQuery("sangue", config.blood);
            query.AddQuery("visao", config.fov);
            query.AddQuery("mao", config.hand);
            query.AddQuery("audio_enable", config.audioEnable);
            query.AddQuery("config", config.config);
            query.AddQuery("mouse_invertido", config.invertedMouse);
            query.AddQuery("msgconvite", config.messageInvitation);
            query.AddQuery("chatsussurro", config.chatPrivate);
            query.AddQuery("macro", config.macros);
        }

        public void UpdateWeapons(PlayerEquipedItems source, PlayerEquipedItems items, DBQuery query)
        {
            if (items.primary != source.primary) query.AddQuery("equipment_primary", source.primary);
            if (items.secondary != source.secondary) query.AddQuery("equipment_secondary", source.secondary);
            if (items.melee != source.melee) query.AddQuery("equipment_melee", source.melee);
            if (items.grenade != source.grenade) query.AddQuery("equipment_grenade", source.grenade);
            if (items.special != source.special) query.AddQuery("equipment_special", source.special);
        }

        public void UpdateChars(PlayerEquipedItems source, PlayerEquipedItems items, DBQuery query)
        {
            if (items.red != source.red) query.AddQuery("character_red", source.red);
            if (items.blue != source.blue) query.AddQuery("character_blue", source.blue);
            if (items.helmet != source.helmet) query.AddQuery("character_helmet", source.helmet);
            if (items.beret != source.beret) query.AddQuery("character_beret", source.beret);
            if (items.dino != source.dino) query.AddQuery("character_dino", source.dino);
        }
        #endregion

        public bool InsertBonus()
        {
            return ExecuteQuery($"INSERT INTO player_bonus(player_id) VALUES ('{playerId}')");
        }

        public bool InsertConfig()
        {
            return ExecuteQuery($"INSERT INTO player_configs (owner_id) VALUES ('{playerId}')");
        }

        public bool InsertEvent()
        {
            return ExecuteQuery($"INSERT INTO player_events (player_id) VALUES ('{playerId}')");
        }

        /// <summary>
        /// Cria o item no Database.
        /// <para>O valor de 'objId' é definido após a criação do item.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool CreateItem(ItemsModel item)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.Parameters.AddWithValue("@itemId", item.id);
                    command.Parameters.AddWithValue("@itemName", item.name);
                    command.Parameters.AddWithValue("@count", (long)item.count);
                    command.Parameters.AddWithValue("@equip", item.equip);
                    command.Parameters.AddWithValue("@category", item.category);
                    command.CommandText = $"INSERT INTO player_items (owner_id, item_id, item_name, count, equip, category) VALUES (@ownerId, @itemId, @itemName, @count, @equip, @category) RETURNING object_id";
                    item.objectId = Convert.ToInt64(command.ExecuteScalar());
                    connection.Close();
                }
                return item.objectId > 0;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return false;
        }

        public void TryCreateItem(ItemsModel modelo)
        {
            try
            {
                ItemsModel item = inventory.GetItem(modelo.id);
                if (item == null)
                {
                    if (CreateItem(modelo))
                    {
                        inventory.AddItem(modelo);
                    }
                }
                else
                {
                    modelo.objectId = item.objectId;
                    if (item.equip == 1)
                    {
                        modelo.count += item.count;
                        ExecuteQuery($"UPDATE player_items SET count='{modelo.count}' WHERE item_id='{modelo.id}' AND owner_id='{playerId}'");
                    }
                    else if (item.equip == 2)
                    {
                        DateTime data = DateTime.ParseExact(item.count.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture);
                        if (modelo.category != 3)
                        {
                            modelo.equip = 2;
                            modelo.count = int.Parse(data.AddSeconds(modelo.count).ToString("yyMMddHHmm"));
                        }
                        else
                        {
                            TimeSpan algo = DateTime.ParseExact(modelo.count.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture) - DateTime.Now;
                            modelo.equip = 2;
                            modelo.count = int.Parse(data.AddDays(algo.TotalDays).ToString("yyMMddHHmm"));
                        }
                        ExecuteQuery($"UPDATE player_items SET count='{modelo.count}' WHERE item_id='{modelo.id}' AND owner_id='{playerId}'");
                    }
                    item.equip = modelo.equip;
                    item.count = modelo.count;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public List<ClanInvite> GetClanRequestEnlistment()
        {
            List<ClanInvite> invites = new List<ClanInvite>();
            if (clanId == 0)
            {
                return invites;
            }
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@clanId", clanId);
                    command.CommandText = $"SELECT * FROM clan_invites WHERE clan_id=@clanId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            invites.Add(new ClanInvite
                            {
                                clanId = clanId,
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

        public int GetInvitesCount(int clanId)
        {
            int count = 0;
            if (clanId == 0)
            {
                return count;
            }
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@clanId", clanId);
                    command.CommandText = $"SELECT COUNT(*) FROM clan_invites WHERE clan_id=@clanId";
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

        public int GetRequestClanId()
        {
            int result = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@playerId", playerId);
                    command.CommandText = $"SELECT clan_id FROM clan_invites WHERE player_id=@playerId";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        if (data.Read())
                        {
                            result = data.GetInt32(0);
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
            return result;
        }

        public string GetRequestText(long accountId)
        {
            if (clanId == 0 || accountId == 0)
            {
                return null;
            }
            string resultado = null;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@accountId", accountId);
                    command.Parameters.AddWithValue("@clanId", clanId);
                    command.CommandText = $"SELECT text FROM clan_invites WHERE clan_id=@clanId AND player_id=@accountId";
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
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return resultado;
        }

        public bool UpdateMessage(int[] array, int expire)
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ownerId", playerId);
                    command.Parameters.AddWithValue("@expire", expire);
                    command.Parameters.AddWithValue("@messagesArray", array);
                    command.CommandText = $"UPDATE player_messages SET expire=@expire, state='0' WHERE object_id=ANY (@messagesArray) AND owner_id=@ownerId";
                    success = command.ExecuteNonQuery();
                    connection.Close();
                    if (success > 1)
                    {
                        Logger.Error($" [Account] (UpdateMessage) Success: {success}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }

        /// <summary>
        /// Analisa se todos os itens equipados existem no inventário.
        /// Não faz atualização na Database.
        /// </summary>
        /// <param name="items">Itens equipados</param>
        /// <param name="inventory">Inventário</param>
        /// <param name="BattleRules">Aceita armas padrões (Sniper e Shotgun)</param>
        /// <returns></returns>
        public int CheckEquipedItems(PlayerEquipedItems equipedItems, bool BattleRules = false)
        {
            int type = 0;
            bool w1 = false, w2 = false, w3 = false, w4 = false, w5 = false;
            bool c1 = false, c2 = false, c3 = false, c4 = false, c5 = false;
            if (equipedItems.primary == 0)
            {
                w1 = true;
            }
            if (BattleRules)
            {
                if (!w1)
                {
                    //SSG-69 (IN MODE): 300005025  || 870MCS (IN MODE): 400006007
                    if (equipedItems.primary == 300005025 || equipedItems.primary == 400006007)
                    {
                        w1 = true;
                    }
                }
                if (!w3)
                {
                    if (equipedItems.melee == 702023001) //Barefist (IN MODE)
                    {
                        w3 = true;
                    }
                }
            }
            if (equipedItems.beret == 0)
            {
                c4 = true;
            }
            lock (inventory)
            {
                for (int i = 0; i < inventory.items.Count; i++)
                {
                    ItemsModel item = inventory.items[i];
                    if (item.count > 0)
                    {
                        if (item.id == equipedItems.primary)
                        {
                            w1 = true;
                        }
                        else if (item.id == equipedItems.secondary)
                        {
                            w2 = true;
                        }
                        else if (item.id == equipedItems.melee)
                        {
                            w3 = true;
                        }
                        else if (item.id == equipedItems.grenade)
                        {
                            w4 = true;
                        }
                        else if (item.id == equipedItems.special)
                        {
                            w5 = true;
                        }
                        else if (item.id == equipedItems.red)
                        {
                            c1 = true;
                        }
                        else if (item.id == equipedItems.blue)
                        {
                            c2 = true;
                        }
                        else if (item.id == equipedItems.helmet)
                        {
                            c3 = true;
                        }
                        else if (item.id == equipedItems.beret)
                        {
                            c4 = true;
                        }
                        else if (item.id == equipedItems.dino)
                        {
                            c5 = true;
                        }
                        if (w1 && w2 && w3 && w4 && w5 && c1 && c2 && c3 && c4 && c5)
                        {
                            break;
                        }
                    }
                }
            }
            if (!w1 || !w2 || !w3 || !w4 || !w5)
            {
                type += 2;
            }
            if (!c1 || !c2 || !c3 || !c4 || !c5)
            {
                type++;
            }
            if (!w1) equipedItems.primary = 0;
            if (!w2) equipedItems.secondary = 601002003;
            if (!w3) equipedItems.melee = 702001001;
            if (!w4) equipedItems.grenade = 803007001;
            if (!w5) equipedItems.special = 904007002;
            if (!c1) equipedItems.red = 1001001005;
            if (!c2) equipedItems.blue = 1001002006;
            if (!c3) equipedItems.helmet = 1102003001;
            if (!c4) equipedItems.beret = 0;
            if (!c5) equipedItems.dino = 1006003041;
            return type;
        }

        public void PlayTimeEvent(long playedTime, PlayTimeModel ptEvent, bool isBotMode)
        {
            if (room == null || isBotMode || events == null)
            {
                return;
            }
            long value = events.LastPlaytimeValue;
            int finish = events.LastPlaytimeFinish;
            int date = events.LastPlaytimeDate;
            if (events.LastPlaytimeDate < ptEvent.startDate)
            {
                events.LastPlaytimeFinish = 0;
                events.LastPlaytimeValue = 0;
            }
            if (events.LastPlaytimeFinish == 0)
            {
                events.LastPlaytimeValue += playedTime;
                if (events.LastPlaytimeValue >= ptEvent.time)
                {
                    events.LastPlaytimeFinish = 1;
                }
                events.LastPlaytimeDate = int.Parse(GetDate());
                if (events.LastPlaytimeValue >= ptEvent.time)
                {
                    SendPacket(new BATTLE_PLAYED_TIME_PAK(0, ptEvent));
                }
                else
                {
                    SendPacket(new BATTLE_PLAYED_TIME_PAK(1, new PlayTimeModel { time = ptEvent.time - events.LastPlaytimeValue }));
                }
            }
            else if (events.LastPlaytimeFinish == 1)
            {
                SendPacket(new BATTLE_PLAYED_TIME_PAK(0, ptEvent));
            }
            if (events.LastPlaytimeValue != value || events.LastPlaytimeFinish != finish || events.LastPlaytimeDate != date)
            {
                if (!ExecuteQuery($"UPDATE player_events SET last_playtime_value='{events.LastPlaytimeValue}', last_playtime_finish='{events.LastPlaytimeFinish}', last_playtime_date='{events.LastPlaytimeDate}' WHERE player_id='{playerId}'"))
                {
                    Logger.Warning(" [Account] (PlayTimeEvent) Não foi possivel atualizar na database os dados do evento.");
                }
            }
        }
        public void DailyCashEvent(long playedTime, bool isBotMode)
        {
            try
            {
                if (room == null || isBotMode || events == null)
                {
                    return;
                }
                long value = events.LastDailyCashValue;
                int date = events.LastDailyCashDate;
                int dateNow = int.Parse(DateTime.Now.ToString("yyMMdd"));
                if (events.LastDailyCashDate < dateNow)
                {
                    events.LastDailyCashValue += playedTime;
                    if (events.LastDailyCashValue >= 7200000) //2h = 7200000 ms
                    {
                        if (UpdateAccountCash(2000))
                        {
                            cash += 2000;
                            events.LastDailyCashDate = dateNow;
                            events.LastDailyCashValue = 0;
                            SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, gold, cash));
                            SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK($"Evento Daily Cash\nParabéns você jogou 2 horas e ganhou 2.000 de Cash!\n\nAtenção: Este evento é diário, somente no dia seguinte você poderá participar novamente."));
                        }
                    }
                    else
                    {
                        SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK($"Evento Daily Cash\nJogue mais {(7200000 - events.LastDailyCashValue) / 60000} minutos e receba 2.000 Cash."));
                    }
                }
                if (events.LastDailyCashValue != value || events.LastPlaytimeDate != date)
                {
                    if (!ExecuteQuery($"UPDATE player_events SET last_daily_cash_date='{events.LastDailyCashDate}', last_daily_cash_value='{events.LastDailyCashValue}' WHERE player_id='{playerId}'"))
                    {
                        Logger.Warning(" [Account] (DailyCashEvent) Não foi possivel atualizar na database os dados do evento.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public void SyncPlayerToClanMembers()
        {
            if (clanId < 1)
            {
                return;
            }
            List<Account> players = GetClanPlayers(playerId);
            using (CLAN_MEMBER_INFO_CHANGE_PAK packet = new CLAN_MEMBER_INFO_CHANGE_PAK(this))
            {
                SendPacketForPlayers(packet, players);
            }
        }

        public void SyncPlayerToFriends(bool all)
        {
            try
            {
                if (friends.friendsCache.Count == 0)
                {
                    return;
                }
                PlayerInfo info = new PlayerInfo(playerId, rankId, nickname, isOnline, status);
                for (int i = 0; i < friends.friendsCache.Count; i++)
                {
                    Friend friend = friends.friendsCache[i];
                    if (all || friend.state == 0 && !friend.removed)
                    {
                        Account accountFriend = AccountManager.GetAccount(friend.playerId, 32);
                        if (accountFriend != null)
                        {
                            Friend friendPlayer = accountFriend.friends.GetFriend(playerId, out int idx);
                            if (friendPlayer != null)
                            {
                                friendPlayer.player = info;
                                accountFriend.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeStateEnum.Update, friendPlayer, idx));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public void GenerateMissionAwards(DBQuery query)
        {
            int activeM = missions.actualMission;
            int missionId = missions.GetCurrentMissionId();
            int cardId = missions.GetCurrentCard();
            if (missionId <= 0 || missions.selectedCard)
            {
                return;
            }
            int CompletedLastCardCount = 0;
            int allCompletedCount = 0;
            byte[] missionL = missions.GetCurrentMissionList();
            List<Card> cards = MissionCardXML.GetCards(missionId, -1);
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                if (missionL[card.arrayIdx] >= card.missionLimit)
                {
                    allCompletedCount++;
                    if (card.cardBasicId == cardId)
                    {
                        CompletedLastCardCount++;
                    }
                }
            }
            if (allCompletedCount >= 40)
            {
                int OldBlueorder = blueorder, OldBrooch = brooch, OldMedal = medal, OldInsignia = insignia;
                CardAwards cardAwards = MissionCardXML.GetAward(missionId, cardId);
                if (cardAwards != null)
                {
                    brooch += cardAwards.brooch;
                    medal += cardAwards.medal;
                    insignia += cardAwards.insignia;
                    gold += cardAwards.gold;
                    exp += cardAwards.exp;
                }
                MisAwards missionAwards = MissionAwards.GetAward(missionId);
                if (missionAwards != null)
                {
                    blueorder += missionAwards.blueOrder;
                    exp += missionAwards.exp;
                    gold += missionAwards.gold;
                }
                List<ItemsModel> items = MissionCardXML.GetMissionAwards(missionId);
                if (items.Count > 0)
                {
                    SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, this, items));
                }
                SendPacket(new BASE_QUEST_ALERT_PAK(273, 4, this));
                if (brooch != OldBrooch)
                {
                    query.AddQuery("broochs", brooch);
                }
                if (insignia != OldInsignia)
                {
                    query.AddQuery("insignias", insignia);
                }
                if (medal != OldMedal)
                {
                    query.AddQuery("medals", medal);
                }
                if (blueorder != OldBlueorder)
                {
                    query.AddQuery("blueorders", blueorder);
                }
                UpdateActualMission(activeM + 1);

                if (activeM == 0)
                {
                    missions.mission1 = 0;
                    missions.card1 = 0;
                    missions.list1 = new byte[40];
                }
                else if (activeM == 1)
                {
                    missions.mission2 = 0;
                    missions.card2 = 0;
                    missions.list2 = new byte[40];
                }
                else if (activeM == 2)
                {
                    missions.mission3 = 0;
                    missions.card3 = 0;
                    missions.list3 = new byte[40];
                }
                else if (activeM == 3)
                {
                    missions.mission4 = 0;
                    missions.card4 = 0;
                    missions.list4 = new byte[40];
                    if (events != null)
                    {
                        events.LastQuestFinish = 1;
                        ExecuteQuery($"UPDATE player_events SET last_quest_finish='1' WHERE player_id='{playerId}'");
                    }
                }
            }
            else if (CompletedLastCardCount == 4 && !missions.selectedCard)
            {
                CardAwards cardAwards = MissionCardXML.GetAward(missionId, cardId);
                if (cardAwards != null)
                {
                    int OldBrooch = brooch, OldMedal = medal, OldInsignia = insignia;
                    brooch += cardAwards.brooch;
                    medal += cardAwards.medal;
                    insignia += cardAwards.insignia;
                    gold += cardAwards.gold;
                    exp += cardAwards.exp;
                    if (brooch != OldBrooch)
                    {
                        query.AddQuery("broochs", brooch);
                    }
                    if (insignia != OldInsignia)
                    {
                        query.AddQuery("insignias", insignia);
                    }
                    if (medal != OldMedal)
                    {
                        query.AddQuery("medals", medal);
                    }
                }
                missions.selectedCard = true;
                SendPacket(new BASE_QUEST_ALERT_PAK(1, 1, this));
            }
        }

        public byte[] GetCardFlags(int missionId, byte[] arrayList)
        {
            if (missionId == 0)
            {
                return new byte[20];
            }
            List<Card> cards = MissionCardXML.GetCards(missionId);
            if (cards.Count == 0)
            {
                return new byte[20];
            }
            using (PacketWriter writer = new PacketWriter(20))
            {
                ushort result = 0;
                for (int cardBasicId = 0; cardBasicId < 10; cardBasicId++)
                {
                    List<Card> list2 = MissionCardXML.GetCards(cards, cardBasicId);
                    for (int i = 0; i < list2.Count; i++)
                    {
                        Card card = list2[i];
                        if (arrayList[card.arrayIdx] >= card.missionLimit)
                        {
                            result |= card.flag;
                        }
                    }
                    writer.WriteH(result);
                    result = 0;
                }
                return writer.memorystream.ToArray();
            }
        }

        public int AddFriend(Account accountFriend, int state)
        {
            int success = 0;
            if (accountFriend == null)
            {
                return -1;
            }
            try
            {
                Friend friend = friends.GetFriend(accountFriend.playerId);
                if (friend == null)
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                    using (NpgsqlCommand command = connection.CreateCommand())
                    {
                        connection.Open();
                        command.Parameters.AddWithValue("@friendId", accountFriend.playerId);
                        command.Parameters.AddWithValue("@ownerId", playerId);
                        command.Parameters.AddWithValue("@state", state);
                        command.CommandText = $"INSERT INTO friends(friend_id,owner_id,state)VALUES(@friendId, @ownerId, @state)";
                        success = command.ExecuteNonQuery();
                        connection.Close();
                        if (success > 1)
                        {
                            Logger.Error($" [Account] (AddFriend) Success: {success}");
                        }
                    }
                    friend = new Friend(accountFriend.playerId, accountFriend.rankId, accountFriend.nickname, accountFriend.isOnline, accountFriend.status)
                    {
                        state = state,
                        removed = false
                    };
                    friends.AddFriend(friend);
                    return success == 1 ? 0 : -1;
                }
                else
                {
                    /*if (friend.removed)
                    {
                      
                    }*/
                    friend.state = state;
                    friend.removed = false;
                    UpdateFriendRemovedAndState(friend);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return -1;
            }
        }

        public List<Account> GetClanPlayers(long playerId)
        {
            List<Account> players = new List<Account>();
            if (clanId == 0)
            {
                return players;
            }
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@clanId", clanId);
                    command.Parameters.AddWithValue("@online", isOnline);
                    command.CommandText = $"SELECT id,nickname,nickcolor,rank,clan_authority,clan_date,status FROM accounts WHERE clan_id=@clanId AND online=@online";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            long pId = data.GetInt64(0);
                            if (pId == playerId)
                            {
                                continue;
                            }
                            Account account = new Account
                            {
                                playerId = pId,
                                nickname = data.GetString(1),
                                nickcolor = (byte)data.GetInt32(2),
                                clanId = clanId,
                                rankId = (byte)data.GetInt32(3),
                                isOnline = true,
                                clanAuthority = (ClanAuthorityEnum)data.GetInt32(4),
                                clanDate = data.GetInt32(5)
                            };
                            account.status.SetData((uint)data.GetInt64(6), account.playerId);
                            Account accountCache = AccountManager.GetAccount(account.playerId, true);
                            if (accountCache != null)
                            {
                                account.client = accountCache.client;
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

        public bool InsertInvite(ClanInvite invite)
        {
            return ExecuteQuery($"INSERT INTO clan_invites(clan_id, player_id, dateInvite, text)VALUES('{invite.clanId}','{invite.playerId}','{invite.inviteDate}','{invite.text}')");
        }

        public bool UpdateActualMission(int actualMission)
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@mission", new byte[0]);
                    command.Parameters.AddWithValue("@id", playerId);
                    command.CommandText = $"UPDATE player_missions SET card{actualMission}='0', mission_id{actualMission}='0', mission{actualMission}_content=@mission WHERE owner_id=@id";
                    success = command.ExecuteNonQuery();
                    connection.Close();
                    if (success > 1)
                    {
                        Logger.Error($" [Account] (UpdateActualMission) Success: {success}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }

        public async Task<bool> UpdateNick(string nick)
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    await connection.OpenAsync();
                    command.CommandText = $"UPDATE accounts SET nickname='{nick}' WHERE id='{playerId}'";
                    success = await command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }
        
        public async Task<bool> UpdateFakeNick()
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    await connection.OpenAsync();
                    command.CommandText = $"UPDATE player_bonus SET fakenick='{nickname}' WHERE player_id='{playerId}'";
                    success = await command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return success == 1;
        }
        public bool ExecuteQuery(string query)
        {
            int success = 0;
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = query;
                    success = command.ExecuteNonQuery();
                    connection.Close();
                    if (success > 1)
                    {
                        Logger.Error($" [Account] (ExecuteQuery) Query: {query} Success: {success}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($" [Account] (ExecuteQuery) Query: {query}\nException: {ex}");
            }
            return success == 1;
        }
    }
}