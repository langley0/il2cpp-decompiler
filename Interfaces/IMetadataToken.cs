namespace ILSpy.Interfaces
{
    public interface IMetadataToken : IEquatable<IMetadataToken>, IComparable<IMetadataToken>
    {
        uint Token { get; }
    }
}