namespace PointBlank.Api
{
    public class API_GET_SERVER_INFO_REQ : ApiPacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            client.SendPacket(new API_SERVER_INFO_ACK());
        }
    }
}