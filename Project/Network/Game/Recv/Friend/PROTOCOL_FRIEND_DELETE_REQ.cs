using System;

namespace PointBlank.Game
{
    public class PROTOCOL_FRIEND_DELETE_REQ : GamePacketReader
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
                if (player == null || (now - player.lastFriendDelete).TotalSeconds < 1)
                {
                    return;
                }
                Friend friendModel = player.friends.GetFriend(index); //Procura e obtém o Friend na lista de friends de quem aceitou pelo (Index) recebido.
                if (friendModel != null)
                {
                    player.DeleteFriend(friendModel.playerId);
                    Account accountFriend = AccountManager.GetAccount(friendModel.playerId, 32); //Procura e obtém o Account do friend removido.
                    if (accountFriend != null)
                    {
                        Friend friendPlayer = accountFriend.friends.GetFriend(player.playerId, out int idx);
                        if (friendPlayer != null)
                        {
                            friendPlayer.removed = true;
                            accountFriend.UpdateFriendRemovedAndState(friendPlayer);
                            accountFriend.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeStateEnum.Update, friendPlayer, idx));
                        }
                    }
                    if (player.friends.RemoveFriend(friendModel))
                    {
                        client.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeStateEnum.Delete, null, 0, index));
                        client.SendCompletePacket(PackageDataManager.FRIEND_REMOVE_SUCCESS_PAK);
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.FRIEND_REMOVE_ERROR_PAK);
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.FRIEND_REMOVE_ERROR_PAK);
                }
                client.SendPacket(new FRIEND_MY_FRIENDLIST_PAK(player.friends.friendsCache));
                player.lastFriendDelete = now;
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}