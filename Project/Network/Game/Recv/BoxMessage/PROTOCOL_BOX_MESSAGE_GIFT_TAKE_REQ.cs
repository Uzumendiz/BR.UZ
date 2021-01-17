using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BOX_MESSAGE_GIFT_TAKE_REQ : GamePacketReader
    {
        private int objectId;
        public override void ReadImplement()
        {
            objectId = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null)
                {
                    return;
                }
                if (player.inventory.items.Count >= 500)
                {
                    client.SendCompletePacket(PackageDataManager.INVENTORY_ITEM_EQUIP_2147487785_PAK);
                    client.SendCompletePacket(PackageDataManager.BOX_MESSAGE_GIFT_TAKE_0x80000000_PAK);
                }
                else
                {
                    Message msg = player.GetMessage(objectId);
                    if (msg != null && msg.type == 2)
                    {
                        GoodItem good = ShopManager.GetGood((int)msg.senderId);
                        if (good != null)
                        {
                            Logger.Warning($" [BOX_MESSAGE_GIFT_TAKE_REQ] Received gift. [Good: {good.id} Item: {good.item.id}]");
                            client.SendPacket(new BOX_MESSAGE_GIFT_TAKE_PAK(1, good.item, player));
                            player.DeleteMessage(objectId);
                        }
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.BOX_MESSAGE_GIFT_TAKE_0x80000000_PAK);
                    }
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}