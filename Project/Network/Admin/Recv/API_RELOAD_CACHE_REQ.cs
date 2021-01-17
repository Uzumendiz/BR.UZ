namespace PointBlank.Api
{
    public class API_RELOAD_CACHE_REQ : ApiPacketReader
    {
        public override void ReadImplement()
        {

        }

        public override void RunImplement()
        {
            client.SendPacket(new API_RELOAD_CACHE_RESULT_ACK("Recarregou 10 itens"));
        }
    }
}