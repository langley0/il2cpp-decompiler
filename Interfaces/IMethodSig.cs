namespace ILSpy.Interfaces
{
    public interface IMethodSig
    {
        bool ImplicitThis { get; }

        IList<ITypeSig> Params { get; }

        ITypeSig RetType { get; }

        bool HasThis { get; }

        IList<ITypeSig> ParamsAfterSentinel { get; }
    }
}