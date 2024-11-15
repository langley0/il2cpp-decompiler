namespace ILSpy.Interfaces
{
    public interface IGenericParameterProvider : ICodedToken, IIsTypeOrMethod
    {
        int NumberOfGenericParameters { get; }
    }
}