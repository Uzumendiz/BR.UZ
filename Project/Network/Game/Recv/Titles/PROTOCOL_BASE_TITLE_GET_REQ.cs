using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_TITLE_GET_REQ : GamePacketReader
    {
        private int titleId;
        public override void ReadImplement()
        {
            titleId = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || titleId >= 45)
                {
                    return;
                }
                PlayerTitles titles = player.titles;
                TitleQ titleQ = TitlesManager.GetTitle(titleId);
                if (titleQ == null)
                {
                    client.SendCompletePacket(PackageDataManager.BASE_TITLE_GET_ERROR_PAK);
                    return;
                }
                TitlesManager.Get2Titles(titleQ.req1, titleQ.req2, out TitleQ titleReq1, out TitleQ titleReq2); 
                if ((titleQ.req1 == 0 || titleReq1 != null) && (titleQ.req2 == 0 || titleReq2 != null) && player.rankId >= titleQ.rank && player.brooch >= titleQ.brooch && player.medal >= titleQ.medals && player.blueorder >= titleQ.blueOrder && player.insignia >= titleQ.insignia && !titles.Contains(titleQ.flag) && titles.Contains(titleReq1.flag) && titles.Contains(titleReq2.flag))
                {
                    if (titles.Slots < titleQ.slot && player.UpdateTitleSlots(titleQ.slot))
                    {
                        titles.Slots = titleQ.slot;
                    }
                    if (player.UpdateTitlesFlags(titles.Add(titleQ.flag)) && player.UpdateTitleRequirements(player.brooch - titleQ.brooch, player.insignia - titleQ.insignia, player.medal - titleQ.medals, player.blueorder - titleQ.blueOrder))
                    {
                        player.brooch -= titleQ.brooch;
                        player.medal -= titleQ.medals;
                        player.blueorder -= titleQ.blueOrder;
                        player.insignia -= titleQ.insignia;
                        List<ItemsModel> items = TitlesManager.GetAwards(titleId);
                        if (items.Count > 0)
                        {
                            client.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, items));
                        }
                        client.SendPacket(new BASE_QUEST_UPDATE_INFO_PAK(player));
                        client.SendPacket(new BASE_TITLE_GET_PAK(0, titles.Slots));
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.BASE_TITLE_GET_ERROR_PAK);
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BASE_TITLE_GET_ERROR_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}