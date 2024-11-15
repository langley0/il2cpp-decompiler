using System.Collections;

namespace ILSpy
{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

    public class LazyList<TValue> : ILazyList<TValue> where TValue : class
    {
        protected class Element(TValue value)
        {
            public Element()
            : this(default)
            {

            }

            public virtual bool IsInitialized => true;
            protected TValue value = value;
            public override string ToString() => value?.ToString() ?? string.Empty;

            public virtual TValue GetValue() => value;
            public virtual void SetValue(TValue value) => this.value = value;
        }

        public struct Enumerator : IEnumerator<TValue>
        {
            readonly LazyList<TValue> list;
            readonly int id;
            int index;

            internal Enumerator(LazyList<TValue> list)
            {
                this.list = list;
                index = -1;

                list.listLock.EnterReadLock();
                try
                {
                    id = list.id;

                }
                finally
                {
                    list.listLock.ExitReadLock();
                }

            }

            public readonly TValue Current
            {
                get
                {
                    return list.list[index].GetValue();
                }
            }

            readonly object IEnumerator.Current
            {
                get { return Current; }
            }

            public readonly void Dispose()
            {
            }

            public bool MoveNext()
            {
                list.listLock.EnterWriteLock();
                try
                {
                    if (list.id == id && index < list.Count)
                    {
                        index++;
                        return true;
                    }
                    else
                    {
                        if (list.id != id)
                        {
                            throw new InvalidOperationException("List was modified");
                        }
                        return false;
                    }
                }
                finally
                {
                    list.listLock.ExitWriteLock();
                }
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }


        protected readonly List<Element> list;
        protected readonly IListListener<TValue>? listener;
        readonly Lock listLock = Lock.Create();
        int id = 0;

        public TValue this[int index]
        {
            get
            {
                listLock.EnterWriteLock();
                try
                {
                    return list[index].GetValue();
                }
                finally
                {
                    listLock.ExitWriteLock();
                }
            }
            set
            {
                try
                {
                    listener?.OnRemove(index, list[index].GetValue());
                    listener?.OnAdd(index, value);
                    list[index].SetValue(value);
                    id++;
                }
                finally
                {
                    listLock.ExitWriteLock();
                }
            }
        }

        public int IndexOf(TValue item)
        {
            listLock.EnterWriteLock();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].GetValue() == item)
                        return i;
                }
                return -1;
            }
            finally
            {
                listLock.ExitWriteLock();
            }
        }

        public void Insert(int index, TValue item)
        {
            listLock.EnterWriteLock();
            try
            {
                listener?.OnAdd(index, item);
                list.Insert(index, new Element(item));
                listener?.OnResize(index);
                id++;
            }
            finally
            {
                listLock.ExitWriteLock();
            }
        }

        public void RemoveAt(int index)
        {
            listLock.EnterWriteLock();
            try
            {
                listener?.OnRemove(index, list[index].GetValue());
                list.RemoveAt(index);
                listener?.OnResize(index);
                id++;
            }
            finally
            {
                listLock.ExitWriteLock();
            }
        }

        public bool Remove(TValue item)
        {
            listLock.EnterWriteLock();
            try
            {
                int index = IndexOf(item);
                if (index < 0)
                {
                    return false;
                }
                RemoveAt(index);
                return true;
            }
            finally
            {
                listLock.ExitWriteLock();
            }
        }

        public void Add(TValue item)
        {
            listLock.EnterWriteLock();
            try
            {
                int index = list.Count;
                listener?.OnAdd(index, item);
                list.Add(new Element(item));
                listener?.OnResize(index);
                id++;
            }
            finally
            {
                listLock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            listLock.EnterWriteLock();
            try
            {
                listener?.OnClear();
                list.Clear();
                listener?.OnResize(0);
                id++;
            }
            finally
            {
                listLock.ExitWriteLock();
            }
        }

        public bool Contains(TValue item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            listLock.EnterWriteLock();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    array[arrayIndex + i] = list[i].GetValue();
                }
            }
            finally
            {
                listLock.ExitWriteLock();
            }
        }

        public int Count
        {
            get
            {
                listLock.EnterReadLock();
                try
                {

                    return list.Count;

                }
                finally
                {
                    listLock.ExitReadLock();
                }
            }
        }

        public bool IsReadOnly => false;

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        public LazyList()
        : this(null)
        {
        }

        public LazyList(IListListener<TValue>? listener)
        {
            this.listener = listener;
            list = [];
        }

        protected LazyList(int length, IListListener<TValue>? listener)
        {
            this.listener = listener;
            list = new List<Element>(length);
        }
    }

    public class LazyList<TValue, TContext> : LazyList<TValue>, ILazyList<TValue> where TValue : class
    {
        readonly TContext context;
        readonly Func<TContext, int, TValue> readOriginalValue;

        sealed class LazyElement : Element
        {
            internal readonly int origIndex;
            LazyList<TValue, TContext>? lazyList;

            public override bool IsInitialized => lazyList is null;

            public override TValue GetValue()
            {
                if (lazyList is not null)
                {
                    value = lazyList.ReadOriginalValue(this);
                    lazyList = null;
                }
                return value;
            }

            public override void SetValue(TValue value)
            {
                this.value = value;
                lazyList = null;
            }

            public LazyElement(int origIndex, LazyList<TValue, TContext> lazyList)
                : base(null)
            {
                this.origIndex = origIndex;
                this.lazyList = lazyList;
            }

            public override string ToString()
            {
                if (lazyList is not null)
                {
                    value = lazyList.ReadOriginalValue(this);
                    lazyList = null;
                }
                return value?.ToString() ?? string.Empty;
            }
        }

        public LazyList(int length, TContext context, Func<TContext, int, TValue> readOriginalValue)
            : this(length, null, context, readOriginalValue)
        {
        }

        public LazyList(int length, IListListener<TValue>? listener, TContext context, Func<TContext, int, TValue> readOriginalValue)
            : base(length, listener)
        {
            this.context = context;
            this.readOriginalValue = readOriginalValue;
            for (int i = 0; i < length; i++)
            {
                list.Add(new LazyElement(i, this));
            }
        }

        TValue ReadOriginalValue(LazyElement elem)
        {
            return ReadOriginalValue(list.IndexOf(elem), elem.origIndex);
        }

        TValue ReadOriginalValue(int index, int origIndex)
        {
            var newValue = readOriginalValue(context, origIndex);
            listener?.OnLazyAdd(index, ref newValue);
            return newValue;
        }
    }
}