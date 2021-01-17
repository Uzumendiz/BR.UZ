namespace PointBlank.Game
{
    public class BATTLE_RECORD_PAK : GamePacketWriter
    {
        private Room room;
        public BATTLE_RECORD_PAK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3363);
            WriteH(room.redKills);
            WriteH(room.redDeaths);
            WriteH(room.blueKills);
            WriteH(room.blueDeaths);
            for (int i = 0; i < 16; i++)
            {
                Slot slot = room.slots[i];
                WriteH(slot.allKills);
                WriteH(slot.allDeaths);
            }
        }
    }
}