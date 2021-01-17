using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_QUEST_DELETE_CARD_SET_REQ : GamePacketReader
    {
        private int missionIdx;
        public override void ReadImplement()
        {
            missionIdx = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                PlayerMissions missions = player != null ? player.missions : null;
                bool failed = false;
                if (missions == null || missionIdx >= 3 || (missionIdx == 0 && missions.mission1 == 0) || (missionIdx == 1 && missions.mission2 == 0) || (missionIdx == 2 && missions.mission3 == 0))
                {
                    failed = true;
                }
                if (!failed && player.UpdateMissionId(0, missionIdx) && player.UpdateCardDelete(missionIdx + 1))
                {
                    if (missionIdx == 0)
                    {
                        missions.mission1 = 0;
                        missions.card1 = 0;
                        missions.list1 = new byte[40];
                    }
                    else if (missionIdx == 1)
                    {
                        missions.mission2 = 0;
                        missions.card2 = 0;
                        missions.list2 = new byte[40];
                    }
                    else if (missionIdx == 2)
                    {
                        missions.mission3 = 0;
                        missions.card3 = 0;
                        missions.list3 = new byte[40];
                    }
                    client.SendPacket(new BASE_QUEST_DELETE_CARD_SET_PAK(player, 0));
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BASE_QUEST_DELETE_CARD_SET_ERROR_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}