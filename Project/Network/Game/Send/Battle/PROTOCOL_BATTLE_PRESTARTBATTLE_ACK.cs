namespace PointBlank.Game
{
    public class BattleSettings
    {
        public static byte
            Unk1 = 32,
            Unk2 = 21,
            Unk3 = 22,
            Unk4 = 23,
            Unk5 = 24,
            Unk6 = 25,
            Unk7 = 17,
            Unk8 = 27,
            Unk9 = 28,
            Unk10 = 29,
            Unk11 = 26,
            Unk12 = 31,
            Unk13 = 9,
            Unk14 = 33,
            Unk15 = 14,
            Unk16 = 30,
            Unk17 = 1,
            Unk18 = 2,
            Unk19 = 3,
            Unk20 = 4,
            Unk21 = 5,
            Unk22 = 6,
            Unk23 = 7,
            Unk24 = 8,
            Unk25 = 20,
            Unk26 = 10,
            Unk27 = 11,
            Unk28 = 12,
            Unk29 = 13,
            Unk30 = 34,
            Unk31 = 15,
            Unk32 = 16,
            Unk33 = 0,
            Unk34 = 18,
            Unk35 = 19;
    }
    public class PROTOCOL_BATTLE_PRESTARTBATTLE_ACK : GamePacketWriter
    {
        private Account player;
        private Account leader;
        private Room room;
        private bool isPreparing;
        private bool loadHits;
        private uint UniqueRoomId;
        private int roomSeed;
        private byte udpType;
        public PROTOCOL_BATTLE_PRESTARTBATTLE_ACK(Account player, Account leader, bool loadHits, byte udpType)
        {
            this.player = player;
            this.leader = leader;
            this.loadHits = loadHits;
            this.udpType = udpType;
            room = player.room;
            if (room != null)
            {
                isPreparing = room.IsPreparing();
                UniqueRoomId = room.UniqueRoomId;
                roomSeed = room.roomSeed;
            }
        }
        public override void Write()
        {
            WriteH(3349);
            WriteD(isPreparing);
            if (!isPreparing)
            {
                return;
            }
            WriteD(player.slotId);
            WriteC(udpType);
            /* RENDEZVOUS (Udp not in operation) only for Tutorial mode;
             * CLIENT (all calculation is performed on player host client) only for AI and Infection mode;
             * RELAY (all calculation is performed on server) for other modes. */
            WriteIP(leader.ipAddress);
            WriteH(29890);
            WriteB(leader.localIP);
            WriteH(29890);
            WriteC(0);
            WriteIP(player.ipAddress);
            WriteH(29890);
            WriteB(player.localIP);
            WriteH(29890);
            WriteC(0);
            WriteIP(Settings.AddressExternal);
            WriteH((ushort)room.sessionPort); //Settings.PortBattle
            WriteD(UniqueRoomId);
            WriteD(roomSeed);
            if (loadHits)
            {
                //WriteC(BattleSettings.Unk1);
                //WriteC(BattleSettings.Unk2);
                //WriteC(BattleSettings.Unk3);
                //WriteC(BattleSettings.Unk4);
                //WriteC(BattleSettings.Unk5);
                //WriteC(BattleSettings.Unk6);
                //WriteC(BattleSettings.Unk7);
                //WriteC(BattleSettings.Unk8);
                //WriteC(BattleSettings.Unk9);
                //WriteC(BattleSettings.Unk10);
                //WriteC(BattleSettings.Unk11);
                //WriteC(BattleSettings.Unk12);
                //WriteC(BattleSettings.Unk13);
                //WriteC(BattleSettings.Unk14);
                //WriteC(BattleSettings.Unk15);
                //WriteC(BattleSettings.Unk16);
                //WriteC(BattleSettings.Unk17);
                //WriteC(BattleSettings.Unk18);
                //WriteC(BattleSettings.Unk19);
                //WriteC(BattleSettings.Unk20);
                //WriteC(BattleSettings.Unk21);
                //WriteC(BattleSettings.Unk22);
                //WriteC(BattleSettings.Unk23);
                //WriteC(BattleSettings.Unk24);
                //WriteC(BattleSettings.Unk25);
                //WriteC(BattleSettings.Unk26);
                //WriteC(BattleSettings.Unk27);
                //WriteC(BattleSettings.Unk28);
                //WriteC(BattleSettings.Unk29);
                //WriteC(BattleSettings.Unk30);
                //WriteC(BattleSettings.Unk31);
                //WriteC(BattleSettings.Unk32);
                //WriteC(BattleSettings.Unk33);
                //WriteC(BattleSettings.Unk34);
                //WriteC(BattleSettings.Unk35);

                //WriteB(room.HitParts);
                WriteB(new byte[35] //hitparts
				{
                    0x20, 0x15, 0x16, 0x17,
                    0x18, 0x19, 0x11, 0x1B,
                    0x1C, 0x1D, 0x1A, 0x1F,
                    0x09, 0x21, 0x0E, 0x1E,
                    0x01, 0x02, 0x03, 0x04,
                    0x05, 0x06, 0x07, 0x08,
                    0x14, 0x0A, 0x0B, 0x0C,
                    0x0D, 0x22, 0x0F, 0x10,
                    0x00, 0x12, 0x13
                });
            }
        }
    }
}