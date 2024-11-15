using System.Collections;

namespace ILSpy.Meta
{
	public readonly struct RidList : IEnumerable<uint>
	{
		readonly uint startRid;
		readonly uint length;
		readonly IList<uint>? rids;

		public static readonly RidList Empty = Create(0, 0);

		public static RidList Create(uint startRid, uint length) => new(startRid, length);

		public static RidList Create(IList<uint> rids) => new(rids);

		RidList(uint startRid, uint length)
		{
			this.startRid = startRid;
			this.length = length;
			rids = null;
		}

		RidList(IList<uint>? rids)
		{
			this.rids = rids ?? throw new ArgumentNullException(nameof(rids));
			startRid = 0;
			length = (uint)rids.Count;
		}

		public uint this[int index]
		{
			get
			{
				if (rids is not null)
				{
					if ((uint)index >= (uint)rids.Count)
					{
						return 0;
					}
					return rids[index];
				}
				else
				{
					if ((uint)index >= length)
					{
						return 0;
					}
					return startRid + (uint)index;
				}
			}
		}

		public int Count => (int)length;

		public struct Enumerator : IEnumerator<uint>
		{
			readonly uint startRid;
			readonly uint length;
			readonly IList<uint>? rids;
			uint index;
			uint current;

			internal Enumerator(RidList list)
			{
				startRid = list.startRid;
				length = list.length;
				rids = list.rids;
				index = 0;
				current = 0;
			}

			public readonly uint Current => current;

            readonly object IEnumerator.Current => current;

			public readonly void Dispose() { }
			
			public bool MoveNext()
			{
				if (rids is null && index < length)
				{
					current = startRid + index;
					index++;
					return true;
				}
				return MoveNextOther();
			}

			bool MoveNextOther()
			{
				if (index >= length)
				{
					current = 0;
					return false;
				}
				if (rids is not null)
					current = rids[(int)index];
				else
					current = startRid + index;
				index++;
				return true;
			}

			void IEnumerator.Reset() => throw new NotSupportedException();
		}

		public Enumerator GetEnumerator() => new Enumerator(this);
		IEnumerator<uint> IEnumerable<uint>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}