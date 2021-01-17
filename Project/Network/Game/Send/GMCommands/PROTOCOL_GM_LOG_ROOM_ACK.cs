namespace PointBlank.Game
{
    public class GM_LOG_ROOM_PAK : GamePacketWriter
    {
        private Account player;
        public GM_LOG_ROOM_PAK(Account player)
        {
            this.player = player;
        }

        public override void Write()
        {
            WriteH(2687);
            WriteD(0);
            WriteQ(player.playerId);
        }
    }
}