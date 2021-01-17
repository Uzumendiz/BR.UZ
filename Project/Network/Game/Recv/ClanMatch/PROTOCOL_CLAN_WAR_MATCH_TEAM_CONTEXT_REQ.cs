using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_MATCH_TEAM_CONTEXT_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
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
                    client.SendPacket(new CLAN_WAR_MATCH_TEAM_CONTEXT_PAK(ch.matchs.Count));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}