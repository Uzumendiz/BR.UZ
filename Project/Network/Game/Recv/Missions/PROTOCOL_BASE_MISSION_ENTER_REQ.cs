using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_MISSION_ENTER_REQ : GamePacketReader
    {
        private byte cardIdx;
        private byte actualMission;
        private ushort cardFlags;
        public override void ReadImplement()
        {
            actualMission = ReadByte(); //Baralho Atual (0-3) (4 BARALHOS CONTANDO O TUTORIAL)
            cardIdx = ReadByte(); //Carta do Baralho Selecinada (0-9) (10 CARTAS DO BARALHO)
            cardFlags = ReadUshort();
        }

        public override void RunImplement()
        {
            try
            {
                Logger.DebugPacket(GetType().Name, $"actualMission: {actualMission} cardIdx: {cardIdx} cardFlags: {cardFlags}");
                Account player = client.SessionPlayer;
                if (player == null || actualMission > 3 || cardIdx > 9)
                {
                    return;
                }
                PlayerMissions missions = player.missions;
                using (DBQuery query = new DBQuery())
                {
                    if (missions.GetCard(actualMission) != cardIdx)
                    {
                        if (actualMission == 0)
                        {
                            missions.card1 = cardIdx;
                        }
                        else if (actualMission == 1)
                        {
                            missions.card2 = cardIdx;
                        }
                        else if (actualMission == 2)
                        {
                            missions.card3 = cardIdx;
                        }
                        else if (actualMission == 3)
                        {
                            missions.card4 = cardIdx;
                        }
                        query.AddQuery($"card{actualMission + 1}", cardIdx);
                    }
                    missions.selectedCard = cardFlags == 65535;
                    if (missions.actualMission != actualMission)
                    {
                        query.AddQuery("actual_mission", actualMission);
                        missions.actualMission = actualMission;
                    }
                    Utilities.UpdateDB("player_missions", "owner_id", player.playerId, query.GetTables(), query.GetValues());
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}