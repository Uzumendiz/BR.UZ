using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_UPTIME_REQ : GamePacketReader
    {
        private int formacao;
        public override void ReadImplement()
        {
            formacao = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account p = client.SessionPlayer;
                if (p == null)
                    return;
                Match mt = p.match;
                if (mt != null && p.matchSlot == mt.leader)
                {
                    mt.formação = formacao;
                    using (CLAN_WAR_MATCH_UPTIME_PAK packet = new CLAN_WAR_MATCH_UPTIME_PAK(0, formacao))
                        mt.SendPacketToPlayers(packet);
                }
                else client.SendPacket(new CLAN_WAR_MATCH_UPTIME_PAK(0x80000000));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}