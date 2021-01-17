using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_QUEST_BUY_CARD_SET_REQ : GamePacketReader
    {
        private byte missionId;
        public override void ReadImplement()
        {
            missionId = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                PlayerMissions missions = player != null ? player.missions : null;
                int price = MissionsXML.GetMissionPrice(missionId);
                if (player == null || price == 1 || 0 > player.gold - price || missions.mission1 == missionId || missions.mission2 == missionId || missions.mission3 == missionId)
                {
                    if (price == 1)
                    {
                        client.SendCompletePacket(PackageDataManager.BASE_QUEST_BUY_CARD_SET_0x8000104C_PAK);
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.BASE_QUEST_BUY_CARD_SET_0x8000104D_PAK);
                    }
                }
                else
                {
                    if (missions.mission1 == 0)
                    {
                        if (player.UpdateMissionId(missionId, 0))
                        {
                            missions.mission1 = missionId;
                            missions.list1 = new byte[40];
                            missions.actualMission = 0;
                            missions.card1 = 0;
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.BASE_QUEST_BUY_CARD_SET_0x8000104C_PAK);
                            return;
                        }
                    }
                    else if (missions.mission2 == 0)
                    {
                        if (player.UpdateMissionId(missionId, 1))
                        {
                            missions.mission2 = missionId;
                            missions.list2 = new byte[40];
                            missions.actualMission = 1;
                            missions.card2 = 0;
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.BASE_QUEST_BUY_CARD_SET_0x8000104C_PAK);
                            return;
                        }
                    }
                    else if (missions.mission3 == 0)
                    {
                        if (player.UpdateMissionId(missionId, 2))
                        {
                            missions.mission3 = missionId;
                            missions.list3 = new byte[40];
                            missions.actualMission = 2;
                            missions.card3 = 0;
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.BASE_QUEST_BUY_CARD_SET_0x8000104C_PAK);
                            return;
                        }
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.BASE_QUEST_BUY_CARD_SET_0x8000104E_PAK);
                        return;
                    }
                    if (price == 0 || player.UpdateAccountGold(player.gold - price))
                    {
                        player.gold -= price;
                        client.SendPacket(new BASE_QUEST_BUY_CARD_SET_PAK(0, player));
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.BASE_QUEST_BUY_CARD_SET_0x8000104C_PAK);
                    }
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
            /*
             * 0x8000104C STR_TBL_GUI_BASE_FAIL_BUY_CARD_BY_NO_CARD_INFO
             * 0x8000104D STR_TBL_GUI_BASE_NO_POINT_TO_GET_ITEM
             * 0x8000104E STR_TBL_GUI_BASE_LIMIT_CARD_COUNT
             * 0x800010D5 STR_TBL_GUI_BASE_DID_NOT_TUTORIAL_MISSION_CARD
             */
        }
    }
}