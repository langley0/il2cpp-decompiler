namespace ILSpy.Interfaces
{
    public interface IMethod : ICodedToken, ITokenOperand, IFullName, IGenericParameterProvider, IMemberRef
    {

        IMethodSig MethodSig { get; set; }
    }
}