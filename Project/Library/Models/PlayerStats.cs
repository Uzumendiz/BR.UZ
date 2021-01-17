using System;

namespace PointBlank
{
    public class PlayerStats
    {
        public int fights;
        public int fightsWin;
        public int fightsLost;
        public int fightsDraw;
        public int kills;
        public int totalkills;
        public int totalfights;
        public int deaths;
        public int escapes;
        public int headshots;
        public int clanFights;
        public int clanWins;

        public int GetKDRatio()
        {
            if (headshots <= 0 && kills <= 0)
            {
                return 0;
            }
            return (int)Math.Floor((kills * 100 + 0.5) / (kills + deaths));
        }

        public int GetHSRatio()
        {
            if (kills <= 0)
            {
                return 0;
            }
            return (int)Math.Floor(headshots * 100 / (double)kills + 0.5);
        }
    }
}