using PointBlank.Game;
using System;
using System.Collections.Generic;

namespace PointBlank.Api
{
    public class API_FUNCTION_REQ : ApiPacketReader
    {
        private byte type;
        private int error = 1;
        public override void ReadImplement()
        {
            type = ReadByte();
        }

        public override void RunImplement()
        {
            string response = "";
            ApiFunctionEnum function = (ApiFunctionEnum)type;
            switch (function)
            {
                case ApiFunctionEnum.KICK_PLAYER:
                    {
                        long playerId = ReadLong();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        player.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2));
                        player.Close(1000, true);
                        response = $"Você desconectou o jogador do servidor. Id: {player.playerId} Nick: {player.nickname}";
                        ApiManager.SendPacketToAllClients(new API_USER_DISCONNECT_ACK(player, 1));
                        break;
                    }
                case ApiFunctionEnum.KICK_ALL:
                    {
                        int count = 0;
                        using (AUTH_ACCOUNT_KICK_PAK packet = new AUTH_ACCOUNT_KICK_PAK(0))
                        {
                            if (GameManager.SocketSessions.Count > 0)
                            {
                                byte[] data = packet.GetCompleteBytes("KickAllPlayers");
                                foreach (GameClient client in GameManager.SocketSessions.Values)
                                {
                                    Account account = client.SessionPlayer;
                                    if (account != null && account.isOnline && account.access <= AccessLevelEnum.TransmissionChampionships)
                                    {
                                        account.SendCompletePacket(data);
                                        account.Close(1000, true);
                                        count++;
                                    }
                                }
                            }
                        }
                        response = $"Você desconectou {count} jogadores do servidor.";
                        break;
                    }
                case ApiFunctionEnum.KICK_AFK:
                    {
                        int count = GameManager.KickActiveClient();
                        response = $"Foram desconectados {count} jogadores por inatividade.";
                        List<Account> players = new List<Account>();
                        foreach (Account player in AccountManager.accounts.Values)
                        {
                            if (player.client != null && player.isOnline)
                            {
                                players.Add(player);
                            }
                        }
                        client.SendPacket(new API_ONLINE_PLAYERS_INFO_ACK(players));
                        break;
                    }
                case ApiFunctionEnum.SET_PCCAFE_BASIC:
                    {
                        long playerId = ReadLong();
                        int days = ReadInt();
                        int dateNow = int.Parse(DateTime.Now.AddDays(days).ToString("yyMMddHHmm"));
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        if (player.UpdatePccafe(1, dateNow, player.cash + 45000, player.gold + 50000))
                        {
                            player.cash += 45000;
                            player.gold += 50000;
                            player.pccafe = 1;
                            player.pccafeDate = dateNow;
                            if (player.isOnline)
                            {
                                player.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
                            }
                            response = $"Foi adicionado Pccafe Basic por {days} dias para o jogador. Id: {player.playerId} Nick: {player.nickname}";
                            ApiManager.SendPacketToAllClients(new API_USER_INFO_ACK(player));
                        }
                        else
                        {
                            response = "Não foi possivel atualizar Pccafe Basic na database.";
                            error = 0x8000;
                        }
                        break;
                    }
                case ApiFunctionEnum.SET_PCCAFE_PLUS:
                    {
                        long playerId = ReadLong();
                        int days = ReadInt();
                        int dateNow = int.Parse(DateTime.Now.AddDays(days).ToString("yyMMddHHmm"));
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        if (player.UpdatePccafe(2, dateNow, player.cash + 75000, player.gold + 80000))
                        {
                            player.cash += 75000;
                            player.gold += 80000;
                            player.pccafe = 2;
                            player.pccafeDate = dateNow;
                            if (player.isOnline)
                            {
                                player.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
                            }
                            response = $"Foi adicionado Pccafe Plus por {days} dias para o jogador. Id: {player.playerId} Nick: {player.nickname}";
                            ApiManager.SendPacketToAllClients(new API_USER_INFO_ACK(player));
                        }
                        else
                        {
                            response = "Não foi possivel atualizar Pccafe Plus na database.";
                            error = 0x8000;
                        }
                        break;
                    }
                case ApiFunctionEnum.SEND_MESSAGE_TO_PLAYER:
                    {
                        long playerId = ReadLong();
                        string message = ReadString(ReadByte());
                        if (message.Length >= 1024)
                        {
                            response = $"Não é possivel mandar uma mensagem muito grande.";
                            error = 0x8000;
                        }
                        else
                        {
                            Account player = AccountManager.GetAccount(playerId, true);
                            if (player == null)
                            {
                                Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                                error = 0x8000;
                                return;
                            }
                            if (player.isOnline)
                            {
                                player.client.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(message));
                                response = $"Mensagem enviada com sucesso.";
                            }
                            else
                            {
                                response = $"Não é possivel mandar uma mensagem para o jogador offline.";
                                error = 0x8000;
                            }
                        }
                        break;
                    }
                case ApiFunctionEnum.SET_NICKNAME:
                    {
                        long playerId = ReadLong();
                        string nickname = ReadString(ReadByte());
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        Room room = player.room;
                        if (player.nickname.Length > Settings.NickMaxLength || player.nickname.Length < Settings.NickMinLength)
                        {
                            response = $"Este nickname ({nickname}) está fora dos padrões de tamanho.";
                            error = 0x8000;
                        }
                        else if (AccountManager.CheckNicknameExist(nickname).Result)
                        {
                            response = $"Este nickname ({nickname}) já existe.";
                            error = 0x8000;
                        }
                        else if (player.ExecuteQuery($"UPDATE accounts SET nickname='{player.nickname}' WHERE id='{player.playerId}'"))
                        {
                            player.nickname = nickname;
                            if (!NickHistoryManager.CreateHistory(player.playerId, player.nickname, nickname, "Admin Change Nickname"))
                            {
                                Logger.Analyze($" [API_FUNCTION_REQ] Não foi possivel salvar o histórico de nome. PlayerId: {player.playerId} Nickname: {nickname} Motivo: First nick.");
                            }
                            player.SendPacket(new PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(player.nickname));
                            if (room != null)
                            {
                                using (PROTOCOL_ROOM_GET_NICKNAME_ACK packet = new PROTOCOL_ROOM_GET_NICKNAME_ACK(player.slotId, player.nickname, player.nickcolor))
                                {
                                    room.SendPacketToPlayers(packet);
                                }
                            }
                            if (player.clanId > 0)
                            {
                                List<Account> players = player.GetClanPlayers(-1);
                                using (PROTOCOL_CLAN_MEMBER_INFO_UPDATE_ACK packet = new PROTOCOL_CLAN_MEMBER_INFO_UPDATE_ACK(player))
                                {
                                    player.SendPacketForPlayers(packet, players);
                                }
                            }
                            player.SyncPlayerToFriends(true);
                            response = $"Seu nickname foi alterado para {nickname}.";
                            ApiManager.SendPacketToAllClients(new API_USER_INFO_ACK(player));
                        }
                        else
                        {
                            response = $"Falha ao atualizar o nickname ({nickname}) na database.";
                            error = 0x8000;
                        }
                        break;
                    }
                case ApiFunctionEnum.SEND_MESSAGE_TO_ALL:
                    {
                        string message = ReadString(ReadByte());
                        if (message.Length >= 1024)
                        {
                            response = $"Não é possivel mandar uma mensagem muito grande.";
                            error = 0x8000;
                        }
                        else
                        {
                            int count = 0;
                            using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(message))
                            {
                                count = GameManager.SendPacketToAllClients(packet);
                            }
                            response = $"Mensagem enviada a {count} jogadores do servidor.";
                        }
                        break;
                    }
                case ApiFunctionEnum.SEND_MESSAGE_TO_SPECIFIC_ROOM_IN_CHANNEL:
                    {
                        int channelId = ReadInt();
                        int roomId = ReadInt();
                        string message = ReadString(ReadByte());
                        if (message.Length >= 1024)
                        {
                            response = $"Não é possivel mandar uma mensagem muito grande.";
                            error = 0x8000;
                        }
                        else
                        {
                            Channel channel = ServersManager.GetChannel(channelId);
                            if (channel != null)
                            {
                                Room roomSelected = channel.GetRoom(roomId);
                                if (roomSelected != null)
                                {
                                    int count = 0;
                                    using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(message))
                                    {
                                        count = roomSelected.SendMessageToPlayers(packet);
                                    }
                                    response = $"Mensagem enviada a {count} jogadores da sala {roomId} no canal {channel.id}.";
                                }
                                else
                                {
                                    response = $"Sala ({roomId}) inexistente neste canal.";
                                    error = 0x8000;
                                }
                            }
                            else
                            {
                                response = $"Canal ({channel.id}) inexistente.";
                                error = 0x8000;
                            }
                        }
                        break;
                    }
                case ApiFunctionEnum.SET_CASH:
                    {
                        long playerId = ReadLong();
                        int valor = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        long cashCalculated = player.cash + valor;
                        if (cashCalculated > 999999999)
                        {
                            response = "Não é possivel adicionar esse valor de cash para este jogador no momento.";
                            error = 0x8000;
                        }
                        else
                        {
                            int cashValid = (int)cashCalculated;
                            if (player.UpdateAccountCash(cashValid))
                            {
                                player.cash += cashValid;
                                player.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
                                response = $"O jogador {player.nickname} recebeu {valor} de cash.";
                                ApiManager.SendPacketToAllClients(new API_USER_INFO_ACK(player));
                            }
                            else
                            {
                                response = "Não foi possivel atualizar o cash para este jogador.";
                                error = 0x8000;
                            }
                        }
                        break;
                    }
                case ApiFunctionEnum.SET_GOLD:
                    {
                        long playerId = ReadLong();
                        int valor = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        long goldCalculated = player.gold + valor;
                        if (goldCalculated > 999999999)
                        {
                            response = "Não é possivel adicionar esse valor de gold para este jogador no momento.";
                            error = 0x8000;
                        }
                        else
                        {
                            int goldValid = (int)goldCalculated;
                            if (player.UpdateAccountGold(goldValid))
                            {
                                player.gold += goldValid;
                                player.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
                                response = $"O jogador {player.nickname} recebeu {valor} de gold.";
                                ApiManager.SendPacketToAllClients(new API_USER_INFO_ACK(player));
                            }
                            else
                            {
                                response = "Não foi possivel atualizar o gold para este jogador.";
                                error = 0x8000;
                            }
                        }
                        break;
                    }
                case ApiFunctionEnum.BATTLE_END_BY_PLAYER_SELECTED:
                    {
                        long playerId = ReadLong();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        Room room = player.room;
                        if (room != null)
                        {
                            if (room.IsPreparing())
                            {
                                room.EndBattle(room.IsBotMode(), room.GetWinnerTeam());
                                response = $"Você finalizou a partida da sala {room.roomId}";
                            }
                            else
                            {
                                response = "Não foi possivel finalizar a partida no momento.";
                                error = 0x8000;
                            }
                        }
                        else
                        {
                            response = "Você precisa estar presente em uma sala.";
                            error = 0x8000;
                        }
                        break;
                    }
                case ApiFunctionEnum.BATTLE_END_TO_SPECIFIC_ROOM_SPECIFIC_IN_CHANNEL:
                    {
                        long playerId = ReadLong();
                        int channelId = ReadInt();
                        int roomId = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        Room room = player.room;
                        Channel channel = ServersManager.GetChannel(channelId);
                        if (channel != null)
                        {
                            Room roomSelected = channel.GetRoom(roomId);
                            if (roomSelected != null)
                            {
                                if (room.IsPreparing())
                                {
                                    room.EndBattle(room.IsBotMode(), room.GetWinnerTeam());
                                    response = $"Você finalizou a partida da sala {room.roomId} no canal {channelId}";
                                }
                                else
                                {
                                    response = "Não foi possivel finalizar a partida no momento.";
                                    error = 0x8000;
                                }
                            }
                            else
                            {
                                response = "Você precisa estar presente em uma sala.";
                                error = 0x8000;
                            }
                        }
                        break;
                    }
                case ApiFunctionEnum.CHANGE_ROOM_MODE:
                    {
                        long playerId = ReadLong();
                        int stageType = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        Room room = player.room;
                        if (room != null)
                        {
                            room.mode = (RoomTypeEnum)stageType;
                            room.UpdateRoomInfo();
                            response = $"Você alterou o modo da sala. Mode: {room.mode}";
                        }
                        else
                        {
                            response = "Você precisa estar presente em uma sala.";
                            error = 0x8000;
                        }
                        break;
                    }
                case ApiFunctionEnum.CHANGE_ROOM_MODESPECIAL:
                    {
                        long playerId = ReadLong();
                        int special = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        Room room = player.room;
                        if (room != null)
                        {
                            room.modeSpecial = (RoomModeSpecial)special;
                            room.UpdateRoomInfo();
                            response = $"Você alterou o modo especial da sala. ModeSpecial: {room.modeSpecial}";
                        }
                        else
                        {
                            response = "Você precisa estar presente em uma sala.";
                            error = 0x8000;
                        }
                        break;
                    }
                case ApiFunctionEnum.CHANGE_ROOM_WEAPONSFLAG:
                    {
                        long playerId = ReadLong();
                        int flags = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        Room room = player.room;
                        if (room != null)
                        {
                            room.weaponsFlag = (byte)flags;
                            room.UpdateRoomInfo();
                            response = $"Você alterou a flag dos equipamentos da sala. WeaponsFlag: {(RoomWeaponsFlag)flags}";
                        }
                        else
                        {
                            response = "Você precisa estar presente em uma sala.";
                            error = 0x8000;
                        }
                        break;
                    }
                case ApiFunctionEnum.SET_RANK:
                    {
                        long playerId = ReadLong();
                        byte rank = ReadByte();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        if (rank > 55 || rank < 0)
                        {
                            response = "O rank escolhido é inválido.";
                            error = 0x8000;
                        }
                        else if (player.rankId == rank)
                        {
                            response = "Você já possui este rank atualmente.";
                            error = 0x8000;
                        }
                        else if (player.ExecuteQuery($"UPDATE accounts SET rank='{rank}' WHERE id='{player.playerId}'"))
                        {
                            player.rankId = rank;
                            int itemIdToRemove = 0;
                            if (player.rankId == 8)
                            {
                                itemIdToRemove = 1301268000;
                            }
                            else if (player.rankId == 12)
                            {
                                itemIdToRemove = 1301271000;
                            }
                            else if (player.rankId == 14)
                            {
                                itemIdToRemove = 1301272000;
                            }
                            else if (player.rankId == 17)
                            {
                                itemIdToRemove = 1301276000;
                            }
                            else if (player.rankId == 26)
                            {
                                itemIdToRemove = 1302040000;
                            }
                            else if (player.rankId == 31)
                            {
                                itemIdToRemove = 1302041000;
                            }
                            else if (player.rankId == 36)
                            {
                                itemIdToRemove = 1302042000;
                            }
                            else if (player.rankId == 41)
                            {
                                itemIdToRemove = 1302043000;
                            }
                            if (itemIdToRemove != 0)
                            {
                                ItemsModel item = player.inventory.GetItem(itemIdToRemove);
                                if (item != null)
                                {
                                    if (player.DeleteItem(item.objectId))
                                    {
                                        player.inventory.RemoveItem(item);
                                    }
                                    player.SendPacket(new INVENTORY_ITEM_EXCLUDE_PAK(1, item.objectId));
                                }
                            }
                            List<ItemsModel> items = RankManager.GetAwards(player.rankId);
                            if (items.Count > 0)
                            {
                                player.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, items));
                            }
                            RankModel NextRank = RankManager.GetRank(player.rankId);
                            if (NextRank != null)
                            {
                                int Experience = player.exp - NextRank.onNextLevel - NextRank.onAllExp + NextRank.onAllExp;
                                player.exp = Experience;
                                player.SendPacket(new PROTOCOL_BASE_RANK_UP_ACK(rank, NextRank.onNextLevel));
                            }
                            Room room = player.room;
                            if (room != null)
                            {
                                room.UpdateSlotsInfo();
                            }
                            response = $"Seu rank foi alterado para {rank}.";
                            ApiManager.SendPacketToAllClients(new API_USER_INFO_ACK(player));
                        }
                        else
                        {
                            response = "Falha ao atualizar o rank na database.";
                            error = 0x8000;
                        }
                        break;
                    }
                case ApiFunctionEnum.NICK_HISTORY:
                    {
                        int type = ReadByte();
                        if (type == 0)
                        {
                            string nickname = ReadString(ReadByte());
                            client.SendPacket(new API_HISTORY_NICKNAME_ACK(NickHistoryManager.GetHistory(nickname, type)));
                            response = "Buscando o histórico de nickname pelo nickname.";
                        }
                        else
                        {
                            long playerId = ReadLong();
                            client.SendPacket(new API_HISTORY_NICKNAME_ACK(NickHistoryManager.GetHistory(playerId, type)));
                             response = "Buscando o histórico de nickname pelo playerId.";
                        }
                        break;
                    }
                case ApiFunctionEnum.ADD_ITEM:
                    {
                        long playerId = ReadLong();
                        int itemId = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        if (itemId < 100000000)
                        {
                            response = "Este item é inválido.";
                        }
                        else
                        {
                            int category = Utilities.GetItemCategory(itemId);
                            player.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, new ItemsModel(itemId, category, "Command item", (byte)(category == 3 ? 1 : 3), 1)));
                            player.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0));
                            response = $"O item {itemId} foi adicionado com sucesso, verifique seu inventário.";
                        }
                        break;
                    }
                case ApiFunctionEnum.BLOCK_USER_ONLINE:
                    {
                        DateTime date = DateTime.Now;

                        long playerId = ReadLong();
                        DateTime endDate = date.AddDays(ReadInt());
                        string reason = ReadString(ReadByte());
                        string linkVideo = ReadString(ReadByte());
                        string linkPrintScreen = ReadString(ReadByte());
                        string comment = ReadString(ReadByte());

                        Account player = AccountManager.GetAccount(playerId, true); //true=Não usa a db, ou seja, somente players online.
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        UserBlock user = new UserBlock
                        {
                            ipAddress = player.ipAddress.ToString(),
                            macAddress = player.macAddress.ToString(),
                            //biosId = player.biosId,
                            //diskId = player.diskId,
                            //videoId = player.videoId,
                            startDate = date,
                            endDate = endDate,
                            reason = reason,
                            linkVideo = linkVideo,
                            linkPrintScreen = linkPrintScreen,
                            comment = comment,
                            userId = playerId
                        };
                        if (ServerBlockManager.AddBlock(user, client.admin))
                        {
                            response = $"Usuário bloqueado.";
                        }
                        else
                        {
                            response = $"Falha ao bloquear o usuário.";
                            error = 0x8000;
                        }
                        break;
                    }
                case ApiFunctionEnum.BLOCK_USER_BY_ID:
                    {
                        DateTime date = DateTime.Now;

                        long playerId = ReadLong();
                        DateTime endDate = date.AddDays(ReadInt());
                        string reason = ReadString(ReadByte());
                        string linkVideo = ReadString(ReadByte());
                        string linkPrintScreen = ReadString(ReadByte());
                        string comment = ReadString(ReadByte());

                        Account player = AccountManager.GetAccount(playerId, false); //true=Não usa a db, ou seja, somente players online.
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            error = 0x8000;
                            return;
                        }
                        UserBlock user = new UserBlock
                        {
                            ipAddress = player.ipAddress.ToString(),
                            macAddress = player.macAddress.ToString(),
                            //biosId = player.biosId,
                            //diskId = player.diskId,
                            //videoId = player.videoId,
                            startDate = date,
                            endDate = endDate,
                            reason = reason,
                            linkVideo = linkVideo,
                            linkPrintScreen = linkPrintScreen,
                            comment = comment,
                            userId = playerId
                        };
                        if (ServerBlockManager.AddBlock(user, client.admin))
                        {
                            response = $"Usuário bloqueado.";
                        }
                        else
                        {
                            response = $"Falha ao bloquear o usuário.";
                            error = 0x8000;
                        }
                        break;
                    }
                case ApiFunctionEnum.SEARCH_USER_BY_NICKNAME:
                    {
                        string nickname = ReadString(ReadByte());
                        Account player = AccountManager.GetAccount(nickname, 0);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Nickname: {nickname}");
                            error = 0x8000;
                            return;
                        }
                        response = "Usuário encontrado.";
                        client.SendPacket(new API_SEARCH_USER_ACK(player));
                        break;
                    }
            }
            Logger.Warning(response);
            client.SendPacket(new API_RESULT_FUNCTION_ACK(error, type, response));
        }
    }
}