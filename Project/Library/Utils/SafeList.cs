﻿using System.Collections.Generic;

namespace PointBlank
{
    public class SafeList<T>
    {
        public List<T> _list = new List<T>();
        private object _sync = new object();
        public void Add(T value)
        {
            lock (_sync)
            {
                _list.Add(value);
            }
        }
        public void Clear()
        {
            lock (_sync)
            {
                _list.Clear();
            }
        }
        public bool Contains(T value)
        {
            lock (_sync)
            {
                return _list.Contains(value);
            }
        }
        public int Count()
        {
            lock (_sync)
            {
                return _list.Count;
            }
        }
        public bool Remove(T value)
        {
            lock (_sync)
            {
                return _list.Remove(value);
            }
        }
    }
}