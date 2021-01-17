namespace PointBlank.Game
{
    public class BATTLE_MISSION_GENERATOR_INFO_PAK : GamePacketWriter
    {
        private Room room;
        public BATTLE_MISSION_GENERATOR_INFO_PAK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3369);
            WriteH((ushort)room.Bar1);
            WriteH((ushort)room.Bar2);
            for (int i = 0; i < 16; i++)
            {
                WriteH(room.slots[i].damageBar1);
            }
        }
    }
}