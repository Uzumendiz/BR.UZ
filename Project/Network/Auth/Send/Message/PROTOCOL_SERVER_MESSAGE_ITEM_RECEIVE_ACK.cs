namespace PointBlank.Auth
{
    public class PROTOCOL_SERVER_MESSAGE_ITEM_RECEIVE_ACK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(2692);
            WriteD(0); //error
        }
    }
}