namespace PointBlank.Game
{
    public class CLAN_CHANGE_MAX_PLAYERS_PAK : GamePacketWriter
    {
        private int max;
        public CLAN_CHANGE_MAX_PLAYERS_PAK(int max)
        {
            this.max = max;
        }

        public override void Write()
        {
            WriteH(1377);
            WriteC((byte)max);
        }
    }
}