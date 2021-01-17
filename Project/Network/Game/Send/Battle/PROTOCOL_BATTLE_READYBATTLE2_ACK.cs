namespace PointBlank.Game
{
    public class BATTLE_READYBATTLE2_PAK : GamePacketWriter
    {
        private Slot slot;
        private PlayerTitles title;
        public BATTLE_READYBATTLE2_PAK(Slot slot, PlayerTitles title)
        {
            this.slot = slot;
            this.title = title;
        }

        public override void Write()
        {
            WriteH(3427);
            WriteC((byte)slot.Id);
            WriteD(slot.equipment.red);
            WriteD(slot.equipment.blue);
            WriteD(slot.equipment.helmet);
            WriteD(slot.equipment.beret);
            WriteD(slot.equipment.dino);
            WriteD(slot.equipment.primary);
            WriteD(slot.equipment.secondary);
            WriteD(slot.equipment.melee);
            WriteD(slot.equipment.grenade);
            WriteD(slot.equipment.special);
            WriteD(0);
            WriteC(title.Equiped1);
            WriteC(title.Equiped2);
            WriteC(title.Equiped3);
            if (Settings.ClientVersion == "1.15.42")
            {
                WriteD(0);
            }
        }
    }
}