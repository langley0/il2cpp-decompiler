using System.Reflection.Metadata.Ecma335;

namespace ILSpy.IO
{
	public class DataReader
	{
		static void ThrowInvalidArgument(string paramName) => throw new DataReaderException("Invalid argument value");
		static void ThrowInvalidOperationException() => throw new InvalidOperationException();

		public uint StartOffset { get; private set; }
		public uint EndOffset { get; private set; }
		public uint Length { get { return EndOffset - StartOffset; } }
		public uint CurrentOffset { get; private set; }
		public DataStream Stream { get; private set; }

		public uint Position
		{
			get => CurrentOffset - StartOffset;
			set
			{
				if (value > Length)
				{
					// Invalid positions should be an IOException and not an ArgumentException
					throw new DataReaderException($"Invalid new {nameof(Position)}");
				}
				CurrentOffset = StartOffset + value;
			}
		}

		public uint BytesLeft
		{
			get
			{
				return EndOffset - CurrentOffset;
			}
		}

		public DataReader(DataStream stream, uint offset, uint length)
		{
			this.Stream = stream;
			this.StartOffset = offset;
			this.EndOffset = offset + length;
			this.CurrentOffset = offset;
		}

		public DataReader Slice(uint start, uint length)
		{
			if ((ulong)start + length > Length)
			{
				ThrowInvalidArgument(nameof(length));
			}
			return new DataReader(Stream, StartOffset + start, length);
		}

		public DataReader Slice(uint start)
		{
			if (start > Length)
			{
				ThrowInvalidArgument(nameof(start));
			}
			return Slice(start, Length - start);
		}

		public DataReader Slice(int start, int length)
		{
			if (start < 0)
			{
				ThrowInvalidArgument(nameof(start));
			}
			if (length < 0)
			{
				ThrowInvalidArgument(nameof(length));
			}
			return Slice((uint)start, (uint)length);
		}

		public DataReader Slice(int start)
		{
			if (start < 0)
			{
				ThrowInvalidArgument(nameof(start));
			}
			if ((uint)start > Length)
			{
				ThrowInvalidArgument(nameof(start));
			}
			return Slice((uint)start, Length - (uint)start);
		}

		public bool CanRead(int length) => length >= 0 && (uint)length <= BytesLeft;

		public bool CanRead(uint length) => length <= BytesLeft;

		public byte ReadByte()
		{
			const uint SIZE = 1;
			VerifyReadable(SIZE);

			var value = Stream.ReadByte(CurrentOffset);
			CurrentOffset += SIZE;
			return value;
		}

		public byte[] ReadBytes(int length)
		{
			if (length < 0)
			{
				ThrowInvalidArgument(nameof(length));
			}
			if (length == 0)
			{
				return [];
			}
			var data = new byte[length];
			ReadBytes(data, 0, length);
			return data;
		}

		public void ReadBytes(byte[] destination, int destinationIndex, int length)
		{
			if (destinationIndex < 0)
			{
				ThrowInvalidArgument(nameof(destinationIndex));
			}
			if (length < 0)
			{
				ThrowInvalidArgument(nameof(length));
			}

			// This is also true if 'this' is the 'default' instance ('stream' is null)
			if (length == 0)
			{
				return;
			}

			VerifyReadable((uint)length);

			Stream.ReadBytes(CurrentOffset, destination, destinationIndex, length);
			CurrentOffset += (uint)length;
		}

		public short ReadInt16()
		{
			const uint SIZE = 2;
			VerifyReadable(SIZE);

			var value = Stream.ReadInt16(CurrentOffset);
			CurrentOffset += SIZE;
			return value;
		}

		public ushort ReadUInt16()
		{
			const uint SIZE = 2;
			VerifyReadable(SIZE);

			var value = Stream.ReadUInt16(CurrentOffset);
			CurrentOffset += SIZE;
			return value;
		}

		public int ReadInt32()
		{
			const uint SIZE = 4;
			VerifyReadable(SIZE);

			var value = Stream.ReadInt32(CurrentOffset);
			CurrentOffset += SIZE;
			return value;
		}

		public uint ReadUInt32()
		{
			const uint SIZE = 4;
			VerifyReadable(SIZE);

			var value = Stream.ReadUInt32(CurrentOffset);
			CurrentOffset += SIZE;
			return value;
		}

		public long ReadInt64()
		{
			const uint SIZE = 8;
			VerifyReadable(SIZE);

			var value = Stream.ReadInt64(CurrentOffset);
			CurrentOffset += SIZE;
			return value;
		}

		public ulong ReadUInt64()
		{
			const uint SIZE = 8;
			VerifyReadable(SIZE);

			var value = Stream.ReadUInt64(CurrentOffset);
			CurrentOffset += SIZE;
			return value;
		}

		public float ReadSingle()
		{
			const uint SIZE = 4;
			VerifyReadable(SIZE);

			var value = Stream.ReadSingle(CurrentOffset);
			CurrentOffset += SIZE;
			return value;
		}

		public double ReadDouble()
		{
			const uint SIZE = 8;
			VerifyReadable(SIZE);

			var value = Stream.ReadDouble(CurrentOffset);
			CurrentOffset += SIZE;
			return value;
		}

		public Guid ReadGuid()
		{
			const uint SIZE = 16;
			VerifyReadable(SIZE);

			var value = Stream.ReadGuid(CurrentOffset);
			CurrentOffset += SIZE;
			return value;
		}

		public string ReadUtf16String(int chars)
		{
			if (chars < 0)
			{
				ThrowInvalidArgument(nameof(chars));
			}
			if (chars == 0)
			{
				return string.Empty;
			}

			uint length = (uint)chars * 2;
			VerifyReadable(length);

			var s = length == 0 ? string.Empty : Stream.ReadUtf16String(CurrentOffset, chars);
			CurrentOffset += length;
			return s;
		}




		private void VerifyReadable(uint size)
		{
			if (EndOffset - CurrentOffset < size)
			{
				throw new DataReaderException("There's not enough bytes left to read");
			}
		}

		public byte[]? TryReadBytesUntil(byte value)
		{
			var currentOffset = CurrentOffset;
			var endOffset = EndOffset;
			// This is also true if 'this' is the 'default' instance ('stream' is null)
			if (currentOffset == endOffset)
			{
				return null;
			}
			if (!Stream.TryGetOffsetOf(currentOffset, endOffset, value, out var valueOffset))
			{
				return null;
			}

			int length = (int)(valueOffset - currentOffset);
			if (length < 0)
			{
				return null;
			}
			return ReadBytes(length);
		}

		public bool TryReadCompressedUInt32(out uint value)
		{
			var currentOffset = CurrentOffset;
			var bytesLeft = EndOffset - currentOffset;
			if (bytesLeft == 0)
			{
				value = 0;
				return false;
			}

			var stream = Stream;
			byte b = stream.ReadByte(currentOffset++);
			if ((b & 0x80) == 0)
			{
				value = b;
				CurrentOffset = currentOffset;
				return true;
			}

			if ((b & 0xC0) == 0x80)
			{
				if (bytesLeft < 2)
				{
					value = 0;
					return false;
				}
				value = (uint)(((b & 0x3F) << 8) | stream.ReadByte(currentOffset++));
				CurrentOffset = currentOffset;
				return true;
			}

			// The encoding 111x isn't allowed but the CLR sometimes doesn't verify this
			// and just assumes it's 110x. Don't fail if it's 111x, just assume it's 110x.

			if (bytesLeft < 4)
			{
				value = 0;
				return false;
			}
			value = (uint)(((b & 0x1F) << 24) | (stream.ReadByte(currentOffset++) << 16) |
					(stream.ReadByte(currentOffset++) << 8) | stream.ReadByte(currentOffset++));
			CurrentOffset = currentOffset;
			return true;
		}

		public byte[] ToArray()
		{
			int length = (int)Length;
			if (length < 0)
			{
				ThrowInvalidOperationException();
			}
			// This is also true if 'this' is the 'default' instance ('stream' is null)
			if (length == 0)
			{
				return [];
			}

			var data = new byte[length];
			Stream.ReadBytes(StartOffset, data, 0, data.Length);
			return data;
		}

	}
}