namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_PLAYER_STATISTICS_ACK : GamePacketWriter
    {
        private PlayerStats statistics;
        public PROTOCOL_LOBBY_PLAYER_STATISTICS_ACK(PlayerStats statistics)
        {
            this.statistics = statistics;
        }

        public override void Write()
        {
            WriteH(2640);
            if (statistics != null)
            {
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
            else
            {
                WriteB(new byte[80]);
            }
        }
    }
}