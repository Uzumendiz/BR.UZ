namespace PointBlank
{
    public class Friend
    {
        public long playerId;
        public int state;
        public bool removed;
        public PlayerInfo player;
        public Friend(long playerId)
        {
            this.playerId = playerId;
            player = new PlayerInfo(playerId);
        }
        public Friend(long playerId, byte rank, string name, bool isOnline, AccountStatus status)
        {
            this.playerId = playerId;
            SetModel(playerId, rank, name, isOnline, status);
        }
        public void SetModel(long player_id, byte rank, string name, bool isOnline, AccountStatus status)
        {
            player = new PlayerInfo(player_id, rank, name, isOnline, status);
        }
    }
}