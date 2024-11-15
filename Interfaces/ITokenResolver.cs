namespace ILSpy.Interfaces
{
public interface ITokenResolver {
		IMetadataTokenProvider ResolveToken(uint token, IGenericParamContext gpContext);
	}
}