using PointBlank.Game;
using System;
using System.Collections.Generic;

namespace PointBlank.Api
{
    public class API_FUNCTION_REQ : ApiPacketReader
    {
        private byte type;
        public override void ReadImplement()
        {
            type = ReadByte();
        }

        public override void RunImplement()
        {
            string response = "";
            switch (type)
            {
                case 1: //Kick Player
                    {
                        long playerId = ReadLong();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            return;
                        }
                        player.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2));
                        player.Close(1000, true);
                        response = $"Você desconectou o jogador do servidor. Id: {player.playerId} Nick: {player.nickname}";
                        break;
                    }
                case 2: //Kick All
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
                case 3: //Kick AFK
                    {
                        int count = GameManager.KickActiveClient();
                        response = $"Foram desconectados {count} jogadores por inatividade.";
                        break;
                    }
                case 4: //Set Pccafe Basic
                    {
                        long playerId = ReadLong();
                        int days = ReadInt();
                        int dateNow = int.Parse(DateTime.Now.AddDays(days).ToString("yyMMddHHmm"));
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
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
                        }
                        else
                        {
                            response = "Não foi possivel atualizar Pccafe Basic na database.";
                        }
                        break;
                    }
                case 5: //Set Pccafe Plus
                    {
                        long playerId = ReadLong();
                        int days = ReadInt();
                        int dateNow = int.Parse(DateTime.Now.AddDays(days).ToString("yyMMddHHmm"));
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
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
                        }
                        else
                        {
                            response = "Não foi possivel atualizar Pccafe Plus na database.";
                        }
                        break;
                    }
                case 6: //Block user
                    {
                        break;
                    }
                case 7: //Set Nickname
                    {
                        long playerId = ReadLong();
                        string nickname = ReadString(ReadByte());
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            return;
                        }
                        Room room = player.room;
                        if (player.nickname.Length > Settings.NickMaxLength || player.nickname.Length < Settings.NickMinLength)
                        {
                            response = $"Este nickname ({nickname}) está fora dos padrões de tamanho.";
                        }
                        else if (AccountManager.CheckNicknameExist(player.nickname).Result)
                        {
                            response = $"Este nickname ({nickname}) já existe.";
                        }
                        else if (player.ExecuteQuery($"UPDATE accounts SET nickname='{player.nickname}' WHERE id='{player.playerId}'"))
                        {
                            player.nickname = nickname;
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
                        }
                        else
                        {
                            response = $"Falha ao atualizar o nickname ({nickname}) na database.";
                        }
                        break;
                    }
                case 8: //Send Message To All
                    {
                        string message = ReadString(ReadByte());
                        if (message.Length >= 1024)
                        {
                            response = $"Não é possivel mandar uma mensagem muito grande.";
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
                case 9: //Send Message To Specific Room in Specific Channel
                    {
                        int channelId = ReadInt();
                        int roomId = ReadInt();
                        string message = ReadString(ReadByte());
                        if (message.Length >= 1024)
                        {
                            response = $"Não é possivel mandar uma mensagem muito grande.";
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
                                }
                            }
                            else
                            {
                                response = $"Canal ({channel.id}) inexistente.";
                            }
                        }
                        break;
                    }
                case 10: //Add Cash
                    {
                        long playerId = ReadLong();
                        int valor = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            return;
                        }
                        Room room = player.room;
                        long cashCalculated = player.cash + valor;
                        if (cashCalculated > 999999999)
                        {
                            response = "Não é possivel adicionar esse valor de cash para este jogador no momento.";
                        }
                        else
                        {
                            int cashValid = (int)cashCalculated;
                            if (player.UpdateAccountCash(cashValid))
                            {
                                player.cash += cashValid;
                                player.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
                                response = $"O jogador {player.nickname} recebeu {valor} de cash.";
                            }
                            else
                            {
                                response = "Não foi possivel atualizar o cash para este jogador.";
                            }
                        }
                        break;
                    }
                case 11: //Add Gold
                    {
                        long playerId = ReadLong();
                        int valor = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
                            return;
                        }
                        long goldCalculated = player.gold + valor;
                        if (goldCalculated > 999999999)
                        {
                            response = "Não é possivel adicionar esse valor de gold para este jogador no momento.";
                        }
                        else
                        {
                            int goldValid = (int)goldCalculated;
                            if (player.UpdateAccountGold(goldValid))
                            {
                                player.gold += goldValid;
                                player.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, player.gold, player.cash));
                                response = $"O jogador {player.nickname} recebeu {valor} de gold.";
                            }
                            else
                            {
                                response = "Não foi possivel atualizar o gold para este jogador.";
                            }
                        }
                        break;
                    }
                case 12: //Battle End by Player Selected
                    {
                        long playerId = ReadLong();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
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
                            }
                        }
                        else
                        {
                            response = "Você precisa estar presente em uma sala.";
                        }
                        break;
                    }
                case 13: //Battle End To Specific Room in Specific Channel
                    {
                        long playerId = ReadLong();
                        int channelId = ReadInt();
                        int roomId = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
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
                                }
                            }
                            else
                            {
                                response = "Você precisa estar presente em uma sala.";
                            }
                        }
                        break;
                    }
                case 14:
                    {
                        long playerId = ReadLong();
                        int stageType = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
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
                        }
                        break;
                    }
                case 15:
                    {
                        long playerId = ReadLong();
                        int special = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
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
                        }
                        break;
                    }
                case 16:
                    {
                        long playerId = ReadLong();
                        int flags = ReadInt();
                        Account player = AccountManager.GetAccount(playerId, true);
                        if (player == null)
                        {
                            Logger.Warning($" [{GetType().Name}] Player is null. Id: {playerId}");
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
                        }
                        break;
                    }
                case 17:
                    {
                        break;
                    }
                case 18:
                    {
                        break;
                    }
                case 19:
                    {
                        break;
                    }
                case 20:
                    {
                        break;
                    }
            }
            Logger.Warning(response);
            client.SendPacket(new API_RESULT_FUNCTION_ACK(response));
        }
    }
}