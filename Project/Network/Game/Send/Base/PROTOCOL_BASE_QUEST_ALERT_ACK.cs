namespace PointBlank.Game
{
    public class BASE_QUEST_ALERT_PAK : GamePacketWriter
    {
        private Account player;
        private uint error;
        private byte type;
        public BASE_QUEST_ALERT_PAK(uint error, byte type, Account player)
        {
            this.error = error;
            this.type = type;
            this.player = player;
        }

        public override void Write()
        {
            WriteH(2602);
            WriteD(error); //Sem efeito - 0 || 1 - Efeito || 273 - finish?
            WriteC(type); //1 CardSetIdx?
            if ((error & 1) == 1)
            {
                WriteD(player.exp);
                WriteD(player.gold);
                WriteD(player.brooch);
                WriteD(player.insignia);
                WriteD(player.medal);
                WriteD(player.blueorder);
                WriteD(player.rankId);
            }
        }
    }
}