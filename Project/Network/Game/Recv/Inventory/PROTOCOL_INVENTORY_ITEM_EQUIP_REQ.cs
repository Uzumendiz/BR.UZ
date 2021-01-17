using System;
using System.Collections.Generic;
using System.Globalization;

namespace PointBlank.Game
{
    public class PROTOCOL_INVENTORY_ITEM_EQUIP_REQ : GamePacketReader
    {
        private long objectId;
        private int itemId, oldCOUNT;
        private uint erro = 1;
        public override void ReadImplement()
        {
            objectId = ReadLong();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                ItemsModel itemObj = player != null ? player.inventory.GetItem(objectId) : null;
                if (itemObj != null)
                {
                    itemId = itemObj.id;
                    oldCOUNT = itemObj.count;
                    if (itemObj.category == 3 && player.inventory.items.Count >= 500)
                    {
                        client.SendPacket(new INVENTORY_ITEM_EQUIP_PAK(0x80001029));
                        Logger.Warning($" [GAME] [PROTOCOL_INVENTORY_ITEM_EQUIP_REQ] Foi atingido o limite de 500 items no inventário. Nick: {player.nickname} Login: {player.login} PlayerId: {player.playerId}");
                        return;
                    }
                    if (itemId == 1301049000)
                    {
                        if (player.UpdateKDReset())
                        {
                            player.statistics.kills = 0;
                            player.statistics.deaths = 0;
                            client.SendPacket(new BASE_USER_CHANGE_STATS_PAK(player.statistics));
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                            return;
                        }
                    }
                    else if (itemId == 1301048000)
                    {
                        if (player.UpdateTotalFights())
                        {
                            player.statistics.fights = 0;
                            player.statistics.fightsWin = 0;
                            player.statistics.fightsLost = 0;
                            player.statistics.fightsDraw = 0;
                            client.SendPacket(new BASE_USER_CHANGE_STATS_PAK(player.statistics));
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                            return;
                        }
                    }
                    else if (itemId == 1301050000)
                    {
                        if (player.ExecuteQuery($"UPDATE accounts SET fights_escapes='0' WHERE id='{player.playerId}'"))
                        {
                            player.statistics.escapes = 0;
                            client.SendPacket(new BASE_USER_CHANGE_STATS_PAK(player.statistics));
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                            return;
                        }
                    }
                    else if (itemId == 1301053000)
                    {
                        Clan clan = ClanManager.GetClan(player.clanId);
                        if (clan.id > 0 && clan.ownerId == player.playerId && clan.UpdateBattlesReset())
                        {
                            clan.partidas = 0;
                            clan.vitorias = 0;
                            clan.derrotas = 0;
                            client.SendPacket(new CLAN_CHANGE_FIGHTS_PAK());
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                            return;
                        }
                    }
                    else if (itemId == 1301055000)
                    {
                        Clan clan = ClanManager.GetClan(player.clanId);
                        if (clan.id > 0 && clan.ownerId == player.playerId)
                        {
                            if ((clan.maxPlayers + 50) <= 250 && player.ExecuteQuery($"UPDATE clan_data SET max_players='{clan.maxPlayers + 50}' WHERE clan_id='{player.clanId}'"))
                            {
                                clan.maxPlayers += 50;
                                client.SendPacket(new CLAN_CHANGE_MAX_PLAYERS_PAK(clan.maxPlayers));
                            }
                            else
                            {
                                erro = 0x80001056;
                            }
                        }
                        else
                        {
                            erro = 0x80001056;
                        }
                    }
                    else if (itemId == 1301056000)
                    {
                        Clan clan = ClanManager.GetClan(player.clanId);
                        if (clan.id > 0 && clan.pontos != 1000)
                        {
                            if (player.ExecuteQuery($"UPDATE clan_data SET pontos='{1000.0f}' WHERE clan_id='{player.clanId}'"))
                            {
                                clan.pontos = 1000;
                                client.SendPacket(new CLAN_CHANGE_POINTS_PAK());
                            }
                            else
                            {
                                erro = 0x80001056;
                            }
                        }
                        else
                        {
                            erro = 0x80001056;
                        }
                    }
                    else if (itemId > 1301113000 && itemId < 1301119000)
                    {
                        int goldReceive = itemId == 1301114000 ? 500 : (itemId == 1301115000 ? 1000 : (itemId == 1301116000 ? 5000 : (itemId == 1301117000 ? 10000 : 30000)));
                        if ((player.gold + goldReceive) <= 999999999 && player.ExecuteQuery($"UPDATE accounts SET gold='{player.gold + goldReceive}' WHERE id='{player.playerId}'"))
                        {
                            player.gold += goldReceive;
                            client.SendPacket(new AUTH_GOLD_REWARD_PAK(goldReceive, player.gold, 0));
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                            return;
                        }
                    }
                    else if (itemId == 1301999000)
                    {
                        if ((player.exp + 515999) <= 999999999 && player.ExecuteQuery($"UPDATE accounts SET exp='{player.exp + 515999}' WHERE id='{player.playerId}'"))
                        {
                            player.exp += 515999;
                            client.SendPacket(new A_3096_PAK(515999, 0));
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                            return;
                        }
                    }
                    else if (itemObj.category == 3 && RandomBoxXML.ContainsBox(itemId))
                    {
                        RandomBoxModel box = RandomBoxXML.GetBox(itemId);
                        if (box != null)
                        {
                            List<RandomBoxItem> sortedList = box.GetSortedList(GetRandomNumber(1, 100));
                            List<RandomBoxItem> rewardList = box.GetRewardList(sortedList, GetRandomNumber(0, sortedList.Count));
                            if (rewardList.Count > 0)
                            {
                                byte itemIdx = (byte)rewardList[0].index;
                                client.SendPacket(new AUTH_RANDOM_BOX_REWARD_PAK(itemId, itemIdx));
                                List<ItemsModel> rewards = new List<ItemsModel>();
                                for (int i = 0; i < rewardList.Count; i++)
                                {
                                    RandomBoxItem cupom = rewardList[i];
                                    if (cupom.item != null)
                                    {
                                        rewards.Add(cupom.item);
                                    }
                                    else if (player.UpdateAccountGold(player.gold + cupom.count))
                                    {
                                        player.gold += cupom.count;
                                        client.SendPacket(new AUTH_GOLD_REWARD_PAK(cupom.count, player.gold, 0));
                                    }
                                    else
                                    {
                                        client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                                        break;
                                    }
                                    if (cupom.special)
                                    {
                                        using (AUTH_JACKPOT_NOTICE_PAK packet = new AUTH_JACKPOT_NOTICE_PAK(player.nickname, itemId, itemIdx))
                                        {
                                            GameManager.SendPacketToAllClients(packet);
                                        }
                                    }
                                }
                                if (rewards.Count > 0)
                                {
                                    client.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, rewards));
                                }
                                rewards = null;
                            }
                            else
                            {
                                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                                return;
                            }
                            sortedList = null;
                            rewardList = null;
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                            return;
                        }
                    }
                    else if (player.rankId < 31 && (itemId == 1103003001 || itemId == 1103003002 || itemId == 1103003003 || itemId == 1103003004 || itemId == 1103003005))
                    {
                        client.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK("Você não tem rank suficiente para equipar esta boina."));
                        return;
                    }
                    else
                    {
                        int wclass = GetIdStatics(itemObj.id, 1);
                        if (wclass <= 11)
                        {
                            if (itemObj.equip == 1)
                            {
                                itemObj.equip = 2;
                                itemObj.count = int.Parse(DateTime.Now.AddSeconds(itemObj.count).ToString("yyMMddHHmm"));
                                player.ExecuteQuery($"UPDATE player_items SET count='{itemObj.count}', equip='{(int)itemObj.equip}' WHERE object_id='{objectId}' AND owner_id='{player.playerId}'");
                            }
                            else
                            {
                                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                                return;
                            }
                        }
                        else if (wclass == 13)
                        {
                            CupomIncreaseDays(player, itemObj.name);
                        }
                        else if (wclass == 15)
                        {
                            CupomIncreaseGold(player, itemObj.id);
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                            return;
                        }
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                    return;
                }
                client.SendPacket(new INVENTORY_ITEM_EQUIP_PAK(erro, itemObj, player));
            }
            catch (OverflowException ex)
            {
                Logger.Error($" [PROTOCOL_INVENTORY_ITEM_EQUIP_REQ] [OverflowException] ObjectId: {objectId} ItemId: {itemId} Count na DB: {oldCOUNT} Nick: {client.SessionPlayer.nickname}\r\n" + ex.ToString());
                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
            }
            catch (Exception ex)
            {
                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                PacketLog(ex);
            }
        }

        private static Random getrandom = new Random();
        private static object syncLock = new object();
        private static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return getrandom.Next(min, max);
            }
        }
        private void CupomIncreaseDays(Account player, string originalName)
        {
            int cupomId = CreateItemId(12, GetIdStatics(itemId, 2), GetIdStatics(itemId, 3), 0);
            int days = GetIdStatics(itemId, 4);
            player.SetCuponsFlags();
            CupomEffects effects = player.effects;
            if (cupomId == 1200065000 && ((effects & (CupomEffects.Colete5 | CupomEffects.Colete10 | CupomEffects.Colete20)) > 0) ||
                cupomId == 1200079000 && ((effects & (CupomEffects.Colete5 | CupomEffects.Colete10 | CupomEffects.Colete90)) > 0) ||
                cupomId == 1200044000 && ((effects & (CupomEffects.Colete5 | CupomEffects.Colete20 | CupomEffects.Colete90)) > 0) ||
                cupomId == 1200030000 && ((effects & (CupomEffects.Colete20 | CupomEffects.Colete10 | CupomEffects.Colete90)) > 0) ||
                cupomId == 1200078000 && ((effects & (CupomEffects.IronBullet | CupomEffects.HollowPoint | CupomEffects.HollowPointF)) > 0) ||
                cupomId == 1200032000 && ((effects & (CupomEffects.HollowPointF | CupomEffects.HollowPointPlus | CupomEffects.IronBullet)) > 0) ||
                cupomId == 1200031000 && ((effects & (CupomEffects.HollowPointF | CupomEffects.HollowPointPlus | CupomEffects.HollowPoint)) > 0) ||
                cupomId == 1200036000 && ((effects & (CupomEffects.HollowPoint | CupomEffects.HollowPointPlus | CupomEffects.IronBullet)) > 0) ||
                cupomId == 1200028000 && effects.HasFlag(CupomEffects.HP5) ||
                cupomId == 1200040000 && effects.HasFlag(CupomEffects.HP10))
            {
                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                return;
            }
            else
            {
                ItemsModel item = player.inventory.GetItem(cupomId);
                if (item == null)
                {
                    bool changed = player.bonus.AddBonuses(cupomId);
                    if (changed)
                    {
                        player.UpdatePlayerBonus();
                    }
                    client.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, new ItemsModel(cupomId, 3, originalName + " [Active]", 2, int.Parse(DateTime.Now.AddDays(days).ToString("yyMMddHHmm")))));
                }
                else
                {
                    DateTime data = DateTime.ParseExact(item.count.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture).AddDays(days);
                    if ((data - DateTime.Now).Days + 1 > Settings.MaxBuyItemDays) //+1 porque ele não conta o dia atual da compra neste calculo, no jogo compra 30 dias, aqui mostra 29 e somo mais 1
                    {
                        Logger.Warning($" [GAME] [{GetType().Name}] Não foi possivel comprar mais de {Settings.MaxBuyItemDays} dias do mesmo equipamento. ItemId: {item.id} PlayerId: {player.playerId} Date: {DateTime.Now}");
                        client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                        return;
                    }
                    item.count = int.Parse(data.ToString("yyMMddHHmm"));
                    player.ExecuteQuery($"UPDATE player_items SET count='{item.count}' WHERE object_id='{item.objectId}' AND owner_id='{player.playerId}'");
                    client.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(2, player, item));
                }
            }
        }
        private void CupomIncreaseGold(Account p, int cupomId)
        {
            int gold = GetIdStatics(cupomId, 4) * 1000;
            gold += GetIdStatics(cupomId, 3) * 100;
            gold += GetIdStatics(cupomId, 2) * 1000000;
            if ((p.gold + gold) <= 999999999 && p.UpdateAccountGold(p.gold + gold))
            {
                p.gold += gold;
                client.SendPacket(new AUTH_GOLD_REWARD_PAK(gold, p.gold, 0));
            }
            else
            {
                client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_0x80000000_PAK);
                return;
            }
        }

        public int CreateItemId(int class1, int usage, int classtype, int number)
        {
            try
            {
                return (class1 * 100000000) + (usage * 1000000) + (classtype * 1000) + number;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return 0;
            }
        }

        /// <summary>
        /// Gera informações do Id de um item.
        /// </summary>
        /// <param name="weaponId">Id do item</param>
        /// <param name="type">Tipo de informação. 1 = Inicio (ITEM_CLASS); 2 = Usage; 3 = Meio (ClassType); 4 = Final (Number)</param>
        /// <returns></returns>
        public int GetIdStatics(int weaponId, int type)
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
            return 0;
        }
    }
}