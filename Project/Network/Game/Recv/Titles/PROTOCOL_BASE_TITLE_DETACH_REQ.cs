using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_TITLE_DETACH_REQ : GamePacketReader
    {
        private int slotIdx;
        public override void ReadImplement()
        {
            slotIdx = ReadUshort();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || slotIdx >= 3)
                {
                    return;
                }
                PlayerTitles titles = player.titles;
                int titleId = titles.GetEquip(slotIdx);
                if (titleId > 0 && player.UpdateEquipedTitle(slotIdx, 0))
                {
                    titles.SetEquip(slotIdx, 0);
                    if (TitlesManager.Contains(titleId, player.equipments.beret) && player.ExecuteQuery($"UPDATE accounts SET character_beret='0' WHERE id='{player.playerId}'"))
                    {
                        player.equipments.beret = 0;
                        Room room = player.room;
                        if (room != null && room.GetSlot(player.slotId, out Slot slot))
                        {
                            slot.equipment = player.equipments;
                        }
                    }
                    client.SendCompletePacket(PackageDataManager.BASE_TITLE_DETACH_SUCCESS_PAK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BASE_TITLE_DETACH_ERROR_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}