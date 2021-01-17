namespace PointBlank.Game
{
    public class BATTLE_DINO_PLACAR_PAK : GamePacketWriter
    {
        private Room room;
        public BATTLE_DINO_PLACAR_PAK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3389);
            WriteH(room.redDino);
            WriteH(room.blueDino);
        }
    }
}