namespace PointBlank.Game
{
    public class BATTLE_MISSION_ESCAPE_PAK : GamePacketWriter
    {
        private Room room;
        private Slot slot;
        public BATTLE_MISSION_ESCAPE_PAK(Room room, Slot slot)
        {
            this.room = room;
            this.slot = slot;
        }

        public override void Write()
        {
            WriteH(3383);
            WriteH(room.redDino);
            WriteH(room.blueDino);
            WriteD(slot.Id); //slot do jogador que passou no portal
            WriteH((short)slot.passSequence); //vezes seguidas que passo no portal
        }
    }
}