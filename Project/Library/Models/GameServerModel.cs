namespace PointBlank
{
    public class GameServerModel
    {
        public int state;
        public int id;
        public byte type;
        public int lastCount;
        public ushort maxPlayers;
        public string ip;
        public ushort port;
        public GameServerModel(string ip)
        {
            this.ip = ip;
        }
    }
}