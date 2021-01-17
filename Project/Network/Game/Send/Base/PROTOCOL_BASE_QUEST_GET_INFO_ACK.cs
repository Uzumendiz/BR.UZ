namespace PointBlank.Game
{
    public class PROTOCOL_BASE_QUEST_GET_INFO_ACK : GamePacketWriter
    {
        private Account player;
        public PROTOCOL_BASE_QUEST_GET_INFO_ACK(Account player)
        {
            this.player = player;
        }

        public override void Write()
        {
            WriteH(2596);
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