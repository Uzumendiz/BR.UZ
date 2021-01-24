using System;

namespace PointBlank.Api
{
    public class API_SERVER_INFO_ACK : ApiPacketWriter
    {
        public override void Write()
        {
            WriteH(2);
            WriteD(AuthManager.SocketSessions.Count);
            WriteD(GameManager.SocketSessions.Count);
            WriteD(AccountManager.accounts.Count);
            WriteD(ClanManager.clans.Count);
            WriteD(Application.recordOnline);
            WriteQ(long.Parse(Application.StartDate.ToString("yyyyMMddHHmmss")));

            GameServerModel server = ServersManager.GetServer();
            WriteD(server.id);
            WriteD(server.type);
            WriteD(server.maxPlayers);

            int allRooms = 0;
            int allPlayers = 0;
            WriteD(Settings.MaxPlayersChannel);
            WriteD(ServersManager.channels.Count);
            foreach (Channel channel in ServersManager.channels)
            {
                WriteD(channel.id);
                WriteD(channel.type);
                WriteD(channel.players.Count);
                WriteD(channel.rooms.Count);
                WriteD(channel.matchs.Count);
                WriteH((ushort)channel.announce.Length);
                WriteS(channel.announce, channel.announce.Length);
                allRooms += channel.rooms.Count;
                allPlayers += channel.players.Count;
            }
            WriteD(allRooms);
            WriteD(allPlayers);

            RankManager.GetRankCounts(out int ranks47, out int ranks48, out int ranks49, out int ranks50, out int ranks51);
            WriteD(ranks47);
            WriteD(ranks48);
            WriteD(ranks49);
            WriteD(ranks50);
            WriteD(ranks51);
        }
    }
}
