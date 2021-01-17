namespace PointBlank.Game
{
    public class BASE_QUEST_UPDATE_INFO_PAK : GamePacketWriter
    {
        private Account player;
        public BASE_QUEST_UPDATE_INFO_PAK(Account player)
        {
            this.player = player;
        }

        public override void Write()
        {
            WriteH(2604);
            if (player != null)
            {
                WriteQ(player.playerId);
                WriteD(player.brooch);
                WriteD(player.insignia);
                WriteD(player.medal);
                WriteD(player.blueorder);
            }
            else
            {
                WriteB(new byte[24]);
            }
        }
    }
}