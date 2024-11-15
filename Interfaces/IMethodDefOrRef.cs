namespace ILSpy.Interfaces
{
    public interface IMethodDefOrRef : ICodedToken, IHasCustomAttribute, ICustomAttributeType, IMethod {
		int MethodDefOrRefTag { get; }
	}
}
