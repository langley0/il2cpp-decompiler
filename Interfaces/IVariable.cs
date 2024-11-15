namespace ILSpy.Interfaces
{
    public interface IVariable
    {
        ITypeSig Type { get; }

        int Index { get; }

        string Name { get; set; }
    }
}