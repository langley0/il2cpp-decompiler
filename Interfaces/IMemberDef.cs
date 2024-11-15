namespace ILSpy.Interfaces
{
    public interface IMemberDef : IDnlibDef, IMemberRef
    {
        new ITypeDefOrRef DeclaringType { get; }
    }
}