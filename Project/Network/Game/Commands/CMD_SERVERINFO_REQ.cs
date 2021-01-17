namespace PointBlank
{
    public class CMD_SERVERINFO_REQ : PacketCommand
    {
        private byte type;
        public CMD_SERVERINFO_REQ(byte type)
        {
            this.type = type;
        }

        public override void RunImplement()
        {
            if (type == 1)
            {
                response = "Jogadores online simultâneos: " + GameManager.SocketSessions.Count;
            }
        }
    }
}