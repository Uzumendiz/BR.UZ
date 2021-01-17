using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_MATCH_TEAM_INFO_REQ : GamePacketReader
    {
        private int id, serverInfo;
        public override void ReadImplement()
        {
            id = ReadShort();
            serverInfo = ReadShort();
        }

        public override void RunImplement()
        {
            Account player = client.SessionPlayer;
            if (player == null || player.match == null)
                return;
            try
            {
                int channelId = serverInfo - ((serverInfo / 10) * 10);
                Channel ch = ServersManager.GetChannel(channelId);
                if (ch != null)
                {
                    Match match = ch.GetMatch(id);
                    if (match != null)
                        client.SendPacket(new CLAN_WAR_MATCH_TEAM_INFO_PAK(0, match.clan));
                    else
                        client.SendPacket(new CLAN_WAR_MATCH_TEAM_INFO_PAK(0x80000000));
                }
                else
                    client.SendPacket(new CLAN_WAR_MATCH_TEAM_INFO_PAK(0x80000000));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}