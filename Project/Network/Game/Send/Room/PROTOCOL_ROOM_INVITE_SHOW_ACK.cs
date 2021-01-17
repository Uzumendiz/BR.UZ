namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_INVITE_SHOW_ACK : GamePacketWriter
    {
        private Account sender;
        private Room room;
        public PROTOCOL_ROOM_INVITE_SHOW_ACK(Account sender, Room room)
        {
            this.sender = sender;
            this.room = room;
        }

        public override void Write()
        {
            WriteH(2053);
            WriteS(sender.nickname, 33);
            WriteD(room.roomId);
            WriteQ(sender.playerId);
            WriteS(room.password, 4);
        }
    }
}