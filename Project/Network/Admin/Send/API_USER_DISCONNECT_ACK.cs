namespace PointBlank.Api
{
    public class API_USER_DISCONNECT_ACK : ApiPacketWriter
    {
        private Account player;
        private byte type;
        public API_USER_DISCONNECT_ACK(Account player, byte type)
        {
            this.player = player;
            this.type = type;
        }
        public override void Write()
        {
            WriteH(8);
            WriteC(type);
            WriteQ(player.playerId);
        }
    }
}