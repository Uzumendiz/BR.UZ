using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_TITLE_USE_REQ : GamePacketReader
    {
        private byte slotIdx;
        private byte titleId;
        public override void ReadImplement()
        {
            slotIdx = ReadByte();
            titleId = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null)
                {
                    return;
                }
                PlayerTitles titles = player.titles;
                TitleQ titleQ = TitlesManager.GetTitle(titleId);
                if (slotIdx >= 3 || titleId >= 45 || titleQ == null || titles.Equiped1 == titleId || titles.Equiped2 == titleId || titles.Equiped3 == titleId)
                {
                    client.SendCompletePacket(PackageDataManager.BASE_TITLE_USE_ERROR_PAK);
                    return;
                }
                TitlesManager.GetTitlesEquipped(titles.Equiped1, titles.Equiped2, titles.Equiped3, out TitleQ titleEquiped1, out TitleQ titleEquiped2, out TitleQ titleEquiped3);
                if (titleQ.classId == titleEquiped1.classId && slotIdx != 0 || titleQ.classId == titleEquiped2.classId && slotIdx != 1 || titleQ.classId == titleEquiped3.classId && slotIdx != 2 || !titles.Contains(titleQ.flag))
                {
                    client.SendCompletePacket(PackageDataManager.BASE_TITLE_USE_ERROR_PAK);
                    return;
                }
                if (player.UpdateEquipedTitle(slotIdx, titleId))
                {
                    titles.SetEquip(slotIdx, titleId);
                    client.SendCompletePacket(PackageDataManager.BASE_TITLE_USE_SUCCESS_PAK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.BASE_TITLE_USE_ERROR_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}