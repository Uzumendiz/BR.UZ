using System.Collections.Generic;

namespace PointBlank.Game
{
    public class BATTLE_ROUND_RESTART_PAK : GamePacketWriter
    {
        private Room room;
        private List<int> dinos;
        private bool isBotMode;
        public BATTLE_ROUND_RESTART_PAK(Room room, List<int> dinos, bool isBotMode)
        {
            this.room = room;
            this.dinos = dinos;
            this.isBotMode = isBotMode;
        }
        public BATTLE_ROUND_RESTART_PAK(Room room)
        {
            this.room = room;
            dinos = room.GetDinossaurs(false, -1);
            isBotMode = room.IsBotMode();
        }
        public override void Write()
        {
            WriteH(3351);
            WriteH(room.GetSlotsFlag(false, false));
            if (room.mode == RoomTypeEnum.HeadHunter)
            {
                WriteB(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
            }
            else if (isBotMode)
            {
                WriteB(new byte[10]);
            }
            else if (room.mode == RoomTypeEnum.Dino || room.mode == RoomTypeEnum.CrossCounter)
            {
                int TRex = dinos.Count == 1 || room.mode == RoomTypeEnum.CrossCounter ? 255 : room.TRex;
                WriteC((byte)TRex); //T-Rex || 255 (não tem t-rex)
                if (room.mode == RoomTypeEnum.Dino || room.mode == RoomTypeEnum.CrossCounter)
                {
                    for (int i = 0; i < dinos.Count; i++)
                    {
                        int slot = dinos[i];
                        if (slot != room.TRex)
                        {
                            WriteC((byte)slot);
                        }
                    }
                }
                int falta = 8 - dinos.Count - (TRex == 255 ? 1 : 0);
                for (int i = 0; i < falta; i++)
                {
                    WriteC(255);
                }
                WriteC(255);
                WriteC(255);
            }
            else
            {
                WriteB(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 });//writeB(new byte[] { 255, 1, 255, 255, 255, 255, 255, 255, 255, 255 });
            }
            dinos = null;
        }
    }
}