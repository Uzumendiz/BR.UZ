namespace PointBlank.Game
{
    public class GM_LOG_LOBBY_PAK : GamePacketWriter
    {
        private Account player;
        public GM_LOG_LOBBY_PAK(Account player)
        {
            this.player = player;
        }

        public override void Write()
        {
            WriteH(2685);
            WriteD(0);
            WriteQ(player.playerId);
        }
    }
}