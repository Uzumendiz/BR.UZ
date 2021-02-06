namespace PointBlank.Api
{
    public class API_USER_INFO_ACK : ApiPacketWriter
    {
        private Account player;
        public API_USER_INFO_ACK(Account player)
        {
            this.player = player;
        }

        public override void Write()
        {
            WriteH(15);
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
            WriteD(player.gold);
            WriteD(player.cash);

            //Room Infos
            Room room = player.room;
            if (room != null)
            {
                WriteD(room.roomId);
                WriteC((byte)room.mapId);
                WriteC((byte)room.roomName.Length);
                WriteS(room.roomName, room.roomName.Length);
                WriteC(room.leaderSlot == player.slotId); //Is Leader
            }
            else
            {
                WriteD(-1);
            }
        }
    }
}