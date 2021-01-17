using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_PROPOSE_REQ : GamePacketReader
    {
        private int id, serverInfo;
        public override void ReadImplement()
        {
            id = ReadShort();
            serverInfo = ReadShort();
        }

        public override void RunImplement()
        {
            try
            {
                Account p = client.SessionPlayer;
                if (p != null && p.match != null && p.matchSlot == p.match.leader && p.match.state == MatchStateEnum.Ready)
                {
                    int channelId = serverInfo - (serverInfo / 10 * 10);
                    Match mt = ServersManager.GetChannel(channelId).GetMatch(id);
                    if (mt != null)
                    {
                        Account lider = mt.GetLeader();
                        if (lider != null && lider.client != null && lider.isOnline)
                        {
                            lider.SendPacket(new CLAN_WAR_MATCH_REQUEST_BATTLE_PAK(p.match, p));
                            client.SendCompletePacket(PackageDataManager.CLAN_WAR_LEAVE_TEAM_SUCCESS_PAK);
                            return;
                        }
                    }
                }
                client.SendCompletePacket(PackageDataManager.CLAN_WAR_MATCH_PROPOSE_ERROR_PAK);
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}