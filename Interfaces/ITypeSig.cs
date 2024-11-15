using ILSpy.Enums;

namespace ILSpy.Interfaces
{
    public interface ITypeSig : IType
    {
        ITypeSig RemovePinnedAndModifiers();

        ElementType GetElementType();
    }
}