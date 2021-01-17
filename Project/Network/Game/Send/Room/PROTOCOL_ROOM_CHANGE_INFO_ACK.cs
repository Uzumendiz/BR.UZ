namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_CHANGE_INFO_ACK : GamePacketWriter
    {
        private Room room;
        public PROTOCOL_ROOM_CHANGE_INFO_ACK(Room room)
        {
            this.room = room;
        }
        public override void Write()
        {
            WriteH(3848);
            WriteD(room.roomId);
            WriteS(room.roomName, 23);
            WriteH((ushort)room.mapId);
            WriteC(room.stage4vs4);
            WriteC((byte)room.mode);
            WriteC((byte)room.state);
            WriteC(room.GetAllPlayersCount());
            WriteC(room.GetSlotCount());
            WriteC(room.ping);
            WriteC(room.weaponsFlag);
            WriteC(room.randomMap);
            WriteC((byte)room.modeSpecial);
            Account leader = room.GetLeader();
            WriteS(leader != null ? leader.nickname : room.leaderName, 33);
            WriteD(room.killtime);
            WriteC(room.limit);
            WriteC(room.seeConf);
            WriteH((ushort)room.balancing);
            if (room.IsBotMode())
            {
                WriteC(room.aiCount);
                WriteC(room.aiLevel);
            }
        }
    }
}