using System.Diagnostics;
using ILSpy.PE;

namespace ILSpy.IO
{
    public static class DataStreamFactory
    {
        static readonly bool supportsUnalignedAccesses = CalculateSupportsUnalignedAccesses();

        static bool CalculateSupportsUnalignedAccesses()
        {
            var machine = ProcessorArchUtils.GetProcessCpuArchitecture();
            switch (machine)
            {
                case Machine.I386:
                case Machine.AMD64:
                    return true;
                case Machine.ARMNT:
                case Machine.ARM64:
                    return false;
                default:
                    Debug.Fail($"Unknown CPU arch: {machine}");
                    return true;
            }
        }


        public static DataStream Create(byte[] data)
        {
            if (supportsUnalignedAccesses)
            {
                return new UnalignedNativeMemoryDataStream(data);
            }
            else
            {
                return new AlignedNativeMemoryDataStream(data);
            }
        }
    }
}