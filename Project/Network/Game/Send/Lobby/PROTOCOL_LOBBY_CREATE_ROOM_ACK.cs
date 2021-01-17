namespace PointBlank.Game
{
    public class LOBBY_CREATE_ROOM_PAK : GamePacketWriter
    {
        private Account leader;
        private Room room;
        private uint error;
        public LOBBY_CREATE_ROOM_PAK(uint error, Room room, Account leader)
        {
            this.error = error;
            this.room = room;
            this.leader = leader;
        }

        public override void Write()
        {
            WriteH(3090);
            WriteD(error == 0 ? (uint)room.roomId : error);
            if (error == 0 && room != null && leader != null)
            {
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
                WriteS(leader.nickname, 33);
                WriteD(room.killtime);
                WriteC(room.limit);
                WriteC(room.seeConf);
                WriteH((ushort)room.balancing);
            }
        }
    }
}