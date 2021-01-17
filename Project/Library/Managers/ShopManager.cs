using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

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
    public class ItemInfo
    {
        public string ItemName;
        public int GoodId;
        public int ItemId;
        public int PriceGold;
        public int PriceCash;
        public int Count;
        public string BuyType;
        public string Tag;
        public int Title;
        public string Visibility;
    }
    public static class ShopManager
    {
        public static List<GoodItem> ShopAllList = new List<GoodItem>();
        public static List<GoodItem> ShopBuyableList = new List<GoodItem>();
        public static ConcurrentDictionary<int, GoodItem> ShopUniqueList = new ConcurrentDictionary<int, GoodItem>();
        public static List<ShopData> ShopDataMatch = new List<ShopData>();
        public static List<ShopData> ShopPccafeBasic = new List<ShopData>();
        public static List<ShopData> ShopPccafePremium = new List<ShopData>();
        public static List<ShopData> ShopDataGoods = new List<ShopData>();
        public static List<ShopData> ShopDataItems = new List<ShopData>();
        public static int TotalGoods, TotalItems, TotalMatching1, TotalMatching2, TotalMatching3, set4p;

        public static void Load()
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(SQLManager.ConnectionString))
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "SELECT * FROM shop";
                    using (NpgsqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            GoodItem good = new GoodItem
                            {
                                id = data.GetInt32(0),
                                price_gold = data.GetInt32(3),
                                price_cash = data.GetInt32(4),
                                auth_type = data.GetInt32(6), //1 = unidade 2 = dias
                                buy_type2 = data.GetInt32(7),
                                buy_type3 = data.GetInt32(8),
                                tag = data.GetInt32(9),
                                title = data.GetInt32(10),//0= Sem titulo Id do Slot=requer titulo
                                visibility = data.GetInt32(11)
                            };
                            good.item.SetItemId(data.GetInt32(1));
                            good.item.name = data.GetString(2);
                            good.item.count = data.GetInt32(5);
                            ShopAllList.Add(good);
                            if (good.visibility != 2 && good.visibility != 4)
                            {
                                ShopBuyableList.Add(good);
                            }
                            if (!ShopUniqueList.ContainsKey(good.item.id) && good.auth_type > 0)
                            {
                                ShopUniqueList.TryAdd(good.item.id, good);
                                if (good.visibility == 4)
                                {
                                    set4p++;
                                }
                            }
                        }
                        LoadDataMatching1Goods();//Pccafe 0
                        LoadDataMatching2();//Pccafe basic/premium
                        LoadDataItems();
                        data.Close();
                        connection.Close();
                    }
                }
                if (set4p > 0)
                {
                    Logger.Informations($" [ShopManager] Loaded {set4p} itens invisíveis com ícones liberados.");
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            //XIEMIELE();
            //CreateJsonShop();
        }

        public static void XIEMIELE()
        {
            SortedList<int, ItemInfo> items = new SortedList<int, ItemInfo>();
            int counttt = 0;
            XmlTextWriter TW = new XmlTextWriter(Directory.GetCurrentDirectory() + "/UserFileList.xml", Encoding.UTF8);
            TW.WriteStartDocument();
            TW.Formatting = Formatting.Indented;
            TW.WriteStartElement("list");
            for (int i = 0; i < ShopAllList.Count; i++)
            {
                GoodItem good = ShopAllList[i];
                string tag = "NULL";
                if (good.tag == 0)
                {
                    tag = "DEFAULT";
                }
                else if (good.tag == 1)
                {
                    tag = "NEW";
                }
                else if (good.tag == 2)
                {
                    tag = "HOT";
                }
                else if (good.tag == 3)
                {
                    tag = "EVENT";
                }
                else if (good.tag == 4)
                {
                    tag = "PCCAFE";
                }
                else if (good.tag == 5)
                {
                    tag = "SALE";
                }
                else if (good.tag == 6)
                {
                    tag = "SET";
                }
                string visibleType = "NULL";
                if (good.visibility == 0)
                {
                    visibleType = "VISIVEL E ICONES";
                }
                else if (good.visibility == 1)
                {
                    visibleType = "SET PRIMARY";
                }
                else if (good.visibility == 2)
                {
                    visibleType = "INVISIVEL";
                }
                else if (good.visibility == 3)
                {
                    visibleType = "SET ITENS";
                }
                else if (good.visibility == 4)
                {
                    visibleType = "APENAS ICONES";
                }
                ItemInfo info = new ItemInfo();
                info.ItemName = good.item.name;
                info.GoodId = i;
                info.ItemId = good.item.id;
                info.PriceGold = good.price_gold;
                info.PriceCash = good.price_cash;
                if (good.auth_type == 0)
                {
                    if (good.item.count > 500)
                    {
                        info.Count = good.item.count / 60 / 60 / 24;
                    }
                    else
                    {
                        info.Count = good.item.count;
                    }
                    info.BuyType = "SET";
                }
                else if (good.auth_type == 1)
                {
                    info.Count = good.item.count;
                    info.BuyType = "UNIDADE";
                }
                else if (good.auth_type == 2)
                {
                    info.BuyType = "DIAS";
                    info.Count = good.item.count / 60 / 60 / 24;
                }
                info.Tag = tag;
                info.Title = good.title;
                info.Visibility = visibleType;

                if (!items.ContainsKey(info.ItemId))
                {
                    items.Add(info.ItemId, info);
                }
                counttt++;
            }
            foreach (ItemInfo info in items.Values)
            {
                if (info.BuyType == "DIAS")
                {
                    TW.WriteStartElement("file");
                    TW.WriteAttributeString("GoodId", "", info.GoodId.ToString());
                    TW.WriteAttributeString("PriceGold", "", info.PriceGold.ToString());
                    TW.WriteAttributeString("PriceCash", "", info.PriceCash.ToString());
                    TW.WriteAttributeString("BuyType", "", info.BuyType);
                    TW.WriteAttributeString("Count", "", info.Count.ToString());
                    TW.WriteAttributeString("Tag", "", info.Tag.ToString());
                    TW.WriteAttributeString("Title", "", info.Title.ToString());
                    TW.WriteAttributeString("Visibility", "", info.Visibility.ToString());
                    TW.WriteAttributeString("ItemId", "", info.ItemId.ToString());
                    TW.WriteAttributeString("ItemName", "", info.ItemName.ToString());
                    TW.WriteEndElement();
                }
                else
                {
                    TW.WriteStartElement("file");
                    TW.WriteAttributeString("GoodId", "", info.GoodId.ToString());
                    TW.WriteAttributeString("PriceGold", "", info.PriceGold.ToString());
                    TW.WriteAttributeString("PriceCash", "", info.PriceCash.ToString());
                    TW.WriteAttributeString("BuyType", "", info.BuyType);
                    TW.WriteAttributeString("Count", "", info.Count.ToString());
                    TW.WriteAttributeString("Tag", "", info.Tag.ToString());
                    TW.WriteAttributeString("Title", "", info.Title.ToString());
                    TW.WriteAttributeString("Visibility", "", info.Visibility.ToString());
                    TW.WriteAttributeString("ItemId", "", info.ItemId.ToString());
                    TW.WriteAttributeString("ItemName", "", info.ItemName.ToString());
                    TW.WriteEndElement();
                }
                TW.WriteEndElement();
                TW.Close();
            }
            Logger.Warning("counttt: " + counttt);
        }

        //public static void CreateJsonShop()
        //{
        //    List<ItemInfo> Shop = new List<ItemInfo>();
        //    for (int i = 0; i < ShopAllList.Count; i++)
        //    {
        //        GoodItem good = ShopAllList[i];
        //        string tag = "NULL";
        //        if (good.tag == 0)
        //        {
        //            tag = "DEFAULT";
        //        }
        //        else if (good.tag == 1)
        //        {
        //            tag = "NEW";
        //        }
        //        else if (good.tag == 2)
        //        {
        //            tag = "HOT";
        //        }
        //        else if (good.tag == 3)
        //        {
        //            tag = "EVENT";
        //        }
        //        else if (good.tag == 4)
        //        {
        //            tag = "PCCAFE";
        //        }
        //        else if (good.tag == 5)
        //        {
        //            tag = "SALE";
        //        }
        //        else if (good.tag == 6)
        //        {
        //            tag = "SET";
        //        }
        //        string visibleType = "NULL";
        //        if (good.visibility == 0)
        //        {
        //            visibleType = "VISIBLE & ICON";
        //        }
        //        else if (good.visibility == 1)
        //        {
        //            visibleType = "SET PRIMARY";
        //        }
        //        else if (good.visibility == 2)
        //        {
        //            visibleType = "INVISIBLE";
        //        }
        //        else if (good.visibility == 3)
        //        {
        //            visibleType = "SET ITENS";
        //        }
        //        else if (good.visibility == 4)
        //        {
        //            visibleType = "ONLY ICON";
        //        }
        //        ItemInfo info = new ItemInfo();
        //        info.ItemName = good.item.name;
        //        info.GoodId = i;
        //        info.ItemId = good.item.id;
        //        info.PriceGold = good.price_gold;
        //        info.PriceCash = good.price_cash;
        //        string buyType = "NULL";
        //        if (good.auth_type == 0)
        //        {
        //            buyType = "SET";
        //            if (good.item.count > 500)
        //            {
        //                uint dias = good.item.count / 60 / 60 / 24;
        //                info.Count = dias;
        //            }
        //            else
        //            {
        //                info.Count = good.item.count;
        //            }
        //            info.BuyType = buyType;
        //        }
        //        else if (good.auth_type == 1)
        //        {
        //            buyType = "UNITY";
        //            info.Count = good.item.count;
        //            info.BuyType = buyType;
        //        }
        //        else if (good.auth_type == 2)
        //        {
        //            buyType = "DAYS";
        //            uint dias = good.item.count / 60 / 60 / 24;
        //            info.BuyType = buyType;
        //            info.Count = dias;
        //        }
        //        info.Tag = tag;
        //        info.Title = good.title;
        //        info.Visibility = visibleType;
        //        Shop.Add(info);
        //    }

        //    JsonSerializer serializer = new JsonSerializer();
        //    serializer.Formatting = Formatting.Indented;
        //    using (StreamWriter SW = new StreamWriter("Data/Shop.json"))
        //    {
        //        using (JsonWriter JW = new JsonTextWriter(SW))
        //        {
        //            serializer.Serialize(JW, Shop);
        //        }
        //    }
        //    serializer = null;
        //}

        //public static void Load()
        //{
        //    using (StreamReader reader = new StreamReader("Data/Shop.json"))
        //    {
        //        List<ItemInfo> items = JsonConvert.DeserializeObject<List<ItemInfo>>(reader.ReadToEnd());
        //        for (int i = 0; i < items.Count; i++)
        //        {
        //            ItemInfo item = items[i];
        //            int tag = 0;
        //            if (item.Tag == "DEFAULT")
        //            {
        //                tag = 0;
        //            }
        //            else if (item.Tag == "NEW")
        //            {
        //                tag = 1;
        //            }
        //            else if (item.Tag == "HOT")
        //            {
        //                tag = 2;
        //            }
        //            else if (item.Tag == "EVENT")
        //            {
        //                tag = 3;
        //            }
        //            else if (item.Tag == "PCCAFE")
        //            {
        //                tag = 4;
        //            }
        //            else if (item.Tag == "SALE")
        //            {
        //                tag = 5;
        //            }
        //            else if (item.Tag == "SET")
        //            {
        //                tag = 6;
        //            }
        //            int visibleType = 0;
        //            if (item.Visibility == "VISIBLE & ICON")
        //            {
        //                visibleType = 0;
        //            }
        //            else if (item.Visibility == "SET PRIMARY")
        //            {
        //                visibleType = 1;
        //            }
        //            else if (item.Visibility == "INVISIBLE")
        //            {
        //                visibleType = 2;
        //            }
        //            else if (item.Visibility == "SET ITENS")
        //            {
        //                visibleType = 3;
        //            }
        //            else if (item.Visibility == "ONLY ICON")
        //            {
        //                visibleType = 4;
        //            }
        //            GoodItem good = new GoodItem();
        //            good.item.name = item.ItemName;
        //            good.id = item.GoodId;
        //            good.item.SetItemId(item.ItemId);
        //            good.price_gold = item.PriceGold;
        //            good.price_cash = item.PriceCash;
        //            good.buy_type2 = 1;
        //            good.buy_type3 = 2;
        //            int buyType = 2;
        //            if (item.BuyType == "UNITY")
        //            {
        //                buyType = 1;
        //                good.item.count = item.Count;
        //                good.auth_type = buyType;
        //            }
        //            else if (item.BuyType == "DAYS")
        //            {
        //                buyType = 2;
        //                uint dias = item.Count * 24 * 60 * 60;
        //                good.item.count = dias;
        //                good.auth_type = buyType;
        //            }
        //            else if (item.BuyType == "SET")
        //            {
        //                buyType = 0;
        //                uint dias = item.Count * 24 * 60 * 60;
        //                good.item.count = dias;
        //                good.auth_type = buyType;
        //            }
        //            good.tag = tag;
        //            good.title = item.Title;
        //            good.visibility = visibleType;
        //            ShopAllList.Add(good);
        //            if (good.visibility != 2 && good.visibility != 4)
        //            {
        //                ShopBuyableList.Add(good);
        //            }
        //            if (!ShopUniqueList.ContainsKey(good.item.id) && good.auth_type > 0)
        //            {
        //                ShopUniqueList.Add(good.item.id, good);
        //                if (good.visibility == 4)
        //                {
        //                    set4p++;
        //                }
        //            }
        //        }
        //        LoadDataMatching1Goods(0);//Pccafe
        //        LoadDataMatching2(1);//Pccafe premium
        //        LoadDataItems();
        //        if (set4p > 0)
        //        {
        //            Logger.Informations($" [ShopManager] Loaded {set4p} itens invisíveis com ícones liberados.");
        //        }
        //        items = null;
        //        reader.Close();
        //    }
        //    //Logger.Warning($"Name: {item.ItemName} Tag: {item.Tag} Visibility: {item.Visibility} BuyType: {item.BuyType} Count: {item.Count} PriceCash: {item.PriceCash} PriceGold: {item.PriceGold} Title: {item.Title} ItemId: {item.ItemId} GoodId: {item.GoodId}");
        //    //Logger.Warning($"Name: {good.item.name} Tag: {good.tag} Visibility: {good.visibility} BuyType: {good.auth_type} Count: {good.item.count} PriceCash: {good.price_cash} PriceGold: {good.price_gold} Title: {good.title} ItemId: {good.item.id} GoodId: {good.id} Category: {good.item.category}");
        //}

        public static void Reset()
        {
            set4p = 0;
            ShopAllList.Clear();
            ShopBuyableList.Clear();
            ShopUniqueList.Clear();
            ShopDataMatch.Clear();
            ShopPccafeBasic.Clear();
            ShopDataGoods.Clear();
            ShopDataItems.Clear();
            TotalGoods = 0;
            TotalItems = 0;
            TotalMatching1 = 0;
            TotalMatching2 = 0;
            TotalMatching3 = 0;
        }

        public static void LoadDataMatching1Goods()
        {
            int cafe = 0;
            List<GoodItem> matchs = new List<GoodItem>();
            List<GoodItem> goods = new List<GoodItem>();
            lock (ShopAllList)
            {
                foreach (GoodItem good in ShopAllList)
                {
                    if (good.item.count == 0)
                    {
                        continue;
                    }
                    if (!(good.tag == 4 && cafe == 0) && (good.tag == 4 && cafe > 0 || good.visibility != 2))
                    {
                        matchs.Add(good);
                    }
                    if (good.visibility < 2 || good.visibility == 4)
                    {
                        goods.Add(good);
                    }
                }
            }
            TotalMatching1 = matchs.Count;
            TotalGoods = goods.Count;
            int Pages = (int)Math.Ceiling(matchs.Count / 741d);
            int count = 0;
            for (int i = 0; i < Pages; i++)
            {
                byte[] buffer = GetMatchingData(741, i, ref count, matchs);
                ShopData data = new ShopData
                {
                    Buffer = buffer,
                    ItemsCount = count,
                    Offset = i * 741
                };
                ShopDataMatch.Add(data);
            }

            Pages = (int)Math.Ceiling(goods.Count / 592d);
            for (int i = 0; i < Pages; i++)
            {
                byte[] buffer = GetGoodsData(592, i, ref count, goods);
                ShopData data = new ShopData
                {
                    Buffer = buffer,
                    ItemsCount = count,
                    Offset = i * 592
                };
                ShopDataGoods.Add(data);
            }
        }
        public static void LoadDataMatching2()
        {
            int cafe = 1;
            List<GoodItem> basic = new List<GoodItem>();
            List<GoodItem> premium = new List<GoodItem>();
            lock (ShopAllList)
            {
                foreach (GoodItem good in ShopAllList)
                {
                    if (good.item.count == 0)
                    {
                        continue;
                    }
                    if (!(good.tag == 4 && cafe == 0) && (good.tag == 4 && cafe > 0 || good.visibility != 2))
                    {
                        basic.Add(good);
                    }
                    //if (good.tag == 6 && good.visibility != 2)
                    //{
                    //    premium.Add(good);
                    //}
                }
            }
            TotalMatching2 = basic.Count;
            int Pages = (int)Math.Ceiling(basic.Count / 741d);
            int count = 0;
            for (int i = 0; i < Pages; i++)
            {
                byte[] buffer = GetMatchingData(741, i, ref count, basic);
                ShopData data = new ShopData
                {
                    Buffer = buffer,
                    ItemsCount = count,
                    Offset = i * 741
                };
                ShopPccafeBasic.Add(data);
            }

            //TotalMatching3 = premium.Count;
            //Pages = (int)Math.Ceiling(premium.Count / 741d);
            //for (int i = 0; i < Pages; i++)
            //{
            //    byte[] buffer = GetMatchingData(741, i, ref count, premium);
            //    ShopData data = new ShopData
            //    {
            //        Buffer = buffer,
            //        ItemsCount = count,
            //        Offset = i * 741
            //    };
            //    ShopPccafePremium.Add(data);
            //}
        }
        public static void LoadDataItems()
        {
            List<GoodItem> items = new List<GoodItem>();
            lock (ShopUniqueList)
            {
                foreach (GoodItem good in ShopUniqueList.Values)
                {
                    if (good.visibility != 1 && good.visibility != 3)
                    {
                        items.Add(good);
                    }
                }
            }
            TotalItems = items.Count;
            int ItemsPages = (int)Math.Ceiling(items.Count / 1111d);
            int count = 0;
            for (int i = 0; i < ItemsPages; i++)
            {
                byte[] buffer = GetItemsData(1111, i, ref count, items);
                ShopData data = new ShopData
                {
                    Buffer = buffer,
                    ItemsCount = count,
                    Offset = i * 1111
                };
                ShopDataItems.Add(data);
            }
        }
        private static byte[] GetItemsData(int maximum, int page, ref int count, List<GoodItem> list)
        {
            count = 0;
            using (PacketWriter Writer = new PacketWriter())
            {
                for (int i = page * maximum; i < list.Count; i++)
                {
                    GoodItem good = list[i];
                    Writer.WriteD(good.item.id);
                    Writer.WriteC((byte)good.auth_type);
                    Writer.WriteC((byte)good.buy_type2);
                    Writer.WriteC((byte)good.buy_type3);
                    Writer.WriteC((byte)good.title);
                    if (++count == maximum)
                    {
                        break;
                    }
                }
                return Writer.memorystream.ToArray();
            }
        }
        private static byte[] GetGoodsData(int maximum, int page, ref int count, List<GoodItem> list)
        {
            count = 0;
            using (PacketWriter Writer = new PacketWriter())
            {
                for (int i = page * maximum; i < list.Count; i++)
                {
                    GoodItem good = list[i];
                    Writer.WriteD(good.id);
                    Writer.WriteC(1);
                    Writer.WriteC((byte)(good.visibility == 4 ? 4 : 1));
                    //Flag1 = Show icon + Buy option | Flag2 = UNK | Flag4 = Show icon + No buy option
                    Writer.WriteD(good.price_gold);
                    Writer.WriteD(good.price_cash);
                    Writer.WriteC((byte)good.tag);
                    if (++count == maximum)
                    {
                        break;
                    }
                }
                return Writer.memorystream.ToArray();
            }
        }
        private static byte[] GetMatchingData(int maximum, int page, ref int count, List<GoodItem> list)
        {
            count = 0;
            using (PacketWriter Writer = new PacketWriter())
            {
                for (int i = page * maximum; i < list.Count; i++)
                {
                    GoodItem good = list[i];
                    Writer.WriteD(good.id);
                    Writer.WriteD(good.item.id);
                    Writer.WriteD(good.item.count);
                    if (++count == maximum)
                    {
                        break;
                    }
                }
                return Writer.memorystream.ToArray();
            }
        }

        public static GoodItem GetGood(int goodId)
        {
            if (goodId == 0)
            {
                return null;
            }
            lock (ShopAllList)
            {
                try
                {
                    foreach(GoodItem good in ShopAllList)
                    {
                        if (good.id == goodId)
                        {
                            return good;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
                return null;
            }
        }
        public static List<GoodItem> GetGoods(List<CartGoods> ShopCart, out int GoldPrice, out int CashPrice)
        {
            GoldPrice = 0;
            CashPrice = 0;
            List<GoodItem> items = new List<GoodItem>();
            if (ShopCart.Count == 0)
            {
                return items;
            }
            lock (ShopBuyableList)
            {
                for (int i = 0; i < ShopBuyableList.Count; i++)
                {
                    GoodItem good = ShopBuyableList[i];
                    for (int i2 = 0; i2 < ShopCart.Count; i2++)
                    {
                        CartGoods CartGood = ShopCart[i2];
                        if (CartGood.GoodId == good.id)
                        {
                            items.Add(good);
                            if (CartGood.BuyType == 1)
                            {
                                GoldPrice += good.price_gold;
                            }
                            else if (CartGood.BuyType == 2)
                            {
                                CashPrice += good.price_cash;
                            }
                        }
                    }
                }
            }
            return items;
        }
    }
    public class CartGoods
    {
        public int GoodId, BuyType;
    }
    public class ShopData
    {
        public byte[] Buffer;
        public int ItemsCount, Offset;
    }
}