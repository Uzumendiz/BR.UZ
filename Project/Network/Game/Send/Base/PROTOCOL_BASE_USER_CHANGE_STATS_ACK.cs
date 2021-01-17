namespace PointBlank.Game
{
    public class BASE_USER_CHANGE_STATS_PAK : GamePacketWriter
    {
        private PlayerStats statistics;
        public BASE_USER_CHANGE_STATS_PAK(PlayerStats statistics)
        {
            this.statistics = statistics;
        }

        public override void Write()
        {
            WriteH(2610);
            WriteD(statistics.fights);
            WriteD(statistics.fightsWin);
            WriteD(statistics.fightsLost);
            WriteD(statistics.fightsDraw);
            WriteD(statistics.kills);
            WriteD(statistics.headshots);
            WriteD(statistics.deaths);
            WriteD(statistics.totalfights);
            WriteD(statistics.totalkills);
            WriteD(statistics.escapes);
            WriteD(statistics.fights);
            WriteD(statistics.fightsWin);
            WriteD(statistics.fightsLost);
            WriteD(statistics.fightsDraw);
            WriteD(statistics.kills);
            WriteD(statistics.headshots);
            WriteD(statistics.deaths);
            WriteD(statistics.totalfights);
            WriteD(statistics.totalkills);
            WriteD(statistics.escapes);
        }
    }
}