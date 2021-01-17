using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_SHOP_BUY_ITEM_REQ : GamePacketReader
    {
        private List<CartGoods> ShopCart = new List<CartGoods>();
        private byte count;
        public override void ReadImplement()
        {
            count = ReadByte();
            for (byte i = 0; i < count; i++)
            {
                ShopCart.Add(new CartGoods
                {
                    GoodId = ReadInt(),
                    BuyType = ReadByte()
                });
            }
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.nickname.Length < Settings.NickMinLength || count == 0)
                {
                    Logger.Warning(" [PROTOCOL_SHOP_BUY_ITEM_REQ] Player is null or nickname is invalid. [!] IP: " + client.GetIPAddress());
                    client.SendCompletePacket(PackageDataManager.SHOP_BUY_2147487767_PAK);
                    return;
                }
                lock (player.inventory.items)
                {
                    if (player.inventory.items.Count >= 500)
                    {
                        client.SendCompletePacket(PackageDataManager.SHOP_BUY_2147487929_PAK);
                        return;
                    }
                    List<GoodItem> items = ShopManager.GetGoods(ShopCart, out int gold, out int cash);
                    if (items.Count == 0)
                    {
                        client.SendCompletePacket(PackageDataManager.SHOP_BUY_2147487767_PAK);
                    }
                    else if (0 > (player.gold - gold) || 0 > (player.cash - cash))
                    {
                        client.SendCompletePacket(PackageDataManager.SHOP_BUY_2147487768_PAK);
                    }
                    else if (player.UpdateAccountCashing(player.gold - gold, player.cash - cash))
                    {
                        player.gold -= gold;
                        player.cash -= cash;
                        client.SendPacket(new SHOP_BUY_PAK(1, items, player));
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.SHOP_BUY_2147487769_PAK);
                    }
                    items = null;
                }
                ShopCart = null;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}