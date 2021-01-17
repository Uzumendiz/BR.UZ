using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_MEMBER_CONTEXT_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
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
                    client.SendCompletePacket(PackageDataManager.CLAN_MEMBER_CONTEXT_ERROR_PAK);
                }
                else
                {
                    client.SendPacket(new CLAN_MEMBER_CONTEXT_PAK(0, clan.GetClanPlayers()));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}