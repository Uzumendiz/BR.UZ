namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CHATTING_ACK : GamePacketWriter
    {
        private string message;
        private int type;
        private int slotId;
        private bool GMColor;
        public PROTOCOL_ROOM_CHATTING_ACK(int type, int slotId, bool GMColor, string message)
        {
            this.type = type;
            this.slotId = slotId;
            this.GMColor = GMColor;
            this.message = message;
        }
        public override void Write()
        {
            WriteH(3851);
            WriteH((short)type);
            WriteD(slotId);
            WriteC(GMColor);
            WriteD(message.Length + 1);
            WriteS(message, message.Length + 1);
        }
    }
}