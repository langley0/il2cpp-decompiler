using ILSpy.IO;

namespace ILSpy.PE{
    interface IPEType {
        RVA ToRVA(PEInfo peInfo, FileOffset offset);
        FileOffset ToFileOffset(PEInfo peInfo, RVA rva);
    }
}