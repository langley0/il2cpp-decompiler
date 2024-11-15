namespace ILSpy.Interfaces
{
    public interface ITypeDefFinder
    {
        ITypeDefOrRef Find(string fullName, bool isReflectionName);

        ITypeDefOrRef Find(ITypeDefOrRef typeRef);
    }
}