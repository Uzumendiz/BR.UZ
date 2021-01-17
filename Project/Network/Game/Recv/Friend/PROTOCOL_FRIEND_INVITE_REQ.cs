using System;

namespace PointBlank.Game
{
    public class PROTOCOL_FRIEND_INVITE_REQ : GamePacketReader
    {
        private string playerName;
        public override void ReadImplement()
        {
            playerName = ReadString(33);
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                DateTime now = DateTime.Now;
                if (player == null || playerName.Length == 0 || player.nickname.Length == 0 || player.nickname == playerName || (now - player.lastFriendInvite).TotalSeconds < 1)
                {
                    client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_ERROR_0x80001037_PAK);
                }
                else if (player.friends.friendsCache.Count >= 50)
                {
                    client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_ERROR_0x80001038_PAK);
                }
                else
                {
                    Account accountFriend = AccountManager.GetAccount(playerName, 32);
                    if (accountFriend != null)
                    {
                        if (player.friends.GetFriendIdx(accountFriend.playerId) == 255) //-1
                        {
                            if (accountFriend.friends.friendsCache.Count >= 50)
                            {
                                client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_ERROR_0x80001038_PAK);
                            }
                            else
                            {
                                int error1 = accountFriend.AddFriend(player, 2); //Pedido recebido                                
                                if (error1 == -1)
                                {
                                    client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_ERROR_0x80001039_PAK);
                                    return;
                                }
                                int error2 = player.AddFriend(accountFriend, 1); //Aguardando confirmação (error1 == 1 ? 0 : 1)
                                if (error2 == -1)
                                {
                                    client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_ERROR_0x80001039_PAK);
                                    return;
                                }
                                Friend f1 = player.friends.GetFriend(accountFriend.playerId, out int idx1);
                                Friend f2 = accountFriend.friends.GetFriend(player.playerId, out int idx2);
                                if (f2 != null)
                                {
                                    accountFriend.SendPacket(new FRIEND_UPDATE_PAK(error1 == 0 ? FriendChangeStateEnum.Insert : FriendChangeStateEnum.Update, f2, idx2));
                                }
                                if (f1 != null)
                                {
                                    client.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeStateEnum.Insert, f1, idx1));
                                }
                                player.lastFriendInvite = now;
                            }
                        }
                        else
                        {
                            client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_ERROR_0x80001041_PAK);
                        }
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.FRIEND_INVITE_ERROR_0x80001042_PAK);
                    }
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}