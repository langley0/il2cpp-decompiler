namespace ILSpy.Interfaces
{
    public interface IMemberForwarded : ICodedToken, IHasCustomAttribute, IFullName, IMemberRef
    {
        int MemberForwardedTag { get; }

        IMapper ImplMap { get; set; }

        bool HasImplMap { get; }
    }
}