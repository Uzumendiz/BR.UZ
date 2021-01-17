using System;
using System.Collections.Generic;
using System.Globalization;

namespace PointBlank.Game
{
    public class SHOP_BUY_PAK : GamePacketWriter
    {
        private List<ItemsModel> weapons = new List<ItemsModel>();
        private List<ItemsModel> charas = new List<ItemsModel>();
        private List<ItemsModel> cupons = new List<ItemsModel>();
        private Account player;
        private uint error;
        public SHOP_BUY_PAK(uint error, List<GoodItem> item = null, Account player = null)
        {
            this.error = error;
            if (error == 1)
            {
                this.player = player;
                AddItems(item);
            }
        }
        public override void Write()
        {
            WriteH(531);
            WriteD(error);
            if (error == 1)
            {
                WriteD(int.Parse(DateTime.Now.ToString("yyMMddHHmm")));
                WriteD(charas.Count);
                WriteD(weapons.Count);
                WriteD(cupons.Count);
                for (int i = 0; i < charas.Count; i++)
                {
                    ItemsModel item = charas[i];
                    WriteQ(item.objectId);
                    WriteD(item.id);
                    WriteC(item.equip);
                    WriteD(item.count);
                }
                for (int i = 0; i < weapons.Count; i++)
                {
                    ItemsModel item = weapons[i];
                    WriteQ(item.objectId);
                    WriteD(item.id);
                    WriteC(item.equip);
                    WriteD(item.count);
                }
                for (int i = 0; i < cupons.Count; i++)
                {
                    ItemsModel item = cupons[i];
                    WriteQ(item.objectId);
                    WriteD(item.id);
                    WriteC(item.equip);
                    WriteD(item.count);
                }
                WriteD(player.gold);
                WriteD(player.cash);
            }
            charas = null;
            weapons = null;
            cupons = null;
        }
        private void AddItems(List<GoodItem> items)
        {
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    GoodItem good = items[i];
                    ItemsModel iv = player.inventory.GetItem(good.item.id);
                    ItemsModel modelo = new ItemsModel(good.item);
                    if (iv == null)
                    {
                        if (player.CreateItem(modelo))
                        {
                            player.inventory.AddItem(modelo);
                        }
                        else
                        {
                            error = 2147487767;
                            break;
                        }
                    }
                    else
                    {
                        modelo.count = iv.count;
                        modelo.objectId = iv.objectId;
                        if (iv.equip == 1)
                        {
                            if ((good.item.count + modelo.count) > Settings.MaxBuyItemUnits)
                            {
                                Logger.Warning($" [GAME] [{GetType().Name}] Não foi possivel comprar mais de {Settings.MaxBuyItemUnits} unidades do mesmo equipamento. ItemId: {iv.id} PlayerId: {player.playerId} Date: {DateTime.Now}");
                                error = 2147487767;
                                break;
                            }
                            modelo.count += good.item.count;
                            if (!player.ExecuteQuery($"UPDATE player_items SET count='{modelo.count}' WHERE owner_id='{player.playerId}' AND item_id='{modelo.id}'"))
                            {
                                error = 2147487767;
                                break;
                            }
                        }
                        else if (iv.equip == 2 && modelo.category != 3)
                        {
                            DateTime data = DateTime.ParseExact(iv.count.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture).AddSeconds(good.item.count);
                            if ((data - DateTime.Now).Days + 1 > Settings.MaxBuyItemDays) //+1 porque ele não conta o dia atual da compra neste calculo, no jogo compra 30 dias, aqui mostra 29 e somo mais 1
                            {
                                Logger.Warning($" [GAME] [{GetType().Name}] Não foi possivel comprar mais de {Settings.MaxBuyItemDays} dias do mesmo equipamento. ItemId: {iv.id} PlayerId: {player.playerId} Date: {DateTime.Now}");
                                error = 2147487767;
                                break;
                            }
                            modelo.count = int.Parse(data.ToString("yyMMddHHmm"));
                            if (!player.ExecuteQuery($"UPDATE player_items SET count='{modelo.count}' WHERE owner_id='{player.playerId}' AND item_id='{modelo.id}'"))
                            {
                                error = 2147487767;
                                break;
                            }
                        }
                        modelo.equip = iv.equip;
                        iv.count = modelo.count;
                    }
                    if (modelo.category == 1)
                    {
                        weapons.Add(modelo);
                    }
                    else if (modelo.category == 2)
                    {
                        charas.Add(modelo);
                    }
                    else if (modelo.category == 3)
                    {
                        cupons.Add(modelo);
                    }
                }
            }
            catch (Exception ex)
            {
                error = 2147487767;
                Logger.Exception(ex);
            }
        }
    }
}