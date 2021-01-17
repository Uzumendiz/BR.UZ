using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_PARTY_LIST_REQ : GamePacketReader
    {
        private List<Match> partyList = new List<Match>();
        private int page;
        public override void ReadImplement()
        {
            page = ReadByte();
            //Logger.warning("[Party_List] Page: " + page);
        }

        public override void RunImplement()
        {
            try
            {
                Account p = client.SessionPlayer;
                if (p == null)
                    return;
                if (p.clanId > 0)
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
                                    partyList.Add(m);
                                }
                            }
                        }
                    }
                }
                client.SendPacket(new CLAN_WAR_PARTY_LIST_PAK(p.clanId == 0 ? 91 : 0, partyList));
                partyList = null;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}