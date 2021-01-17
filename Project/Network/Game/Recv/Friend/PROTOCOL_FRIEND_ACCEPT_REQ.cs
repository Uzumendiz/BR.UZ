using System;

namespace PointBlank.Game
{
    public class PROTOCOL_FRIEND_ACCEPT_REQ : GamePacketReader
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
                if (player == null)
                {
                    return;
                }
                Friend friend = player.friends.GetFriend(index); //Procura e obtém o Friend na lista de friends de quem aceitou pelo (Index) recebido.
                if (friend != null && friend.state > 0)
                {
                    Account accountFriend = AccountManager.GetAccount(friend.playerId, 32); //Procura e obtém o Account do friend aceito.
                    if (accountFriend != null)
                    {
                        if (friend.player == null)
                        {
                            //Adiciona informações do Account do friend aceito.
                            friend.SetModel(accountFriend.playerId, accountFriend.rankId, accountFriend.nickname, accountFriend.isOnline, accountFriend.status);
                        }
                        else
                        {              
                            //Atualiza informações do Account do friend aceito.
                            friend.player.SetInfo(accountFriend.rankId, accountFriend.nickname, accountFriend.isOnline, accountFriend.status);
                        }
                        friend.state = 0;
                        player.UpdateFriendState(friend);
                        client.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeStateEnum.Accept, null, friend.state, index));
                        client.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeStateEnum.Update, friend, index));

                        Friend friendPlayer = accountFriend.friends.GetFriend(player.playerId, out int idx); //Procura e obtém o Friend de quem aceitou na lista de quem enviou o convite.
                        if (friendPlayer != null && friendPlayer.state > 0)
                        {
                            if (friendPlayer.player == null)
                            {   
                                //Adiciona informações do Account do friend de quem aceitou.
                                friendPlayer.SetModel(player.playerId, player.rankId, player.nickname, player.isOnline, player.status);
                            }
                            else
                            {
                                //Atualiza informações do Account do friend de quem aceitou.
                                friendPlayer.player.SetInfo(player.rankId, player.nickname, player.isOnline, player.status);
                            }
                            friendPlayer.state = 0;
                            accountFriend.UpdateFriendState(friendPlayer);
                            accountFriend.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeStateEnum.Update, friendPlayer, idx));
                        }
                        client.SendCompletePacket(PackageDataManager.FRIEND_ACCEPT_SUCCESS_PAK);
                    }
                    else
                    {
                        client.SendCompletePacket(PackageDataManager.FRIEND_ACCEPT_ERROR_PAK);//STR_TBL_GUI_BASE_NO_USER_IN_USERLIST
                    }
                }
                else
                {
                    client.SendCompletePacket(PackageDataManager.FRIEND_ACCEPT_ERROR_PAK);//STR_TBL_GUI_BASE_NO_USER_IN_USERLIST
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}