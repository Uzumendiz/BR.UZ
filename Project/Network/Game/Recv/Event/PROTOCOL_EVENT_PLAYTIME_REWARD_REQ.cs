using System;

namespace PointBlank.Game
{
    public class PROTOCOL_EVENT_PLAYTIME_REWARD_REQ : GamePacketReader
    {
        private int goodId;
        public override void ReadImplement()
        {
            goodId = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.events == null || player.checkEventPlayTimeReward)
                {
                    return;
                }
                GoodItem good = ShopManager.GetGood(goodId);
                if (good == null)
                {
                    return;
                }
                PlayTimeModel eventPlayTime = EventPlayTimeSyncer.GetRunningEvent();
                if (eventPlayTime != null)
                {
                    int count = eventPlayTime.GetRewardCount(goodId);
                    if (player.events.LastPlaytimeFinish == 1 && count > 0 && player.UpdatePlayTimeReward())
                    {
                        player.events.LastPlaytimeFinish = 2;
                        client.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, new ItemsModel(good.item.id, good.item.category, "Playtime reward", good.item.equip, count)));
                    }
                    player.checkEventPlayTimeReward = true;
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}