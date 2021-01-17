using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_MEMBER_LIST_REQ : GamePacketReader
    {
        private int page;
        public override void ReadImplement()
        {
            page = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null)
                {
                    return;
                }
                Clan clan = ClanManager.GetClan(player.clanId);
                if (clan.id == 0)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_MEMBER_LIST_ERROR_PAK);
                    return;
                }
                List<Account> clanPlayers = clan.GetPlayers(-1, false);
                using (PacketWriter writer = new PacketWriter())
                {
                    int count = 0;
                    for (int i = page * 14; i < clanPlayers.Count; i++)
                    {
                        Account member = clanPlayers[i];
                        writer.WriteQ(member.playerId);
                        writer.WriteS(member.nickname, 33);
                        writer.WriteC(member.rankId);
                        writer.WriteC((byte)member.clanAuthority);
                        writer.WriteQ(Utilities.GetClanStatus(member.status, member.isOnline));
                        writer.WriteD(member.clanDate);
                        writer.WriteC(member.nickcolor);
                        if (count++ == 14)
                        {
                            break;
                        }
                    }
                    client.SendPacket(new CLAN_MEMBER_LIST_PAK(page, count, writer.memorystream.ToArray()));
                }
                clanPlayers = null;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}