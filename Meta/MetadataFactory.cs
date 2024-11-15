using ILSpy.PE;
using ILSpy.Enums;

namespace ILSpy.Meta
{
	public static class MetadataFactory
	{
		public enum MetadataType
		{
			Unknown,
			Compressed, // #~ (normal)
			ENC,        // #- (edit and continue)
		}

		public static Metadata Load(string fileName, CLRRuntimeReaderKind runtime)
		{
			PEImage? peImage = null;
			try
			{
				peImage = new PEImage(fileName);
				return Load(peImage, runtime);
			}
			catch
			{
				peImage?.Dispose();
				throw;
			}
		}

		public  static Metadata Load(byte[] data, CLRRuntimeReaderKind runtime)
		{
			PEImage? peImage = null;
			try
			{
				return Load(peImage = new PEImage(data), runtime);
			}
			catch
			{
				if (peImage is not null)
					peImage.Dispose();
				throw;
			}
		}

		public static Metadata Load(IPEImage peImage, CLRRuntimeReaderKind runtime)
		{
			MetadataBase? md = null;
			try
			{
				var dotNetDir = peImage.ImageNTHeaders.OptionalHeader.DataDirectories[14];
				if (dotNetDir is null)
				{
					throw new BadImageFormatException("No .NET data directory found");
				}

				// Mono doesn't check that the Size field is >= 0x48
				if (dotNetDir.VirtualAddress == 0)
				{
					throw new BadImageFormatException(".NET data directory RVA is 0");
				}

				var cor20HeaderReader = peImage.CreateReader(dotNetDir.VirtualAddress, dotNetDir.Size);
				var cor20Header = new ImageCor20Header(cor20HeaderReader);
				if (cor20Header.Metadata.VirtualAddress == 0) {
					throw new BadImageFormatException(".NET metadata RVA is 0");
				}
				var mdRva = cor20Header.Metadata.VirtualAddress;
				// Don't use the size field, Mono ignores it. Create a reader that can read to EOF.
				var mdHeaderReader = peImage.CreateReader(mdRva);
				var mdHeader = new MetadataHeader(mdHeaderReader, runtime);


				md = GetMetadataType(mdHeader.StreamHeaders, runtime) switch
				{
					MetadataType.Compressed => new CompressedMetadata(peImage, cor20Header, mdHeader, runtime),
					MetadataType.ENC => new ENCMetadata(peImage, cor20Header, mdHeader, runtime),
					_ => throw new BadImageFormatException("No #~ or #- stream found"),
				};
				md.Initialize(null);

				return md;
			}
			catch
			{
				md?.Dispose();
				throw;
			}
		}

		static MetadataType GetMetadataType(IList<StreamHeader> streamHeaders, CLRRuntimeReaderKind runtime)
		{
			MetadataType? mdType = null;
			if (runtime == CLRRuntimeReaderKind.CLR)
			{
				foreach (var sh in streamHeaders)
				{
					if (mdType is null)
					{
						if (sh.Name == "#~")
							mdType = MetadataType.Compressed;
						else if (sh.Name == "#-")
							mdType = MetadataType.ENC;
					}
					if (sh.Name == "#Schema")
						mdType = MetadataType.ENC;
				}
			}
			else if (runtime == CLRRuntimeReaderKind.Mono)
			{
				foreach (var sh in streamHeaders)
				{
					if (sh.Name == "#~")
						mdType = MetadataType.Compressed;
					else if (sh.Name == "#-")
					{
						mdType = MetadataType.ENC;
						break;
					}
				}
			}
			else
				throw new ArgumentOutOfRangeException(nameof(runtime));
			if (mdType is null)
				return MetadataType.Unknown;
			return mdType.Value;
		}
	}
}