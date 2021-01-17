namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CHANGE_PASSWORD_ACK : GamePacketWriter
    {
        private string password;
        public PROTOCOL_ROOM_CHANGE_PASSWORD_ACK(string password)
        {
            this.password = password;
        }

        public override void Write()
        {
            WriteH(3907);
            WriteS(password, 4);
        }
    }
}