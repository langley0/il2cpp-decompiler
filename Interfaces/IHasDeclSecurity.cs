namespace ILSpy.Interfaces
{
    public interface IHasDeclSecurity : ICodedToken, IHasCustomAttribute, IFullName
    {
        int HasDeclSecurityTag { get; }
        IList<IDeclSecurity> DeclSecurities { get; }
        bool HasDeclSecurities { get; }
    }
}