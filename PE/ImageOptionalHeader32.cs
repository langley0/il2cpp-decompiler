using ILSpy.IO;

namespace ILSpy.PE
{
	public sealed class ImageOptionalHeader32 : FileSection, IImageOptionalHeader
	{
		public ushort Magic { get; private set; }
		public byte MajorLinkerVersion { get; private set; }
		public byte MinorLinkerVersion { get; private set; }
		public uint SizeOfCode { get; private set; }
		public uint SizeOfInitializedData { get; private set; }
		public uint SizeOfUninitializedData { get; private set; }
		public RVA AddressOfEntryPoint { get; private set; }
		public RVA BaseOfCode { get; private set; }
		public RVA BaseOfData { get; private set; }
		public ulong ImageBase { get; private set; }
		public uint SectionAlignment { get; private set; }
		public uint FileAlignment { get; private set; }
		public ushort MajorOperatingSystemVersion { get; private set; }
		public ushort MinorOperatingSystemVersion { get; private set; }
		public ushort MajorImageVersion { get; private set; }
		public ushort MinorImageVersion { get; private set; }
		public ushort MajorSubsystemVersion { get; private set; }
		public ushort MinorSubsystemVersion { get; private set; }
		public uint Win32VersionValue { get; private set; }
		public uint SizeOfImage { get; private set; }
		public uint SizeOfHeaders { get; private set; }
		public uint CheckSum { get; private set; }
		public Subsystem Subsystem { get; private set; }
		public DllCharacteristics DllCharacteristics { get; private set; }
		public ulong SizeOfStackReserve { get; private set; }
		public ulong SizeOfStackCommit { get; private set; }
		public ulong SizeOfHeapReserve { get; private set; }
		public ulong SizeOfHeapCommit { get; private set; }
		public uint LoaderFlags { get; private set; }
		public uint NumberOfRvaAndSizes { get; private set; }
		public ImageDataDirectory[] DataDirectories { get; private set; }

		public ImageOptionalHeader32(DataReader reader, uint totalSize)
		{
			if (totalSize < 0x60)
			{
				throw new BadImageFormatException("Invalid optional header size");
			}
			// 최대 사이즈로 16을 사용한다
			DataDirectories = new ImageDataDirectory[16];

			SetStartOffset(reader.CurrentOffset);
			Magic = reader.ReadUInt16();
			MajorLinkerVersion = reader.ReadByte();
			MinorLinkerVersion = reader.ReadByte();
			SizeOfCode = reader.ReadUInt32();
			SizeOfInitializedData = reader.ReadUInt32();
			SizeOfUninitializedData = reader.ReadUInt32();
			AddressOfEntryPoint = (RVA)reader.ReadUInt32();
			BaseOfCode = (RVA)reader.ReadUInt32();
			BaseOfData = (RVA)reader.ReadUInt32();
			ImageBase = reader.ReadUInt32();
			SectionAlignment = reader.ReadUInt32();
			FileAlignment = reader.ReadUInt32();
			MajorOperatingSystemVersion = reader.ReadUInt16();
			MinorOperatingSystemVersion = reader.ReadUInt16();
			MajorImageVersion = reader.ReadUInt16();
			MinorImageVersion = reader.ReadUInt16();
			MajorSubsystemVersion = reader.ReadUInt16();
			MinorSubsystemVersion = reader.ReadUInt16();
			Win32VersionValue = reader.ReadUInt32();
			SizeOfImage = reader.ReadUInt32();
			SizeOfHeaders = reader.ReadUInt32();
			CheckSum = reader.ReadUInt32();
			Subsystem = (Subsystem)reader.ReadUInt16();
			DllCharacteristics = (DllCharacteristics)reader.ReadUInt16();
			SizeOfStackReserve = reader.ReadUInt32();
			SizeOfStackCommit = reader.ReadUInt32();
			SizeOfHeapReserve = reader.ReadUInt32();
			SizeOfHeapCommit = reader.ReadUInt32();
			LoaderFlags = reader.ReadUInt32();
			NumberOfRvaAndSizes = reader.ReadUInt32();

			// DataDirectories 의 각각의 의미
			// 0 : Export Table
			// 1 : Import Table
			// 2 : Resource Table
			// 3 : Exception Table
			// 4 : Certificate Table
			// 5 : Base Relocation Table
			// 6 : Debug
			// 7 : Architecture
			// 8 : Global Ptr
			// 9 : TLS Table
			// 10 : Load Config Table
			// 11 : Bound Import
			// 12 : IAT
			// 13 : Delay Import Descriptor
			// 14 : .NET Metadata
			// 15 : Reserved
			for (int i = 0; i < DataDirectories.Length; i++)
			{
				uint len = reader.Position - (uint)StartOffset;
				if (len + 8 <= totalSize)
				{
					DataDirectories[i] = new ImageDataDirectory(reader);
				}
				else
				{
					DataDirectories[i] = new ImageDataDirectory();
				}
			}
			reader.Position = (uint)StartOffset + totalSize;
			SetEndOffset(reader.CurrentOffset);
		}
	}
}