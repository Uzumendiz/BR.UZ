namespace PointBlank
{
    public class PlayerInfo
    {
        public byte rank;
        private long playerId;
        public string playerNickname;
        public bool isOnline;
        public AccountStatus status;
        public PlayerInfo(long playerId)
        {
            this.playerId = playerId;
            status = new AccountStatus();
        }

        public PlayerInfo(long playerId, byte rank, string name, bool isOnline, AccountStatus status)
        {
            this.playerId = playerId;
            SetInfo(rank, name, isOnline, status);
        }

        public void SetOnlineStatus(bool state)
        {
            if (isOnline != state && Utilities.ExecuteQuery($"UPDATE accounts SET online='{state}' WHERE id='{playerId}'"))
            {
                isOnline = state;
            }
        }

        public void SetInfo(byte rank, string playerNickname, bool isOnline, AccountStatus status)
        {
            this.rank = rank;
            this.playerNickname = playerNickname;
            this.isOnline = isOnline;
            this.status = status;
        }
    }
}