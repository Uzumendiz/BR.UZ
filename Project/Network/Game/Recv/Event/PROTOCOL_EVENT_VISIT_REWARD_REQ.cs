using System;

namespace PointBlank.Game
{
    public class PROTOCOL_EVENT_VISIT_REWARD_REQ : GamePacketReader
    {
        private int eventId;
        private int type;
        public override void ReadImplement()
        {
            eventId = ReadInt();
            type = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.nickname.Length == 0 || player.checkEventVisitReward || type > 1)
                {
                    client.SendCompletePacket(PackageDataManager.EVENT_VISIT_REWARD_ERROR_USERFAIL_PAK);
                }
                else if (player.events != null)
                {
                    if (player.events.LastVisitSequence1 == player.events.LastVisitSequence2)
                    {
                        client.SendCompletePacket(PackageDataManager.EVENT_VISIT_REWARD_ERROR_ALREADYCHECK_PAK);
                    }
                    else
                    {
                        EventVisitModel eventv = EventVisitSyncer.GetEvent(eventId);
                        if (eventv == null)
                        {
                            client.SendCompletePacket(PackageDataManager.EVENT_VISIT_REWARD_ERROR_UNKNOWN_PAK);
                            return;
                        }
                        if (eventv.EventIsEnabled())
                        {
                            VisitItem chI = eventv.GetReward(player.events.LastVisitSequence2, type);
                            if (chI != null)
                            {
                                GoodItem good = ShopManager.GetGood(chI.goodId);
                                if (good != null)
                                {
                                    player.events.NextVisitDate = int.Parse(DateTime.Now.AddDays(1).ToString("yyMMdd"));
                                    if (player.ExecuteQuery($"UPDATE player_events SET next_visit_date='{player.events.NextVisitDate}', last_visit_sequence2='{player.events.LastVisitSequence2++}' WHERE player_id='{player.playerId}'"))
                                    {
                                        client.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, new ItemsModel(good.item.id, good.item.category, good.item.name, good.item.equip, chI.count)));
                                        client.SendCompletePacket(PackageDataManager.EVENT_VISIT_REWARD_SUCCESS_PAK);
                                    }
                                    else
                                    {
                                        client.SendCompletePacket(PackageDataManager.EVENT_VISIT_REWARD_ERROR_UNKNOWN_PAK);
                                    }
                                }
                                else
                                {
                                    client.SendCompletePacket(PackageDataManager.EVENT_VISIT_REWARD_ERROR_NOTENOUGH_PAK);
                                }
                            }
                            else
                            {
                                client.SendCompletePacket(PackageDataManager.EVENT_VISIT_REWARD_ERROR_UNKNOWN_PAK);
                            }
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.EVENT_VISIT_REWARD_ERROR_WRONGVERSION_PAK);
                        }
                    }
                    player.checkEventVisitReward = true;
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.EVENT_VISIT_REWARD_ERROR_UNKNOWN_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}