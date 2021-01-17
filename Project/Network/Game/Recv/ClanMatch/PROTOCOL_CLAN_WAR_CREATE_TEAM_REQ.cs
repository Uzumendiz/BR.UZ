using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_WAR_CREATE_TEAM_REQ : GamePacketReader
    {
        private int formacao;
        private List<int> party = new List<int>();
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
                Channel ch = p.GetChannel();
                if (ch != null && ch.type == 4 && p.room == null)
                {
                    if (p.match != null)
                    {
                        client.SendPacket(new CLAN_WAR_CREATE_TEAM_PAK(0x80001087));
                        return;
                    }
                    if (p.clanId == 0)
                    {
                        client.SendPacket(new CLAN_WAR_CREATE_TEAM_PAK(0x8000105B));
                        return;
                    }
                    int matchId = -1, friendId = -1;
                    lock (ch.matchs)
                    {
                        for (int i = 0; i < 250; i++)
                        {
                            if (ch.GetMatch(i) == null)
                            {
                                matchId = i;
                                break;
                            }
                        }
                        foreach (Match m in ch.matchs)
                        {
                            if (m.clan.id == p.clanId)
                            {
                                party.Add(m.friendId);
                            }
                        }
                    }
                    for (int i = 0; i < 25; i++)
                    {
                        if (!party.Contains(i))
                        {
                            friendId = i;
                            break;
                        }
                    }
                    if (matchId == -1)
                        client.SendPacket(new CLAN_WAR_CREATE_TEAM_PAK(0x80001088));
                    else if (friendId == -1)
                        client.SendPacket(new CLAN_WAR_CREATE_TEAM_PAK(0x80001089));
                    else
                    {
                        try
                        {
                            Match match = new Match(ClanManager.GetClan(p.clanId))
                            {
                                matchId = matchId,
                                friendId = friendId,
                                formação = formacao,
                                channelId = p.channelId,
                                serverId = Settings.ServerId
                            };
                            match.AddPlayer(p);
                            ch.AddMatch(match);
                            client.SendPacket(new CLAN_WAR_CREATE_TEAM_PAK(0, match));
                            client.SendPacket(new CLAN_WAR_REGIST_MERCENARY_PAK(match));
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex);
                        }
                    }
                }
                else
                    client.SendPacket(new CLAN_WAR_CREATE_TEAM_PAK(0x80000000));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}