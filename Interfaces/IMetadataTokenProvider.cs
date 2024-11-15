namespace ILSpy.Interfaces
{
	public interface IMetadataTokenProvider
	{
		IMetadataToken Token { get; }
		uint Rid { get; }
	}
}