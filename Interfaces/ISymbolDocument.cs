namespace ILSpy.Interfaces
{
	public interface ISymbolDocument {
		public abstract string URL { get; }

		public abstract Guid Language { get; }

		public abstract Guid LanguageVendor { get; }

		public abstract Guid DocumentType { get; }

		public abstract Guid CheckSumAlgorithmId { get; }

		public abstract byte[] CheckSum { get; }

		public abstract IPdbCustomDebugInfo[] CustomDebugInfos { get; }

		public abstract IMetadataToken? MDToken { get; }
	}
}