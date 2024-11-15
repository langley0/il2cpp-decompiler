namespace ILSpy.Interfaces
{
	public interface ITypeDefOrRef : ICodedToken, IHasCustomAttribute, IMemberRefParent, IType, ITokenOperand, IMemberRef {
		int TypeDefOrRefTag { get; }

		public ITypeDef ResolveTypeDef();

		ITypeSig ToTypeSig();
	}
}