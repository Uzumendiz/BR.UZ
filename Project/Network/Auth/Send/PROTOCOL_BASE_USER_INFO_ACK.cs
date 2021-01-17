using System;
using System.Collections.Generic;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_INFO_ACK : GamePacketWriter
    {
        private List<ItemsModel> charas = new List<ItemsModel>();
        private List<ItemsModel> weapons = new List<ItemsModel>();
        private List<ItemsModel> cupons = new List<ItemsModel>();
        private Account player;
        private Clan clan;
        private uint error;
        private bool xmas;
        private int date;
        public PROTOCOL_BASE_USER_INFO_ACK(Account player, Clan clan, int date, uint error)
        {
            this.player = player;
            this.clan = clan;
            this.date = date;
            this.error = error;
        }

        private void GetInventoryInfo()
        {
            lock (player.inventory.items)
            {
                for (int i = 0; i < player.inventory.items.Count; i++)
                {
                    ItemsModel item = player.inventory.items[i];
                    if (item.category == 1)
                    {
                        weapons.Add(item);
                    }
                    else if (item.category == 2)
                    {
                        charas.Add(item);
                    }
                    else if (item.category == 3)
                    {
                        cupons.Add(item);
                    }
                }
            }
        }

        private void CheckGameEvents(EventVisitModel eventVisit)
        {
            PlayerEvent playerEvent = player.events;
            if (playerEvent != null)
            {
                QuestModel eventQuest = EventQuestSyncer.GetRunningEvent();
                if (eventQuest != null)
                {
                    long lastDate = playerEvent.LastQuestDate;
                    int finish = playerEvent.LastQuestFinish;
                    if (playerEvent.LastQuestDate < eventQuest.startDate)
                    {
                        playerEvent.LastQuestDate = 0;
                        playerEvent.LastQuestFinish = 0;
                        player.SendCompletePacket(PackageDataManager.SERVER_MESSAGE_EVENT_QUEST_PAK);
                    }
                    if (playerEvent.LastQuestFinish == 0)
                    {
                        player.missions.mission4 = 13; //13 = MissionId Event
                        if (playerEvent.LastQuestDate == 0)
                        {
                            playerEvent.LastQuestDate = date;
                        }
                    }
                    if (playerEvent.LastQuestDate != lastDate || playerEvent.LastQuestFinish != finish)
                    {
                        player.ExecuteQuery($"UPDATE player_events SET last_quest_date='{playerEvent.LastQuestDate}', last_quest_finish='{playerEvent.LastQuestFinish}' WHERE player_id='{player.playerId}'");
                    }
                }
                EventLoginModel eventLogin = EventLoginSyncer.GetRunningEvent();
                if (eventLogin != null && !(eventLogin.startDate < playerEvent.LastLoginDate && playerEvent.LastLoginDate < eventLogin.endDate))
                {
                    ItemsModel item = new ItemsModel(eventLogin.rewardId, eventLogin.category, "Login event", 1, eventLogin.count);
                    player.TryCreateItem(item);
                    player.SendCompletePacket(PackageDataManager.AUTH_SERVER_MESSAGE_ITEM_RECEIVE_PAK);
                    if (item.category == 1)
                    {
                        weapons.Add(item);
                    }
                    else if (item.category == 2)
                    {
                        charas.Add(item);
                    }
                    else if (item.category == 3)
                    {
                        cupons.Add(item);
                    }
                    long dateNow = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                    player.ExecuteQuery($"UPDATE player_events SET last_login_date='{dateNow}' WHERE player_id='{player.playerId}'");
                }
                if (eventVisit != null && playerEvent.LastVisitEventId != eventVisit.id)
                {
                    playerEvent.LastVisitEventId = eventVisit.id;
                    playerEvent.LastVisitSequence1 = 0;
                    playerEvent.LastVisitSequence2 = 0;
                    playerEvent.NextVisitDate = 0;
                    player.ExecuteQuery($"UPDATE player_events SET last_visit_event_id='{eventVisit.id}', last_visit_sequence1='0', last_visit_sequence2='0', next_visit_date='0' WHERE player_id='{player.playerId}'");
                }
                EventXmasModel eventXMAS = EventXmasSyncer.GetRunningEvent();
                if (eventXMAS != null)
                {
                    if (playerEvent.LastXmasRewardDate < eventXMAS.startDate)
                    {
                        playerEvent.LastXmasRewardDate = 0;
                        player.ExecuteQuery($"UPDATE player_events SET last_xmas_reward_date='0' WHERE player_id='{player.playerId}'");
                    }
                    if (!(playerEvent.LastXmasRewardDate > eventXMAS.startDate && playerEvent.LastXmasRewardDate <= eventXMAS.endDate))
                    {
                        xmas = true;
                    }
                }
            }
        }

        public override void Write()
        {
            WriteH(2566);
            WriteD(error);
            if (error != 0)
            {
                return;
            }
            EventVisitModel eventVisit = EventVisitSyncer.GetRunningEvent();
            WriteC(player.age);
            WriteS(player.nickname, 33);
            WriteD(player.exp);
            WriteD(player.rankId);
            WriteD(player.rankId);
            WriteD(player.gold);
            WriteD(player.cash);
            WriteD(clan.id);
            WriteD((int)player.clanAuthority);
            WriteD(player.nickname == "" ? 1 : 0);
            WriteD(0);
            WriteC(player.pccafe);
            WriteC(player.tourneyLevel);
            WriteC(player.nickcolor);
            WriteS(clan.name, 17);
            WriteC(clan.rank);
            WriteC(clan.GetClanUnit());
            WriteD(clan.logo);
            WriteC(clan.nameColor);
            WriteD(10000);
            WriteC(0);
            WriteD(0);
            WriteD(player.lastRankUpDate); //109 BYTES
            WriteD(player.statistics.fights);
            WriteD(player.statistics.fightsWin);
            WriteD(player.statistics.fightsLost);
            WriteD(player.statistics.fightsDraw);
            WriteD(player.statistics.kills);
            WriteD(player.statistics.headshots);
            WriteD(player.statistics.deaths);
            WriteD(player.statistics.totalfights);
            WriteD(player.statistics.totalkills);
            WriteD(player.statistics.escapes);
            WriteD(player.statistics.fights);
            WriteD(player.statistics.fightsWin);
            WriteD(player.statistics.fightsLost);
            WriteD(player.statistics.fightsDraw);
            WriteD(player.statistics.kills);
            WriteD(player.statistics.headshots);
            WriteD(player.statistics.deaths);
            WriteD(player.statistics.totalfights);
            WriteD(player.statistics.totalkills);
            WriteD(player.statistics.escapes);
            WriteD(player.equipments.red);
            WriteD(player.equipments.blue);
            WriteD(player.equipments.helmet);
            WriteD(player.equipments.beret);
            WriteD(player.equipments.dino);
            WriteD(player.equipments.primary);
            WriteD(player.equipments.secondary);
            WriteD(player.equipments.melee);
            WriteD(player.equipments.grenade);
            WriteD(player.equipments.special);
            WriteH(0); //Auxiliar de cor da mira
            WriteD(player.bonus.fakeRank);
            WriteD(player.bonus.fakeRank);
            WriteS(player.bonus.fakeNick, 33);
            WriteH(player.bonus.sightColor);
            WriteC(player.country);
            CheckGameEvents(eventVisit);
            if (Settings.ClientVersion == "1.15.23" || Settings.ClientVersion == "1.15.37")
            {
                GetInventoryInfo();
                WriteC(Settings.InventoryActive);
                WriteD(charas.Count);
                WriteD(weapons.Count);
                WriteD(cupons.Count);
                WriteD(0);
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
            }
            WriteC(Settings.OutpostActive);
            WriteD(player.brooch);
            WriteD(player.insignia);
            WriteD(player.medal);
            WriteD(player.blueorder);
            WriteC(player.missions.actualMission);
            WriteC(player.missions.card1);
            WriteC(player.missions.card2);
            WriteC(player.missions.card3);
            WriteC(player.missions.card4);
            WriteB(player.GetCardFlags(player.missions.mission1, player.missions.list1));
            WriteB(player.GetCardFlags(player.missions.mission2, player.missions.list2));
            WriteB(player.GetCardFlags(player.missions.mission3, player.missions.list3));
            WriteB(player.GetCardFlags(player.missions.mission4, player.missions.list4));
            WriteC(player.missions.mission1);
            WriteC(player.missions.mission2);
            WriteC(player.missions.mission3);
            WriteC(player.missions.mission4);
            WriteB(player.missions.list1);
            WriteB(player.missions.list2);
            WriteB(player.missions.list3);
            WriteB(player.missions.list4);
            WriteQ(player.titles.Flags);
            WriteC(player.titles.Equiped1);
            WriteC(player.titles.Equiped2);
            WriteC(player.titles.Equiped3);
            WriteD(player.titles.Slots);
            WriteD(44); //Tutorial
            WriteD(1); //Deathmatch
            WriteD(25); //Destruction
            WriteD(35); //Sabotage
            WriteD(11); //Supression
            WriteD(39); //Defense
            WriteD(1); //Challenge
            WriteD(40); //Dinosaur
            WriteD(1); //Sniper
            WriteD(1); //Shotgun
            WriteD(0); //HeadHunter
            WriteD(0); //Knuckle
            WriteD(54); //CrossCounter
            WriteD(1); //Chaos
            if (Settings.ClientVersion == "1.15.38" || Settings.ClientVersion == "1.15.39" || Settings.ClientVersion == "1.15.41" || Settings.ClientVersion == "1.15.42")
            {
                WriteD(1); //TheifMode
            }
            WriteC((byte)MapsXML.ModeList.Count); //124 maps ver 42

            WriteC(4); //(Flag pages | 4 bytes)
            WriteD(MapsXML.maps1);
            WriteD(MapsXML.maps2);
            WriteD(MapsXML.maps3);
            WriteD(MapsXML.maps4);
            WriteB(MapsXML.ModeBytes);
            WriteB(MapsXML.TagBytes);

            WriteC(Settings.MissionActive); //Pages Count
            WriteD(MissionsXML.missionPage1);
            WriteD(0); //Multiplicado por 100?
            WriteD(0); //Multiplicado por 100?
            WriteC(0);
            WriteH(20); //length de algo.
            //WriteB(new byte[20] { 0x70, 0x0C, 0x94, 0x2D, 0x48, 0x08, 0xDD, 0x1E, 0xB0, 0xAB, 0x1A, 0x00, 0x99, 0x7B, 0x42, 0x00, 0x70, 0x0C, 0x94, 0x2D });
            WriteB(new byte[20]);
            WriteD(player.IsGM() || player.HaveAcessLevel());
            WriteD(xmas);
            WriteC(1); //Repair?

            WriteVisitEvent(eventVisit);

            WriteD(int.Parse(DateTime.Now.ToString("yyMMddHHmm"))); //DataNow By Server

            WriteS("10.120.1.44", 256);
            WriteH(0); //8085
            WriteH(0);

            WriteC(0); //Presentes
            WriteH(1); //1
            WriteC(0);

            WriteC(1);
            WriteC(7); //6

            WriteC(4); //Vip
            WriteC(1); //Posição do item VIP na loja
            WriteC(1);
            WriteC(2);
            WriteC(5);
            WriteC(3);
            WriteC(6);

            charas = null;
            weapons = null;
            cupons = null;
        }

        private void WriteVisitEvent(EventVisitModel eventVisit)
        {
            PlayerEvent playerEvent = player.events;
            if (eventVisit != null && (playerEvent.LastVisitSequence1 < eventVisit.checks && playerEvent.NextVisitDate <= int.Parse(DateTime.Now.ToString("yyMMdd")) || playerEvent.LastVisitSequence2 < eventVisit.checks && playerEvent.LastVisitSequence2 != playerEvent.LastVisitSequence1))
            {
                WriteD(eventVisit.id);
                WriteC(playerEvent.LastVisitSequence1);
                WriteC(playerEvent.LastVisitSequence2);
                WriteB(EventVisitSyncer.MyinfoBytes);
            }
            else
            {
                WriteB(new byte[Settings.ClientVersion == "1.15.39" || Settings.ClientVersion == "1.15.41" || Settings.ClientVersion == "1.15.42" ? 181 : 172]);
            }
        }
    }
}