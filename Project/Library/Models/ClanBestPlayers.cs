namespace PointBlank
{
    public class ClanBestPlayers
    {
        public RecordInfo Exp, Participation, Wins, Kills, Headshot;
        public void SetPlayers(string Exp, string Part, string Wins, string Kills, string Hs)
        {
            string[] expSplit = Exp.Split('-'),
                     partSplit = Part.Split('-'),
                     winsSplit = Wins.Split('-'),
                     killsSplit = Kills.Split('-'),
                     hsSplit = Hs.Split('-');

            this.Exp = new RecordInfo(expSplit);
            Participation = new RecordInfo(partSplit);
            this.Wins = new RecordInfo(winsSplit);
            this.Kills = new RecordInfo(killsSplit);
            Headshot = new RecordInfo(hsSplit);
        }
        public void SetDefault()
        {
            string[] split = new string[] { "0", "0" };

            Exp = new RecordInfo(split);
            Participation = new RecordInfo(split);
            Wins = new RecordInfo(split);
            Kills = new RecordInfo(split);
            Headshot = new RecordInfo(split);
        }

        public long GetPlayerId(string[] split)
        {
            try
            {
                return long.Parse(split[0]);
            }
            catch { return 0; }
        }

        public int GetPlayerValue(string[] split)
        {
            try
            {
                return int.Parse(split[1]);
            }
            catch { return 0; }
        }

        public void SetBestExp(Slot slot)
        {
            if (slot.exp <= Exp.RecordValue)
                return;

            Exp.PlayerId = slot.playerId;
            Exp.RecordValue = slot.exp;
        }

        public void SetBestHeadshot(Slot slot)
        {
            if (slot.headshots <= Headshot.RecordValue)
                return;

            Headshot.PlayerId = slot.playerId;
            Headshot.RecordValue = slot.headshots;
        }

        public void SetBestKills(Slot slot)
        {
            if (slot.allKills <= Kills.RecordValue)
                return;

            Kills.PlayerId = slot.playerId;
            Kills.RecordValue = slot.allKills;
        }

        public void SetBestWins(PlayerStats stats, Slot slot, bool WonTheMatch)
        {
            if (!WonTheMatch)
                return;
            Utilities.ExecuteQuery($"UPDATE accounts SET clan_wins='{stats.clanWins++}' WHERE id='{slot.playerId}'");

            if (stats.clanWins <= Wins.RecordValue)
                return;

            Wins.PlayerId = slot.playerId;
            Wins.RecordValue = stats.clanWins;
        }

        public void SetBestParticipation(PlayerStats stats, Slot slot)
        {
            Utilities.ExecuteQuery($"UPDATE accounts SET clan_fights='{stats.clanFights++}' WHERE id='{slot.playerId}'");
            if (stats.clanFights <= Participation.RecordValue)
                return;

            Participation.PlayerId = slot.playerId;
            Participation.RecordValue = stats.clanFights;
        }
    }

    public class RecordInfo
    {
        public long PlayerId;
        public int RecordValue;
        public RecordInfo(string[] split)
        {
            PlayerId = GetPlayerId(split);
            RecordValue = GetPlayerValue(split);
        }

        public long GetPlayerId(string[] split)
        {
            try
            {
                return long.Parse(split[0]);
            }
            catch { return 0; }
        }

        public int GetPlayerValue(string[] split)
        {
            try
            {
                return int.Parse(split[1]);
            }
            catch { return 0; }
        }

        public string GetSplit()
        {
            return PlayerId + "-" + RecordValue;
        }
    }
}