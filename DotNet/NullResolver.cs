using ILSpy.Interfaces;

namespace ILSpy.DotNet
{
    public sealed class NullResolver : IAssemblyResolver, IResolver {

		public static readonly NullResolver Instance = new NullResolver();

		NullResolver() 
        {
		}

		/// <inheritdoc/>
		public IAssemblyDef? Resolve(IAssembly assembly, IModuleDef sourceModule) => null;

		/// <inheritdoc/>
		public ITypeDef? Resolve(ITypeRef typeRef, IModuleDef sourceModule) => null;

		/// <inheritdoc/>
		public IMemberForwarded? Resolve(IMemberRef memberRef) => null;
	}
}