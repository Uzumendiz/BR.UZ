using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_LEAVE_TEAM_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account p = client.SessionPlayer;
                Match mt = p?.match;
                if (mt == null || !mt.RemovePlayer(p))
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_WAR_LEAVE_TEAM_ERROR_PAK);
                    return;
                }

                client.SendCompletePacket(PackageDataManager.CLAN_WAR_LEAVE_TEAM_SUCCESS_PAK);
                p.status.UpdateClanMatch(255);
                p.SyncPlayerToClanMembers();
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}