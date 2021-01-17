namespace PointBlank.Game
{
    public class CLAN_MEMBER_INFO_CHANGE_PAK : GamePacketWriter
    {
        private Account player;
        private ulong status;
        public CLAN_MEMBER_INFO_CHANGE_PAK(Account player)
        {
            this.player = player;
            status = Utilities.GetClanStatus(player.status, player.isOnline);
        }
        public CLAN_MEMBER_INFO_CHANGE_PAK(Account player, FriendStateEnum memberState)
        {
            this.player = player;
            if (memberState == 0)
            {
                status = Utilities.GetClanStatus(player.status, player.isOnline);
            }
            else
            {
                status = Utilities.GetClanStatus(memberState);
            }
        }

        public override void Write()
        {
            WriteH(1355);
            WriteQ(player.playerId);
            WriteQ(status);
        }
    }
}