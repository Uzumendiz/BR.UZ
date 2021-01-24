namespace PointBlank.Api
{
    public class API_GET_ROOM_INFO_REQ : ApiPacketReader
    {
        private int channelId;
        private int roomId;
        public override void ReadImplement()
        {
            channelId = ReadInt();
            roomId = ReadInt();
        }

        public override void RunImplement()
        {
            Channel channel = ServersManager.GetChannel(channelId);
            if (channel != null)
            {
                Room room = channel.GetRoom(roomId);
                if (room != null)
                {
                    client.SendPacket(new API_ROOM_INFO_ACK(room));
                }
            }
        }
    }
}