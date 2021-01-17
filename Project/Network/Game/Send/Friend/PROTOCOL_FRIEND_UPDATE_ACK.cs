namespace PointBlank.Game
{
    public class FRIEND_UPDATE_PAK : GamePacketWriter
    {
        private Friend friend;
        private int index;
        private FriendStateEnum state;
        private FriendChangeStateEnum type;
        public FRIEND_UPDATE_PAK(FriendChangeStateEnum type, Friend friend, int index)
        {
            this.type = type;
            state = (FriendStateEnum)friend.state;
            this.friend = friend;
            this.index = index;
        }
        public FRIEND_UPDATE_PAK(FriendChangeStateEnum type, Friend friend, int state, int index)
        {
            this.type = type;
            this.state = (FriendStateEnum)state;
            this.friend = friend;
            this.index = index;
        }
        public FRIEND_UPDATE_PAK(FriendChangeStateEnum type, Friend friend, FriendStateEnum state, int index)
        {
            this.type = type;
            this.state = state;
            this.friend = friend;
            this.index = index;
        }
        public override void Write()
        {
            WriteH(279);
            WriteC((byte)type); //Somente 1, 2 e 4.
            WriteC((byte)index);
            if (type == FriendChangeStateEnum.Insert || type == FriendChangeStateEnum.Update)
            {
                PlayerInfo info = friend != null ? friend.player : null;
                if (info == null)
                {
                    WriteB(new byte[17]);
                }
                else
                {
                    WriteC((byte)(info.playerNickname.Length + 1));
                    WriteS(info.playerNickname, info.playerNickname.Length + 1);
                    WriteQ(friend.playerId);
                    WriteD(Utilities.GetFriendStatus(friend, state));
                    WriteC(info.rank);
                    WriteC(0);
                    WriteH(0);
                }
            }
            else
            {
                WriteB(new byte[17]);
            }
        }
    }
}