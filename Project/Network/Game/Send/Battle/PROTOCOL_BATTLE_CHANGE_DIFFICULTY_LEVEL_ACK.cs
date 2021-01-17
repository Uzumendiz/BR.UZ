namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_ACK : GamePacketWriter
    {
        private Room room;
        public PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_ACK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3377);
            WriteC(room.IngameAiLevel);
            for (int i = 0; i < 16; i++)
            {
                WriteD(room.slots[i].aiLevel); //Level atual da I.A
            }
        }
    }
}