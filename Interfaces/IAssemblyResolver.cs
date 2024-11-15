namespace ILSpy.Interfaces 
{
    public interface IAssemblyResolver {
		IAssemblyDef? Resolve(IAssembly assembly, IModuleDef sourceModule);
	}

}