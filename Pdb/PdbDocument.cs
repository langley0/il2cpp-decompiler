using ILSpy.Interfaces;

namespace ILSpy.Pdb
{
    public sealed class PdbDocument : IHasCustomDebugInformation
    {
        public string Url { get; set; }

        public Guid Language { get; set; }

        public Guid LanguageVendor { get; set; }

        public Guid DocumentType { get; set; }

        public Guid CheckSumAlgorithmId { get; set; }

        public byte[] CheckSum { get; set; }

        /// <inheritdoc/>
        public int HasCustomDebugInformationTag => 22;

        /// <inheritdoc/>
        public bool HasCustomDebugInfos => CustomDebugInfos.Count > 0;

        public IList<IPdbCustomDebugInfo> CustomDebugInfos { get; private set; }

        public IMetadataToken? MDToken { get; internal set; }

        public PdbDocument(ISymbolDocument? symDoc)
            : this(symDoc, partial: false)
        {
        }

        public PdbDocument(ISymbolDocument? symDoc, bool partial)
        {
            ArgumentNullException.ThrowIfNull(symDoc);

            Url = symDoc.URL;
            if (!partial)
            {
                Language = symDoc.Language;
                LanguageVendor = symDoc.LanguageVendor;
                DocumentType = symDoc.DocumentType;
                CheckSumAlgorithmId = symDoc.CheckSumAlgorithmId;
                CheckSum = symDoc.CheckSum;
                CustomDebugInfos = [.. symDoc.CustomDebugInfos];
                MDToken = symDoc.MDToken;
                CheckSum = [];
            }
            else
            {
                CustomDebugInfos = [];
                CheckSum = [];
            }
        }


        public PdbDocument(string url, Guid language, Guid languageVendor, Guid documentType, Guid checkSumAlgorithmId, byte[] checkSum)
        {
            Url = url;
            Language = language;
            LanguageVendor = languageVendor;
            DocumentType = documentType;
            CheckSumAlgorithmId = checkSumAlgorithmId;
            CheckSum = checkSum;

            CustomDebugInfos = [];
        }


        internal static PdbDocument CreatePartialForCompare(ISymbolDocument symDoc)
        {
            return new(symDoc, partial: true);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Url ?? string.Empty);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not PdbDocument other)
            {
                return false;
            }
            return StringComparer.OrdinalIgnoreCase.Equals(Url ?? string.Empty, other.Url ?? string.Empty);
        }
    }
}