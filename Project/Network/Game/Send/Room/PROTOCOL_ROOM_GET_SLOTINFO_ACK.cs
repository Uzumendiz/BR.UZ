using System;

namespace PointBlank.Game
{
    public class PROTOCOL_ROOM_GET_SLOTINFO_ACK : GamePacketWriter
    {
        private Room room;
        public PROTOCOL_ROOM_GET_SLOTINFO_ACK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            try
            {
                WriteH(3861);
                if (room.GetLeader() == null)
                {
                    room.SetNewLeader(-1, 0, room.leaderSlot, false);
                }
                WriteD(room.leaderSlot);
                for (int i = 0; i < 16; i++)
                {
                    Slot slot = room.slots[i];
                    Account playerSlot = room.GetPlayerBySlot(slot);
                    if (playerSlot != null)
                    {
                        if (i == room.leaderSlot)
                        {
                            room.leaderName = playerSlot.nickname;
                        }
                        Clan clan = ClanManager.GetClan(playerSlot.clanId);
                        WriteC((byte)slot.state);
                        WriteC((byte)playerSlot.GetRank());
                        WriteD(clan.id);
                        WriteD((int)playerSlot.clanAuthority);
                        WriteC(clan.rank);
                        WriteD(clan.logo);
                        WriteC(playerSlot.pccafe);
                        WriteC(playerSlot.tourneyLevel);
                        WriteD((uint)playerSlot.effects);
                        //writeC((byte)pR.effect_1); //Lista de cupons 1 [1 - 90% Colete do BOPE Reforçado || 2 - Ketupat || 4 - 20% Colete Reforçado || 8 - Hollow Point Ammo Plus || 16 - 10% Colete Plus || 32 - 5% HP || 64 - Hollowpoint F. || 128 - Explosivo extra]
                        //writeC((byte)pR.effect_2); //Lista de cupons 2 [1 - C4 Speed || 2 - Hollowpoint || 4 - Bala de Ferro || 8 - 5% Colete || 16 - +1s piscando || 32 - +10% HP || 64 - Recarregamento rápido || 128 - Troca rápida] / [1/2/4/8/16/32/64/128]
                        //writeC((byte)pR.effect_3); //Lista de cupons 3 [1 - Flash Bang Protection || 2 - Receber drop || 4 - +40% de munição || 16 - 30% Respawn || 32 - +50% Respawn || 64 - +100% Respawn || 128 - +10% de munição]  [1/2/4/8/16/32/64/128]
                        //writeC((byte)pR.effect_4); //Lista de cupons 4 [1 - Item especial extra || 4 - Bala de ferro]
                        //writeC((byte)pR.effect_5); //Lista de cupons 5 [2 - Receber drop] - DEAD?
                        WriteS(clan.name, 17);
                        WriteD(0);
                        WriteC(playerSlot.country);
                    }
                    else
                    {
                        WriteC((byte)slot.state);
                        WriteB(PackageDataManager.SLOTINFO);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}