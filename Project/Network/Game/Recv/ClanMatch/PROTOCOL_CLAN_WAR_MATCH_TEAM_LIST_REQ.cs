using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_MATCH_TEAM_LIST_REQ : GamePacketReader
    {
        private int page;
        public override void ReadImplement()
        {
            page = ReadShort();
            //Logger.warning("[Match_Team_List] Page: " + page);
        }

        public override void RunImplement()
        {
            try
            {
                Account p = client.SessionPlayer;
                if (p == null || p.match == null)
                    return;
                Channel ch = p.GetChannel();
                if (ch != null && ch.type == 4)
                    client.SendPacket(new CLAN_WAR_MATCH_TEAM_LIST_PAK(page, ch.matchs, p.match.matchId));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}