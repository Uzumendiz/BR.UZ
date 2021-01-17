using System;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_FIND_USER_REQ : GamePacketReader
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
                DateTime now = DateTime.Now;
                if (player == null || nickname.Length == 0 || player.nickname.Length == 0 || player.nickname == nickname || (now - player.lastFindUser).TotalSeconds < 1)
                {
                    return;
                }
                Account user = AccountManager.GetAccount(nickname, 0);
                if (user == null)
                {
                    client.SendCompletePacket(PackageDataManager.AUTH_FIND_USER_2147489795_PAK);
                }
                //else if (!user.isOnline)
                //{
                //    client.SendCompletePacket(PackageDataManager.AUTH_FIND_USER_2147489796_PAK);
                //}
                else
                {
                    client.SendPacket(new AUTH_FIND_USER_PAK(user, 0));
                }
                player.lastFindUser = now;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}