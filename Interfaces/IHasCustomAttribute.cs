namespace ILSpy.Interfaces
{
	public interface IHasCustomAttribute : ICodedToken
	{
		int HasCustomAttributeTag { get; }

		ICustomAttributeCollection CustomAttributes { get; }

		bool HasCustomAttributes { get; }
	}
}