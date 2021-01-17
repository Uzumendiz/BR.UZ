using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_MISSION_DEFENCE_INFO_REQ : GamePacketReader
    {
        private ushort tanqueA, tanqueB;
        private List<ushort> _damag1 = new List<ushort>();
        private List<ushort> _damag2 = new List<ushort>();
        public override void ReadImplement()
        {
            tanqueA = ReadUshort();
            tanqueB = ReadUshort();
            for (int i = 0; i < 16; i++)
            {
                _damag1.Add(ReadUshort());
            }
            for (int i = 0; i < 16; i++)
            {
                _damag2.Add(ReadUshort());
            }
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room != null && room.round.Timer == null && room.state == RoomStateEnum.Battle && !room.swapRound)
                {
                    Slot slot = room.GetSlot(player.slotId);
                    if (slot == null || slot.state != SlotStateEnum.BATTLE)
                    {
                        return;
                    }
                    room.Bar1 = tanqueA;
                    room.Bar2 = tanqueB;
                    for (int i = 0; i < 16; i++)
                    {
                        Slot slotR = room.slots[i];
                        if (slotR.playerId > 0 && slotR.state == SlotStateEnum.BATTLE)
                        {
                            slotR.damageBar1 = _damag1[i];
                            slotR.damageBar2 = _damag2[i];
                        }
                    }
                    using (BATTLE_MISSION_DEFENCE_INFO_PAK packet = new BATTLE_MISSION_DEFENCE_INFO_PAK(room))
                    {
                        room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                    }
                    if (tanqueA == 0 && tanqueB == 0)
                    {
                        room.swapRound = true;
                        room.redRounds++;
                        room.BattleEndRound(0, RoundEndTypeEnum.Normal);
                    }
                }
                _damag1 = null;
                _damag2 = null;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}