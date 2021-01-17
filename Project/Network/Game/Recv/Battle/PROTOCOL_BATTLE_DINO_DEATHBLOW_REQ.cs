using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BATTLE_DINO_DEATHBLOW_REQ : GamePacketReader
    {
        private int weaponId;
        private int TRex;
        public override void ReadImplement()
        {
            TRex = ReadByte();
            weaponId = ReadInt();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (Settings.UdpType == UdpStateEnum.CLIENT && room != null && room.round.Timer == null && room.state == RoomStateEnum.Battle && room.TRex == TRex)
                {
                    Slot slot = room.GetSlot(player.slotId);
                    if (slot == null || slot.state != SlotStateEnum.BATTLE)
                    {
                        return;
                    }
                    if (slot.teamId == 0)
                    {
                        room.redDino += 5;
                    }
                    else
                    {
                        room.blueDino += 5;
                    }
                    using (BATTLE_DINO_PLACAR_PAK packet = new BATTLE_DINO_PLACAR_PAK(room))
                    {
                        room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                    }
                    room.MissionCompleteBase(player, slot, MissionTypeEnum.DEATHBLOW, weaponId);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}