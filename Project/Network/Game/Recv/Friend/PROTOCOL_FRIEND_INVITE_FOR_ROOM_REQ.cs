using System;

namespace PointBlank.Game
{
    public class PROTOCOL_FRIEND_INVITE_FOR_ROOM_REQ : GamePacketReader
    {
        private int index;
        public override void ReadImplement()
        {
            index = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                DateTime now = DateTime.Now;
                if (player == null ||  (now - player.lastFriendInviteRoom).TotalSeconds < 1)
                {
                    return;
                }
                Friend friend = player.friends.GetFriend(index);
                if (friend == null)
                {
                    return;
                }
                Account accountFriend = AccountManager.GetAccount(friend.playerId, 32);
                if (accountFriend != null)
                {
                    if (accountFriend.status.serverId == 255 || accountFriend.status.serverId == 0)
                    {
                        client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_FOR_ROOM_ERROR_0x80003002_PAK);
                        return;
                    }
                    else if (accountFriend.matchSlot >= 0)
                    {
                        client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_FOR_ROOM_ERROR_0x80003003_PAK);
                        return;
                    }
                    byte pIdx = accountFriend.friends.GetFriendIdx(player.playerId);
                    if (pIdx == 255) //-1
                    {
                        client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_FOR_ROOM_ERROR_0x8000103E_PAK);
                    }
                    else if (accountFriend.isOnline)
                    {
                        accountFriend.SendPacket(new FRIEND_ROOM_INVITE_PAK(pIdx));
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_FOR_ROOM_ERROR_0x8000103F_PAK);
                    }
                    player.lastFriendInviteRoom = now;
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_FOR_ROOM_ERROR_0x8000103D_PAK);
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}