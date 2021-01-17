using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_CHECK_LOGO_REQ : GamePacketReader
    {
        private uint logo;
        public override void ReadImplement()
        {
            logo = ReadUint();
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
                if (player.clanId == 0 || clan.id == 0 || clan.logo == logo || ClanManager.IsClanLogoExist(logo).Result)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CHECK_LOGO_ERROR_ACK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CHECK_LOGO_SUCCESS_ACK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}