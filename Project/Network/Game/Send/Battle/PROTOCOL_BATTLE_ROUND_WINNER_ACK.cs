namespace PointBlank.Game
{
    public class BATTLE_ROUND_WINNER_PAK : GamePacketWriter
    {
        private Room room;
        private int winner;
        private RoundEndTypeEnum reason;
        public BATTLE_ROUND_WINNER_PAK(Room room, int winner, RoundEndTypeEnum reason)
        {
            this.room = room;
            this.winner = winner;
            this.reason = reason;
        }
        public BATTLE_ROUND_WINNER_PAK(Room room, TeamResultTypeEnum winner, RoundEndTypeEnum reason)
        {
            this.room = room;
            this.winner = (int)winner;
            this.reason = reason;
        }
        public override void Write()
        {
            WriteH(3353);
            WriteC((byte)winner); //empate = 2
            WriteC((byte)reason); //empate = 1
            if (room.mode == RoomTypeEnum.Dino)
            {
                WriteH(room.redDino);
                WriteH(room.blueDino);
            }
            else if (room.mode == RoomTypeEnum.DeathMatch || room.mode == RoomTypeEnum.HeadHunter || room.mode == RoomTypeEnum.Chaos)
            {
                WriteH(room.redKills);
                WriteH(room.blueKills);
            }
            else
            {
                WriteH(room.redRounds);
                WriteH(room.blueRounds);
            }
        }
    }
}