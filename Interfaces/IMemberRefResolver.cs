namespace ILSpy.Interfaces
{
    public interface IMemberRefResolver
    {
        IMemberForwarded Resolve(IMemberRef memberRef);
    }
}