namespace ILSpy.Interfaces
{
    public interface IAssemblyDef : IHasCustomAttribute, IHasDeclSecurity, IHasCustomDebugInformation, IAssembly, IListListener<IModuleDef>, ITypeDefFinder, IDnlibDef
    {
    }
}