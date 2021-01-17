namespace PointBlank.Game
{
    public class BATTLE_READYBATTLE_PAK : GamePacketWriter
    {
        private byte PlayersCount;
        private Room room;
        private byte[] Data;
        public BATTLE_READYBATTLE_PAK(Room room)
        {
            this.room = room;
            using (PacketWriter packet = new PacketWriter())
            {
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = this.room.slots[i];
                    if (slot.state >= SlotStateEnum.READY && slot.equipment != null)
                    {
                        Account player = this.room.GetPlayerBySlot(slot);
                        if (player != null && player.slotId == i)
                        {
                            WriteSlotInfo(slot, player, packet);
                            PlayersCount++;
                        }
                    }
                }
                Data = packet.memorystream.ToArray();
            }
        }
        private void WriteSlotInfo(Slot slot, Account player, PacketWriter packet)
        {
            packet.WriteC((byte)slot.Id);
            packet.WriteD(slot.equipment.red);
            packet.WriteD(slot.equipment.blue);
            packet.WriteD(slot.equipment.helmet);
            packet.WriteD(slot.equipment.beret);
            packet.WriteD(slot.equipment.dino);
            packet.WriteD(slot.equipment.primary);
            packet.WriteD(slot.equipment.secondary);
            packet.WriteD(slot.equipment.melee);
            packet.WriteD(slot.equipment.grenade);
            packet.WriteD(slot.equipment.special);
            packet.WriteD(0); //9
            if (player != null)
            {
                packet.WriteC(player.titles.Equiped1);
                packet.WriteC(player.titles.Equiped2);
                packet.WriteC(player.titles.Equiped3);
            }
            else
            {
                packet.WriteB(new byte[3]);
            }
            if (Settings.ClientVersion == "1.15.42")
            {
                packet.WriteD(0);
            }
        }
        public override void Write()
        {
            WriteH(3426);
            WriteH((ushort)room.mapId);
            WriteC(room.stage4vs4);
            WriteC((byte)room.mode);
            WriteC(PlayersCount);
            WriteB(Data);
        }
    }
}