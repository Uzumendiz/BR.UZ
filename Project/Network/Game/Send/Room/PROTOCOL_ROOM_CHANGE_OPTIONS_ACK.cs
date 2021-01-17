namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CHANGE_OPTIONS_ACK : GamePacketWriter
    {
        private Room room;
        public PROTOCOL_ROOM_CHANGE_OPTIONS_ACK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3859);
            WriteS(room.leaderName, 33);
            WriteD(room.killtime);
            WriteC(room.limit);
            WriteC(room.seeConf);
            WriteH((ushort)room.balancing);
        }
    }
}