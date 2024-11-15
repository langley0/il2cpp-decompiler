namespace ILSpy.Interfaces
{
    public interface IMemberRefParent : ICodedToken, IHasCustomAttribute, IFullName
    {
        int MemberRefParentTag { get; }
    }
}