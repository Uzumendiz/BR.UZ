using System.Collections.Generic;

namespace PointBlank
{
    public class FriendSystem
    {
        public List<Friend> friendsCache = new List<Friend>(50);
        public bool MemoryCleaned;
        public void CleanList()
        {
            lock (friendsCache)
            {
                foreach (Friend friend in friendsCache)
                {
                    friend.player = null;
                }
            }
            MemoryCleaned = true;
        }

        public void AddFriend(Friend friend)
        {
            lock (friendsCache)
            {
                friendsCache.Add(friend);
            }
        }

        public bool RemoveFriend(Friend friend)
        {
            lock (friendsCache)
            {
                return friendsCache.Remove(friend);
            }
        }

        public void RemoveFriend(int index)
        {
            lock (friendsCache)
            {
                friendsCache.RemoveAt(index);
            }
        }

        public void RemoveFriend(long id)
        {
            lock (friendsCache)
            {
                for (int i = 0; i < friendsCache.Count; i++)
                {
                    if (friendsCache[i].playerId == id)
                    {
                        friendsCache.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public byte GetFriendIdx(long id)
        {
            lock (friendsCache)
            {
                for (byte i = 0; i < friendsCache.Count; i++)
                {
                    if (friendsCache[i].playerId == id)
                    {
                        return i;
                    }
                }
            }
            return 255; //-1
        }

        public Friend GetFriend(int idx)
        {
            lock (friendsCache)
            {
                try
                {
                    return friendsCache[idx];
                }
                catch
                {
                    return null;
                }
            }
        }

        public Friend GetFriend(long id)
        {
            lock (friendsCache)
            {
                for (int i = 0; i < friendsCache.Count; i++)
                {
                    Friend friend = friendsCache[i];
                    if (friend.playerId == id)
                    {
                        return friend;
                    }
                }
            }
            return null;
        }

        public Friend GetFriend(long id, out int index)
        {
            lock (friendsCache)
            {
                for (int i = 0; i < friendsCache.Count; i++)
                {
                    Friend friend = friendsCache[i];
                    if (friend.playerId == id)
                    {
                        index = i;
                        return friend;
                    }
                }
            }
            index = -1;
            return null;
        }
    }
}