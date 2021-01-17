using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_CHECK_CREATE_INVITE_REQ : GamePacketReader
    {
        private int clanId;
        public override void ReadImplement()
        {
            clanId = ReadInt();
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
                Clan clan = ClanManager.GetClan(clanId);
                if (clan.id == 0)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CHECK_CREATE_INVITE_0x80000000_PAK);
                }
                else if (player.rankId < clan.limitRankId)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CHECK_CREATE_INVITE_2147487867_PAK);
                }
                else if (player.age < clan.limitAgeBigger || player.age > clan.limitAgeSmaller)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CHECK_CREATE_INVITE_0x8000107A_ACK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CHECK_CREATE_INVITE_SUCCESS_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}