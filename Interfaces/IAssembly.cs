using ILSpy.Enums;

namespace ILSpy.Interfaces
{
    public interface IAssembly : IFullName
    {
        Version Version { get; set; }
        AssemblyAttributes Attributes { get; set; }
        IPublicKey PublicKeyOrToken { get; }
        UTF8String Culture { get; set; }
        string FullNameToken { get; }
        bool HasPublicKey { get; set; }
        AssemblyAttributes ProcessorArchitecture { get; set; }
        AssemblyAttributes ProcessorArchitectureFull { get; set; }
        bool IsProcessorArchitectureNone { get; }
        bool IsProcessorArchitectureMSIL { get; }
        bool IsProcessorArchitectureX86 { get; }
        bool IsProcessorArchitectureIA64 { get; }
        bool IsProcessorArchitectureX64 { get; }
        bool IsProcessorArchitectureARM { get; }
        bool IsProcessorArchitectureNoPlatform { get; }
        bool IsProcessorArchitectureSpecified { get; set; }
        bool EnableJITcompileTracking { get; set; }
        bool DisableJITcompileOptimizer { get; set; }
        bool IsRetargetable { get; set; }
        AssemblyAttributes ContentType { get; set; }
        bool IsContentTypeDefault { get; }
        bool IsContentTypeWindowsRuntime { get; }
    }
}