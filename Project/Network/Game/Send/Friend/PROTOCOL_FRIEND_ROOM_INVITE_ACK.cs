namespace PointBlank.Game
{
    public class FRIEND_ROOM_INVITE_PAK : GamePacketWriter
    {
        private byte index;
        public FRIEND_ROOM_INVITE_PAK(byte index)
        {
            this.index = index;
        }

        public override void Write()
        {
            WriteH(277);
            WriteC(index);
        }
    }
}