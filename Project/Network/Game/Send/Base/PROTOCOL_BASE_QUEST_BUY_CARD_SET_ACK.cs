﻿namespace PointBlank.Game
{
    public class BASE_QUEST_BUY_CARD_SET_PAK : GamePacketWriter
    {
        private Account player;
        private uint error;
        public BASE_QUEST_BUY_CARD_SET_PAK(uint error, Account player)
        {
            this.error = error;
            this.player = player;
        }

        public override void Write()
        {
            WriteH(2606);
            WriteD(error);
            if (error == 0)
            {
                WriteD(player.gold);
                WriteC(player.missions.actualMission);
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
            }
        }
    }
}