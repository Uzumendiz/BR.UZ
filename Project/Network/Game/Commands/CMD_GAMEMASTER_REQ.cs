using PointBlank.Game;
using System;
using System.Collections.Generic;

namespace PointBlank
{
    public class CMD_GAMEMASTER_REQ : PacketCommand
    {
        private string command;
        private byte type;
        public CMD_GAMEMASTER_REQ(string command, byte type)
        {
            this.command = command;
            this.type = type;
        }

        public override void RunImplement()
        {
            if (type == 1)
            {
                if (administrador.rankId == 53 || administrador.rankId == 54)
                {
                    administrador.hideGMcolor = !administrador.hideGMcolor;
                    if (administrador.hideGMcolor)
                    {
                        response = "A Cor do seu nickname foi alterada para a cor padrão.";
                    }
                    else
                    {
                        response = "A Cor do seu nickname foi alterada para a cor de GameMaster";
                    }
                }
                else
                {
                    response = "Não foi possivel alterar a cor do nickname devido seu rank não ser GM/MOD.";
                }
            }
            else if (type == 2)
            {
                administrador.antiKickGM = !administrador.antiKickGM;
                if (administrador.antiKickGM)
                {
                    response = "O sistema de Anti-Kick foi ativado na sua conta.";
                }
                else
                {
                    response = "O sistema de Anti-Kick foi desativado na sua conta.";
                }
            }
            else if (type == 3)
            {
                byte rank = byte.Parse(command.Substring(5));
                if (rank > 55 || rank < 0)
                {
                    response = "O rank escolhido é inválido.";
                }
                else if (administrador.rankId == rank)
                {
                    response = "Você já possui este rank atualmente.";
                }             
                else if (administrador.ExecuteQuery($"UPDATE accounts SET rank='{rank}' WHERE id='{administrador.playerId}'"))
                {
                    administrador.rankId = rank;
                    int itemIdToRemove = 0;
                    if (administrador.rankId == 8)
                    {
                        itemIdToRemove = 1301268000;
                    }
                    else if (administrador.rankId == 12)
                    {
                        itemIdToRemove = 1301271000;
                    }
                    else if (administrador.rankId == 14)
                    {
                        itemIdToRemove = 1301272000;
                    }
                    else if (administrador.rankId == 17)
                    {
                        itemIdToRemove = 1301276000;
                    }
                    else if (administrador.rankId == 26)
                    {
                        itemIdToRemove = 1302040000;
                    }
                    else if (administrador.rankId == 31)
                    {
                        itemIdToRemove = 1302041000;
                    }
                    else if (administrador.rankId == 36)
                    {
                        itemIdToRemove = 1302042000;
                    }
                    else if (administrador.rankId == 41)
                    {
                        itemIdToRemove = 1302043000;
                    }
                    if (itemIdToRemove != 0)
                    {
                        ItemsModel item = administrador.inventory.GetItem(itemIdToRemove);
                        if (item != null)
                        {
                            if (administrador.DeleteItem(item.objectId))
                            {
                                administrador.inventory.RemoveItem(item);
                            }
                            administrador.SendPacket(new INVENTORY_ITEM_EXCLUDE_PAK(1, item.objectId));
                        }
                    }
                    List<ItemsModel> items = RankManager.GetAwards(administrador.rankId);
                    if (items.Count > 0)
                    {
                        administrador.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, administrador, items));
                    }
                    RankModel NextRank = RankManager.GetRank(administrador.rankId);
                    if (NextRank != null)
                    {
                        int Experience = administrador.exp - NextRank.onNextLevel - NextRank.onAllExp + NextRank.onAllExp;
                        administrador.exp = Experience;
                        administrador.SendPacket(new PROTOCOL_BASE_RANK_UP_ACK(rank, NextRank.onNextLevel));
                    }
                    if (room != null)
                    {
                        room.UpdateSlotsInfo();
                    }
                    response = $"Seu rank foi alterado para {rank}.";
                }
                else
                {
                    response = "Falha ao atualizar o rank na database.";
                }
            }
            else if (type == 4)
            {
                string nickname = command.Substring(5);
                if (AccountManager.CheckNickLengthInvalid(nickname))
                {
                    response = "Este nickname está fora dos padrões de tamanho.";
                }
                else if (!AccountManager.CheckNicknameExist(nickname).Result && administrador.UpdateNick(nickname).Result)
                {
                    administrador.nickname = nickname;
                    administrador.SendPacket(new PROTOCOL_AUTH_CHANGE_NICKNAME_ACK(nickname));
                    if (room != null)
                    {
                        using (PROTOCOL_ROOM_GET_NICKNAME_ACK packet = new PROTOCOL_ROOM_GET_NICKNAME_ACK(administrador.slotId, administrador.nickname, administrador.nickcolor))
                        {
                            room.SendPacketToPlayers(packet);
                        }
                    }
                    if (administrador.clanId > 0)
                    {
                        List<Account> players = administrador.GetClanPlayers(-1);
                        using (PROTOCOL_CLAN_MEMBER_INFO_UPDATE_ACK packet = new PROTOCOL_CLAN_MEMBER_INFO_UPDATE_ACK(administrador))
                        {
                            administrador.SendPacketForPlayers(packet, players);
                        }
                    }
                    administrador.SyncPlayerToFriends(true);
                    response = $"Seu nickname foi alterado para {nickname}.";
                }
                else
                {
                    response = "Este nickname já existe.";
                }
            }
            else if (type == 5)
            {
                int itemId = int.Parse(command.Substring(8));
                if (itemId < 100000000)
                {
                    response = "Este item é inválido.";
                }
                else
                {
                    int category = Utilities.GetItemCategory(itemId);
                    administrador.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, administrador, new ItemsModel(itemId, category, "Command item", (byte)(category == 3 ? 1 : 3), 1)));
                    administrador.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0));
                    response = $"O item {itemId} foi adicionado com sucesso, verifique seu inventário.";
                }
            }
            else if (type == 6)
            {
                string playerName = command.Substring(12);
                Account playerSet = playerName == administrador.nickname ? administrador : AccountManager.GetAccount(playerName, 0);
                if (playerSet != null)
                {
                    int dateNow = int.Parse(DateTime.Now.AddDays(30).ToString("yyMMddHHmm"));
                    if (playerSet.UpdatePccafe(1, dateNow, playerSet.cash + 45000, playerSet.gold + 50000))
                    {
                        playerSet.cash += 45000;
                        playerSet.gold += 50000;
                        playerSet.pccafe = 1;
                        playerSet.pccafeDate = dateNow;
                        if (playerSet.isOnline)
                        {
                            playerSet.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, playerSet.gold, playerSet.cash));
                        }
                    }
                    else
                    {
                        response = "Não foi possivel atualizar Vip Basic na database.";
                    }
                }
                else
                {
                    response = "Não foi possivel encontrar o jogador.";
                }
            }
            else if (type == 7)
            {
                string playerName = command.Substring(11);
                Account playerSet = playerName == administrador.nickname ? administrador : AccountManager.GetAccount(playerName, 0);
                if (playerSet != null)
                {
                    int dateNow = int.Parse(DateTime.Now.AddDays(30).ToString("yyMMddHHmm"));
                    if (playerSet.UpdatePccafe(2, dateNow, playerSet.cash + 75000, playerSet.gold + 80000))
                    {
                        playerSet.cash += 75000;
                        playerSet.gold += 80000;
                        playerSet.pccafe = 2;
                        playerSet.pccafeDate = dateNow;
                        if (playerSet.isOnline)
                        {
                            playerSet.SendPacket(new PROTOCOL_BASE_WEB_CASH_ACK(0, playerSet.gold, playerSet.cash));
                        }
                    }
                    else
                    {
                        response = "Não foi possivel atualizar Vip Plus na database.";
                    }
                }
                else
                {
                    response = "Não foi possivel encontrar o jogador.";
                }
            }
        }
    }
}
