namespace ILSpy.Interfaces
{
    public interface ICustomAttributeType : ICodedToken, IHasCustomAttribute, IMethod
    {
        int CustomAttributeTypeTag { get; }
    }
}