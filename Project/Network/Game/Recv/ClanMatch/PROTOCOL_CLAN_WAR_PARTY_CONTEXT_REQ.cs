using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_PARTY_CONTEXT_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account p = client.SessionPlayer;
                int matchs = 0;
                if (p != null && p.clanId > 0)
                {
                    Channel ch = p.GetChannel();
                    if (ch != null && ch.type == 4)
                    {
                        lock (ch.matchs)
                        {
                            foreach (Match m in ch.matchs)
                            {
                                if (m.clan.id == p.clanId)
                                {
                                    matchs++;
                                }
                            }
                        }
                    }
                }
                client.SendPacket(new CLAN_WAR_PARTY_CONTEXT_PAK(matchs));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}