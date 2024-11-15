using ILSpy.IO;

namespace ILSpy.PE
{
    public interface IRvaFileOffsetConverter
    {
        RVA ToRVA(FileOffset offset);
        FileOffset ToFileOffset(RVA rva);
    }

}