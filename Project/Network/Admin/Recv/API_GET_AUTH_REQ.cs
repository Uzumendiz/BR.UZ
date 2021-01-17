namespace PointBlank.Api
{
    public class API_GET_AUTH_REQ : ApiPacketReader
    {
        public override void ReadImplement()
        {
     
        }

        public override void RunImplement()
        {
            client.SendPacket(new API_AUTH_ACK());
        }
    }
}