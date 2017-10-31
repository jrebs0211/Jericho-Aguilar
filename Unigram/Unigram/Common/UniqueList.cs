﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Api.TL;

namespace Unigram.Common
{
    public class UniqueList<TKey, TValue> : IList<TValue>
    {
        private readonly Func<TValue, TKey> _selector;
        private readonly SortedList<TKey, TValue> _inner;

        public UniqueList(Func<TValue, TKey> selector)
        {
            _selector = selector;
            _inner = new SortedList<TKey, TValue>();
        }

        public TValue this[int index]
        {
            get
            {
                return _inner.Values[index];
            }
            set { }
        }

        public int Count
        {
            get
            {
                return _inner.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(TValue item)
        {
            var key = _selector(item);
            if (_inner.ContainsKey(key))
            {
                return;
            }

            _inner.Add(key, item);
        }

        public void Clear()
        {
            _inner.Clear();
        }

        public bool Contains(TValue item)
        {
            return _inner.ContainsKey(_selector(item));
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _inner.Values.GetEnumerator();
        }

        public int IndexOf(TValue item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, TValue item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TValue item)
        {
            return _inner.Remove(_selector(item));
        }

        public void RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _inner.Values.GetEnumerator();
        }
    }
}
