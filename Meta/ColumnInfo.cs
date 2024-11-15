using System.Diagnostics;
using ILSpy.IO;

namespace ILSpy.Meta
{
    public sealed class ColumnInfo {
		readonly byte index;
		byte offset;
		readonly ColumnType columnType;
		byte size;
		readonly string name;

		
		public int Index => index;

		public int Offset {
			get => offset;
			internal set => offset = (byte)value;
		}

		public int Size {
			get => size;
			internal set => size = (byte)value;
		}

		public string Name => name;

		public ColumnType ColumnType => columnType;

		public ColumnInfo(byte index, string name, ColumnType columnType) {
			this.index = index;
			this.name = name;
			this.columnType = columnType;
		}

		public ColumnInfo(byte index, string name, ColumnType columnType, byte offset, byte size) {
			this.index = index;
			this.name = name;
			this.columnType = columnType;
			this.offset = offset;
			this.size = size;
		}

		public uint Read(DataReader reader) =>
			size switch {
				1 => reader.ReadByte(),
				2 => reader.ReadUInt16(),
				4 => reader.ReadUInt32(),
				_ => throw new InvalidOperationException("Invalid column size"),
			};

		internal uint Read24(ref DataReader reader) {
			Debug.Assert(size == 2 || size == 4);
			return size == 2 ? reader.ReadUInt16() : reader.ReadUInt32();
		}

		// public void Write(DataWriter writer, uint value) {
		// 	switch (size) {
		// 	case 1: writer.WriteByte((byte)value); break;
		// 	case 2: writer.WriteUInt16((ushort)value); break;
		// 	case 4: writer.WriteUInt32(value); break;
		// 	default: throw new InvalidOperationException("Invalid column size");
		// 	}
		// }

		// internal void Write24(DataWriter writer, uint value) {
		// 	Debug.Assert(size == 2 || size == 4);
		// 	if (size == 2)
		// 		writer.WriteUInt16((ushort)value);
		// 	else
		// 		writer.WriteUInt32(value);
		// }
	}
}