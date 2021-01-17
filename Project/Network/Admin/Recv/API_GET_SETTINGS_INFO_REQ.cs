namespace PointBlank.Api
{
    public class API_GET_SETTINGS_INFO_REQ : ApiPacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            client.SendPacket(new API_SETTINGS_INFO_ACK());
        }
    }
}
