using System;

namespace PointBlank.Game
{
    public class PROTOCOL_SHOP_LIST_REQ : GamePacketReader
    {
        private int error;
        public override void ReadImplement()
        {
            error = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || error != 0 && error != 44)
                {
                    return;
                }
                if (!player.loadedShop)
                {
                    player.loadedShop = true;
                    for (int i = 0; i < ShopManager.ShopDataItems.Count; i++)
                    {
                        client.SendPacket(new SHOP_GET_ITEMS_PAK(ShopManager.ShopDataItems[i], ShopManager.TotalItems));
                    }
                    for (int i = 0; i < ShopManager.ShopDataGoods.Count; i++)
                    {
                        client.SendPacket(new SHOP_GET_GOODS_PAK(ShopManager.ShopDataGoods[i], ShopManager.TotalGoods));
                    }
                    client.SendCompletePacket(PackageDataManager.SHOP_GET_REPAIR_PAK);
                    client.SendCompletePacket(PackageDataManager.SHOP_TEST2_PAK);
                    if (player.pccafe == 0)
                    {
                        for (int i = 0; i < ShopManager.ShopDataMatch.Count; i++)
                        {
                            client.SendPacket(new SHOP_GET_MATCHING_PAK(ShopManager.ShopDataMatch[i], ShopManager.TotalMatching1));
                        }
                    }
                    else/* if (player.pccafe == 1)*/
                    {
                        for (int i = 0; i < ShopManager.ShopPccafeBasic.Count; i++)
                        {
                            client.SendPacket(new SHOP_GET_MATCHING_PAK(ShopManager.ShopPccafeBasic[i], ShopManager.TotalMatching2));
                        }
                    }
                    //else if (player.pccafe == 2)
                    //{
                    //    for (int i = 0; i < ShopManager.ShopPccafePremium.Count; i++)
                    //    {
                    //        client.SendPacket(new SHOP_GET_MATCHING_PAK(ShopManager.ShopPccafePremium[i], ShopManager.TotalMatching3));
                    //    }
                    //}
                }
                client.SendPacket(new SHOP_LIST_PAK());
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}