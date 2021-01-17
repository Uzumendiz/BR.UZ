namespace PointBlank.Game
{
    public class BATTLE_DEATH_PAK : GamePacketWriter
    {
        private Room room;
        private FragInfos kills;
        private Slot killer;
        private bool isBotMode;
        public BATTLE_DEATH_PAK(Room room, FragInfos kills, Slot killer, bool isBotMode)
        {
            this.room = room;
            this.kills = kills;
            this.killer = killer;
            this.isBotMode = isBotMode;
        }

        public override void Write()
        {
            WriteH(3355);
            WriteC((byte)kills.killingType);
            WriteC(kills.killsCount);
            WriteC(kills.killerIdx);
            WriteD(kills.weaponId);
            WriteT(kills.x);
            WriteT(kills.y);
            WriteT(kills.z);
            WriteC(kills.flag);
            for (int i = 0; i < kills.frags.Count; i++)
            {
                Frag frag = kills.frags[i];
                WriteC(frag.victimWeaponClass);
                WriteC(frag.hitspotInfo);
                WriteH((short)frag.killFlag);
                WriteC(frag.flag);
                WriteT(frag.x);
                WriteT(frag.y);
                WriteT(frag.z);
            }
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
            if (killer.killsOnLife == 2)
            {
                WriteC(1);
            }
            else if (killer.killsOnLife == 3)
            {
                WriteC(2);
            }
            else if (killer.killsOnLife > 3)
            {
                WriteC(3);
            }
            else
            {
                WriteC(0);
            }
            WriteH(kills.Score);
            if (room.mode == RoomTypeEnum.Dino)
            {
                WriteH(room.redDino);
                WriteH(room.blueDino);
            }
        }
    }
}