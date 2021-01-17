namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_GET_NICKNAME_ACK : GamePacketWriter
    {
        private int slotId;
        private byte nickcolor;
        private string nickname;
        public PROTOCOL_ROOM_GET_NICKNAME_ACK(int slotId, string nickname, byte nickcolor)
        {
            this.slotId = slotId;
            this.nickname = nickname;
            this.nickcolor = nickcolor;
        }

        public override void Write()
        {
            WriteH(3844);
            if (slotId >= 0 && slotId <= 15 && nickname.Length > 0)
            {
                WriteD(slotId);
                WriteS(nickname, 33);
                WriteC(nickcolor);
            }
        }
    }
}