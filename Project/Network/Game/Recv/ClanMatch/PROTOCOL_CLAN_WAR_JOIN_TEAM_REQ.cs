using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_JOIN_TEAM_REQ : GamePacketReader
    {
        private int matchId, serverInfo, type;
        private uint erro;
        public override void ReadImplement()
        {
            matchId = ReadShort();
            serverInfo = ReadShort();
            type = ReadByte(); //0 normal | 1 - amigos do clã
        }

        public override void RunImplement()
        {
            try
            {
                Account p = client.SessionPlayer;
                if (type >= 2 || p == null || p.match != null || p.room != null)
                {
                    client.SendPacket(new CLAN_WAR_JOIN_TEAM_PAK(0x80000000));
                    return;
                }
                int channelId = serverInfo - ((serverInfo / 10) * 10);
                Channel ch = ServersManager.GetChannel(type == 0 ? channelId : p.channelId);
                if (ch != null)
                {
                    if (p.clanId == 0)
                        client.SendPacket(new CLAN_WAR_JOIN_TEAM_PAK(0x8000105B));
                    else
                    {
                        Match mt = type == 1 ? ch.GetMatch(matchId, p.clanId) : ch.GetMatch(matchId);
                        if (mt != null)
                            JoinPlayer(p, mt);
                        else
                            client.SendPacket(new CLAN_WAR_JOIN_TEAM_PAK(0x80000000));
                    }
                }
                else
                    client.SendPacket(new CLAN_WAR_JOIN_TEAM_PAK(0x80000000));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
        private void JoinPlayer(Account p, Match mt)
        {
            if (!mt.AddPlayer(p))
                erro = 0x80000000;
            client.SendPacket(new CLAN_WAR_JOIN_TEAM_PAK(erro, mt));
            if (erro == 0)
            {
                using (CLAN_WAR_REGIST_MERCENARY_PAK packet = new CLAN_WAR_REGIST_MERCENARY_PAK(mt))
                    mt.SendPacketToPlayers(packet);
            }
        }
    }
}