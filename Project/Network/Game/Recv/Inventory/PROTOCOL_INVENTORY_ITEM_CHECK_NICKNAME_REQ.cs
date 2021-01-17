using System;

namespace PointBlank.Game
{
    public class PROTOCOL_INVENTORY_ITEM_CHECK_NICKNAME_REQ : GamePacketReader
    {
        private string nickname;
        public override void ReadImplement()
        {
            nickname = ReadString(33);
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null || AccountManager.CheckNickLengthInvalid(nickname))
                {
                    client.SendCompletePacket(PackageDataManager.AUTH_CHECK_NICKNAME_ERROR_PAK);
                    return;
                }
                if (!StringFilter.CheckStringFilter(nickname) || AccountManager.CheckNicknameExist(nickname).Result)
                {
                    client.SendCompletePacket(PackageDataManager.AUTH_CHECK_NICKNAME_ERROR_PAK);
                    return;
                }
                if (nickname != player.nickname)
                {
                    client.SendCompletePacket(PackageDataManager.AUTH_CHECK_NICKNAME_SUCCESS_PAK);
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.AUTH_CHECK_NICKNAME_ERROR_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}