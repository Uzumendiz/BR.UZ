namespace PointBlank.Game
{
    public class A_3428_PAK : GamePacketWriter
    {
        private Room room;
        public A_3428_PAK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3429);
            WriteD((int)room.mode);
            WriteD((room.GetTimeByMask() * 60) - room.GetInBattleTime());
            if (room.mode == RoomTypeEnum.Dino)
            {
                WriteD(room.redDino);
                WriteD(room.blueDino);
            }
            else if (room.mode == RoomTypeEnum.DeathMatch || room.mode == RoomTypeEnum.HeadHunter || room.mode == RoomTypeEnum.Chaos)
            {
                WriteD(room.redKills);
                WriteD(room.blueKills);
            }
            else
            {
                WriteD(room.redRounds);
                WriteD(room.blueRounds);
            }
        }
    }
}