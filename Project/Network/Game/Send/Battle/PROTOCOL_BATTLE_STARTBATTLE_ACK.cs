using System.Collections.Generic;

namespace PointBlank.Game
{
    public class BATTLE_STARTBATTLE_PAK : GamePacketWriter
    {
        private Room room;
        private Slot slot;
        private int isBattle, type;
        private List<int> dinos;
        public BATTLE_STARTBATTLE_PAK(Slot slot, Account pR, List<int> dinos, bool isBotMode, bool type)
        {
            this.slot = slot;
            room = pR.room;
            this.type = type ? 0 : 1;
            this.dinos = dinos;
            if (room != null)
            {
                isBattle = 1;
                if (!isBotMode)
                {
                    room.MissionCompleteBase(pR, slot, type ? MissionTypeEnum.STAGE_ENTER : MissionTypeEnum.STAGE_INTERCEPT, 0);
                }
            }
        }
        public BATTLE_STARTBATTLE_PAK()
        {
        }
        public override void Write()
        {
            WriteH(3334);
            WriteD(isBattle);
            if (isBattle == 1)
            {
                WriteD(slot.Id);
                WriteC((byte)type);
                WriteH(room.GetSlotsFlag(false, false));
                if (room.mode == RoomTypeEnum.Destruction || room.mode == RoomTypeEnum.Sabotage || room.mode == RoomTypeEnum.Suppression || room.mode == RoomTypeEnum.Defense)
                {
                    WriteH(room.redRounds);
                    WriteH(room.blueRounds);
                    if (room.mode != RoomTypeEnum.Sabotage && room.mode != RoomTypeEnum.Defense)
                    {
                        WriteH(room.GetSlotsFlag(true, false));
                    }
                    else
                    {
                        WriteH((ushort)room.Bar1);
                        WriteH((ushort)room.Bar2);
                        for (int i = 0; i < 16; i++)
                        {
                            WriteH(room.slots[i].damageBar1);
                        }
                        if (room.mode == RoomTypeEnum.Defense)
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                WriteH(room.slots[i].damageBar2);
                            }
                        }
                    }
                }
                else if (room.mode == RoomTypeEnum.Dino || room.mode == RoomTypeEnum.CrossCounter)
                {
                    WriteH(room.mode == RoomTypeEnum.CrossCounter ? room.redKills : room.redDino);
                    WriteH(room.mode == RoomTypeEnum.CrossCounter ? room.blueKills : room.blueDino);
                    WriteC(room.rounds);
                    WriteH(room.GetSlotsFlag(false, false)); //usa primeira lógica de slots EF AF (eu entrando 16 pessoas)
                    int TRex = dinos.Count == 1 || room.mode == RoomTypeEnum.CrossCounter ? 255 : room.TRex;
                    WriteC((byte)TRex); //T-Rex || 255 (não tem t-rex)
                    foreach (int slotId in dinos)
                    {
                        if ((slotId != room.TRex && room.mode == RoomTypeEnum.Dino) || room.mode == RoomTypeEnum.CrossCounter)
                        {
                            WriteC((byte)slotId);
                        }
                    }

                    int falta = 8 - dinos.Count - (TRex == 255 ? 1 : 0);
                    for (int i = 0; i < falta; i++)
                    {
                        WriteC(255);
                    }
                    WriteC(255);
                    WriteC(255);
                    WriteC(37); //89
                }
            }
        }
    }
}