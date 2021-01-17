namespace PointBlank.Game
{
    public class LOBBY_GET_ROOMINFO_PAK : GamePacketWriter
    {
        private Room room;
        public LOBBY_GET_ROOMINFO_PAK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3088);
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