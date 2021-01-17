using System.Collections.Generic;

namespace PointBlank.Api
{
    public class API_GET_ONLINE_PLAYERS_REQ : ApiPacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            List<Account> players = new List<Account>();
            foreach (Account player in AccountManager.accounts.Values)
            {
                if (player.client != null && player.isOnline)
                {
                    players.Add(player);
                }
            }
            client.SendPacket(new API_ONLINE_PLAYERS_INFO_ACK(players));
        }
    }
}
