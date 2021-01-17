using System.Collections.Generic;

namespace PointBlank
{
    public class SafeSortedList<TKey, TValue>
    {
        private SortedList<TKey, TValue> _list = new SortedList<TKey, TValue>();
        private object _sync = new object();
        public void Add(TKey key, TValue value)
        {
            lock (_sync)
            {
                _list.Add(key, value);
            }
        }
        public void Clear()
        {
            lock (_sync)
            {
                _list.Clear();
            }
        }
        public bool Contains(TKey key)
        {
            lock (_sync)
            {
                return _list.ContainsKey(key);
            }
        }
        public bool Remove(TKey key)
        {
            lock (_sync)
            {
                return _list.Remove(key);
            }
        }
    }
}