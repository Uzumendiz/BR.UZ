using System;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_INFO_REQ : AuthPacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.checkUserInfo || player.inventory.items.Count > 0)
                {
                    client.SendCompletePacket(PackageDataManager.PROTOCOL_BASE_USER_INFO_ERROR_ACK);
                    return;
                }
                player.checkUserInfo = true;
                Clan clan = ClanManager.GetClan(player.clanId);
                player.LoadInventory();
                player.LoadMissionList();
                player.LoadPlayerTitles();
                player.LoadPlayerBonus();
                player.LoadPlayerEvents();
                player.status.SetData(4294967295, player.playerId);
                player.status.UpdateServer(0); //0 = Auth(Ainda não selecionou o servidor) Game = Settings.ServerId
                player.DiscountPlayerItems();
                int dateNow = int.Parse(player.GetDate());
                if (player.pccafe > 0 && dateNow > player.pccafeDate && player.UpdatePccafe(0, 0, 0, 0))
                {
                    player.pccafe = 0;
                    player.pccafeDate = 0;
                }
                player.ExecuteQuery($"UPDATE accounts SET last_login='{dateNow}' WHERE id='{player.playerId}'");
                client.SendPacket(new PROTOCOL_BASE_USER_INFO_ACK(player, clan, dateNow, 0));
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}