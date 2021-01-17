using System.Collections.Generic;

namespace PointBlank.Game
{
    public class FRIEND_MY_FRIENDLIST_PAK : GamePacketWriter
    {
        private List<Friend> friends;
        public FRIEND_MY_FRIENDLIST_PAK(List<Friend> friends)
        {
            this.friends = friends;
        }

        public override void Write()
        {
            WriteH(274);
            WriteC((byte)friends.Count);
            for (int i = 0; i < friends.Count; i++)
            {
                Friend friend = friends[i];
                PlayerInfo info = friend.player;
                if (info != null)
                {
                    WriteC((byte)(info.playerNickname.Length + 1));
                    WriteS(info.playerNickname, info.playerNickname.Length + 1);
                    WriteQ(friend.playerId);
                    WriteD(Utilities.GetFriendStatus(friend));
                    WriteC(info.rank);
                    WriteH(0);
                    WriteC(0);
                }
                else
                {
                    WriteB(new byte[17]);
                }
            }
            friends = null;
        }
    }
}