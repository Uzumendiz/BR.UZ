using System;

namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_CHECK_NAME_REQ : GamePacketReader
    {
        private string clanName;
        public override void ReadImplement()
        {
            clanName = ReadString(ReadByte());
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
                if (ClanManager.CheckNameLengthInvalid(clanName) || !StringFilter.CheckStringFilter(clanName) || ClanManager.IsClanNameExist(clanName).Result)
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CHECK_NAME_ERROR_ACK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.CLAN_CHECK_NAME_SUCCESS_ACK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}