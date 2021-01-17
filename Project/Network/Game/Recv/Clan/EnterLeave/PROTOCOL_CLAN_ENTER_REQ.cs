using System;

namespace PointBlank.Game
{   
    /*
     * Entrar na pagina de clã.
     */
    public class PROTOCOL_CLAN_ENTER_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || player.nickname.Length < Settings.NickMinLength)
                {
                    return;
                }
                Room room = player.room;
                if (room != null)
                {
                    room.ChangeSlotState(player.slotId, SlotStateEnum.CLAN, false);
                    room.StopCountDown(player.slotId);
                    room.UpdateSlotsInfo();
                }
                Clan clan = ClanManager.GetClan(player.clanId);
                int clanId = 0;
                if (clan.id == 0)
                {
                    clanId = player.GetRequestClanId();
                    client.SendPacket(new CLAN_CLIENT_ENTER_PAK(clanId, player.clanAuthority));
                }
                else
                {
                    client.SendPacket(new CLAN_CLIENT_ENTER_PAK(clan.id, player.clanAuthority));
                }
                if (clan.id > 0 && clanId == 0)
                {
                    //Não precisa do owner != null pois o clan está sendo exibido na lista de clans e quando for obter informações precisa enviar o pacote.
                    Account owner = AccountManager.GetAccount(clan.ownerId, 0);
                    client.SendPacket(new PROTOCOL_CLAN_DETAIL_INFO_ACK(clan, owner, 0));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}