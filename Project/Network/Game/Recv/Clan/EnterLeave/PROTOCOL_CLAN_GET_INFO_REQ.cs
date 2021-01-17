using System;

namespace PointBlank.Game
{
    /*
     *  Obtém e exibe informações do clan selecionado da lista de clãs.
     */
    public class PROTOCOL_CLAN_GET_INFO_REQ : GamePacketReader
    {
        private int clanId;
        private int unk;
        public override void ReadImplement()
        {
            clanId = ReadInt();
            unk = ReadByte(); //1 = Sempre | 0 = Quando passa dono
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
                if (clan.id > 0)
                {
                    //Não precisa do owner != null pois o clan está sendo exibido na lista de clans e quando for obter informações precisa enviar o pacote.
                    Account owner = AccountManager.GetAccount(clan.ownerId, 0);
                    client.SendPacket(new PROTOCOL_CLAN_DETAIL_INFO_ACK(clan, owner, 1));
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}