using System;
using System.Collections.Generic;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_MISSION_GENERATOR_INFO_REQ : GamePacketReader
    {
        private ushort barRed, barBlue;
        private List<ushort> damages = new List<ushort>();
        public override void ReadImplement()
        {
            barRed = ReadUshort();
            barBlue = ReadUshort();
            for (int i = 0; i < 16; i++)
            {
                damages.Add(ReadUshort());
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
                    room.Bar1 = barRed;
                    room.Bar2 = barBlue;
                    for (int i = 0; i < 16; i++)
                    {
                        Slot slotR = room.slots[i];
                        if (slotR.playerId > 0 && slotR.state == SlotStateEnum.BATTLE)
                        {
                            slotR.damageBar1 = damages[i];
                            slotR.earnedXP = damages[i] / 600;
                        }
                    }
                    using (BATTLE_MISSION_GENERATOR_INFO_PAK packet = new BATTLE_MISSION_GENERATOR_INFO_PAK(room))
                    {
                        room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                    }
                    if (barRed == 0)
                    {
                        room.swapRound = true;
                        room.blueRounds++;
                        room.BattleEndRound(1, RoundEndTypeEnum.Normal);
                    }
                    else if (barBlue == 0)
                    {
                        room.swapRound = true;
                        room.redRounds++;
                        room.BattleEndRound(0, RoundEndTypeEnum.Normal);
                    }
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
            finally
            {
                damages = null;
            }
        }
    }
}