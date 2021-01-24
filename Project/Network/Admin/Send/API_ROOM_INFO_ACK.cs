namespace PointBlank.Api
{
    public class API_ROOM_INFO_ACK : ApiPacketWriter
    {
        private Room room;
        public API_ROOM_INFO_ACK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(9);
            WriteS(room.leaderName, 33);
            WriteC((byte)room.killtime);
            WriteC((byte)(room.rounds - 1));
            WriteH((ushort)room.GetInBattleTime());
            WriteC(room.limit);
            WriteC(room.seeConf);
            WriteH((ushort)room.balancing);
        }
    }
}