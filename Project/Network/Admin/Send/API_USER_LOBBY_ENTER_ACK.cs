namespace PointBlank.Api
{
    public class API_USER_LOBBY_ENTER_ACK : ApiPacketWriter
    {
        private Account player;
        public API_USER_LOBBY_ENTER_ACK(Account player)
        {
            this.player = player;
        }
        public override void Write()
        {
            WriteH(4);
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
        }
    }
}