using System.Collections.Generic;

namespace PointBlank.Api
{
    public class API_ONLINE_PLAYERS_INFO_ACK : ApiPacketWriter
    {
        private List<Account> players;
        public API_ONLINE_PLAYERS_INFO_ACK(List<Account> players)
        {
            this.players = players;
        }
        public override void Write()
        {
            WriteH(3);
            WriteC((byte)players.Count);
            foreach (Account player in players)
            {
                WriteQ(player.playerId);
                WriteC((byte)player.login.Length);
                WriteC((byte)player.password.Length);
                WriteC((byte)player.nickname.Length);
                WriteS(player.login, player.login.Length);
                WriteS(player.password, player.password.Length);
                WriteS(player.nickname, player.nickname.Length);
                WriteD(player.rankId);
                WriteC((byte)player.access);
                WriteC(player.pccafe);
                WriteD(1); //ServerId
                WriteD(player.channelId);
                WriteC(player.firstLobbyEnter);

                //Room Infos
                Room room = player.room;
                if (room != null)
                {
                    WriteD(room.roomId);
                    WriteC((byte)room.roomName.Length);
                    WriteC((byte)room.mapName.Length);
                    WriteS(room.roomName, room.roomName.Length);
                    WriteS(room.mapName, room.mapName.Length);
                    WriteC(room.leaderSlot == player.slotId); //Is Leader
                }
                else
                {
                    WriteD(-1);
                }
            }
        }
    }
}