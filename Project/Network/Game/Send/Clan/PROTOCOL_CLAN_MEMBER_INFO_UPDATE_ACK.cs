namespace PointBlank.Game
{
    public class PROTOCOL_CLAN_MEMBER_INFO_UPDATE_ACK : GamePacketWriter
    {
        private Account player;
        private ulong status;
        public PROTOCOL_CLAN_MEMBER_INFO_UPDATE_ACK(Account player)
        {
            this.player = player;
            status = Utilities.GetClanStatus(player.status, player.isOnline);
        }

        public override void Write()
        {
            WriteH(1380);
            WriteQ(player.playerId);
            WriteS(player.nickname, 33);
            WriteC(player.rankId);
            WriteC((byte)player.clanAuthority);
            WriteQ(status);
            WriteD(player.clanDate);
            WriteC(player.nickcolor);
        }
    }
}