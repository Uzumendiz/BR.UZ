using System.Collections.Generic;

namespace PointBlank.Auth
{
    public class PROTOCOL_BASE_USER_FRIENDS_ACK : GamePacketWriter
    {
        private List<Friend> friends;
        public PROTOCOL_BASE_USER_FRIENDS_ACK(List<Friend> friends)
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
                PlayerInfo info = friend != null ? friend.player: null;
                if (info != null)
                {
                    int length = info.playerNickname.Length + 1;
                    WriteC((byte)length);
                    WriteS(info.playerNickname, length);
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
        }
    }
}