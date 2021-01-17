using System;

namespace PointBlank.Game
{
    public class PROTOCOL_EVENT_VISIT_CONFIRM_REQ : GamePacketReader
    {
        private int eventId;
        public override void ReadImplement()
        {
            eventId = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.events == null || string.IsNullOrEmpty(player.nickname) || player.nickname.Length == 0 || player.checkEventVisitConfirm)
                {
                    client.SendCompletePacket(PackageDataManager.EVENT_VISIT_CONFIRM_0x80001500_PAK);
                }
                else
                {
                    int dateNow = int.Parse(DateTime.Now.ToString("yyMMdd"));
                    if (player.events.NextVisitDate <= dateNow)
                    {
                        EventVisitModel eventVisit = EventVisitSyncer.GetEvent(eventId);
                        if (eventVisit != null)
                        {
                            if (eventVisit.EventIsEnabled())
                            {
                                player.events.NextVisitDate = int.Parse(DateTime.Now.AddDays(1).ToString("yyMMdd"));
                                if (player.ExecuteQuery($"UPDATE player_events SET next_visit_date='{player.events.NextVisitDate}', last_visit_sequence1='{player.events.LastVisitSequence1++}' WHERE player_id='{player.playerId}'"))
                                {
                                    bool IsReward = false;
                                    try
                                    {
                                        IsReward = eventVisit.box[player.events.LastVisitSequence2].reward1.IsReward;
                                    }
                                    catch
                                    {
                                    }
                                    if (!IsReward)
                                    {
                                        player.ExecuteQuery($"UPDATE player_events SET last_visit_sequence2='{player.events.LastVisitSequence2++}' WHERE player_id='{player.playerId}'");
                                    }
                                    client.SendPacket(new EVENT_VISIT_CONFIRM_PAK(EventErrorEnum.VisitEvent_Success, eventVisit, player.events));
                                }
                                else
                                {
                                    client.SendCompletePacket(PackageDataManager.EVENT_VISIT_CONFIRM_0x80001505_PAK);
                                }
                            }
                            else
                            {
                                client.SendCompletePacket(PackageDataManager.EVENT_VISIT_CONFIRM_0x80001503_PAK);
                            }
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.EVENT_VISIT_CONFIRM_0x80001505_PAK);
                        }
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.EVENT_VISIT_CONFIRM_0x80001502_PAK);
                    }
                    player.checkEventVisitConfirm = true;
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}