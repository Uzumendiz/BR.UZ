namespace PointBlank.Game
{
    public class BASE_GET_USER_STATS_PAK : GamePacketWriter
    {
        private PlayerStats stats;
        public BASE_GET_USER_STATS_PAK(PlayerStats stats)
        {
            this.stats = stats;
        }

        public override void Write()
        {
            WriteH(2592);
            if (stats != null)
            {
                WriteD(stats.fights);
                WriteD(stats.fightsWin);
                WriteD(stats.fightsLost);
                WriteD(stats.fightsDraw);
                WriteD(stats.kills);
                WriteD(stats.headshots);
                WriteD(stats.deaths);
                WriteD(stats.totalfights);
                WriteD(stats.totalkills);
                WriteD(stats.escapes);
                WriteD(stats.fights);
                WriteD(stats.fightsWin);
                WriteD(stats.fightsLost);
                WriteD(stats.fightsDraw);
                WriteD(stats.kills);
                WriteD(stats.headshots);
                WriteD(stats.deaths);
                WriteD(stats.totalfights);
                WriteD(stats.totalkills);
                WriteD(stats.escapes);
            }
            else
            {
                WriteB(new byte[80]);
            }
        }
    }
}