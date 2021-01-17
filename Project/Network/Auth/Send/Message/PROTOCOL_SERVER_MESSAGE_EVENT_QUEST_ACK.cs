namespace PointBlank.Auth
{
    public class PROTOCOL_SERVER_MESSAGE_EVENT_QUEST_ACK : GamePacketWriter
    {
        public override void Write()
        {
            WriteH(2061);
        }
    }
}