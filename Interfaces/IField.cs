namespace ILSpy.Interfaces
{
    public interface IField : ICodedToken, ITokenOperand, IFullName, IMemberRef
    {
        IFieldSig FieldSig { get; set; }
    }
}