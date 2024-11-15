using System.Reflection;
using ILSpy.Enums;
using ILSpy.Interfaces;

namespace ILSpy.DotNet
{
    public sealed class AssemblyNameInfo : IAssembly
    {
        Version version;
        AssemblyAttributes flags;
        IPublicKey publicKeyOrToken;
        UTF8String name;
        UTF8String culture;

        public AssemblyNameInfo(AssemblyName asmName)
        {
            ArgumentNullException.ThrowIfNull(asmName);

            version = asmName.Version ?? new Version(0, 0, 0, 0);
            flags = (AssemblyAttributes)asmName.Flags;
            // ensure that the public key or token is set
            IPublicKey? publicKeyOrToken = PublicKey.CreatePublicKey(asmName.GetPublicKey());
            publicKeyOrToken ??= PublicKeyToken.CreatePublicKeyToken(asmName.GetPublicKeyToken());
            ArgumentNullException.ThrowIfNull(publicKeyOrToken);

            this.publicKeyOrToken = publicKeyOrToken;

            name = asmName.Name ?? string.Empty;
            culture = asmName.CultureInfo is not null && asmName.CultureInfo.Name is not null ? asmName.CultureInfo.Name : string.Empty;
        }

        public Version Version
        {
            get => version;
            set => version = value;
        }

        public AssemblyAttributes Attributes
        {
            get => flags;
            set => flags = value;
        }

        public IPublicKey PublicKeyOrToken
        {
            get => publicKeyOrToken;
            set => publicKeyOrToken = value;
        }

        public UTF8String Name
        {
            get => name;
            set => name = value;
        }

        public UTF8String Culture
        {
            get => culture;
            set => culture = value;
        }

        public string FullName => FullNameToken;

        public string FullNameToken => FullNameFactory.AssemblyFullName(this, true);

        void ModifyAttributes(AssemblyAttributes andMask, AssemblyAttributes orMask) => Attributes = (Attributes & andMask) | orMask;

        void ModifyAttributes(bool set, AssemblyAttributes flags)
        {
            if (set)
                Attributes |= flags;
            else
                Attributes &= ~flags;
        }

        public bool HasPublicKey
        {
            get => (Attributes & AssemblyAttributes.PublicKey) != 0;
            set => ModifyAttributes(value, AssemblyAttributes.PublicKey);
        }

        public AssemblyAttributes ProcessorArchitecture
        {
            get => Attributes & AssemblyAttributes.PA_Mask;
            set => ModifyAttributes(~AssemblyAttributes.PA_Mask, value & AssemblyAttributes.PA_Mask);
        }

        public AssemblyAttributes ProcessorArchitectureFull
        {
            get => Attributes & AssemblyAttributes.PA_FullMask;
            set => ModifyAttributes(~AssemblyAttributes.PA_FullMask, value & AssemblyAttributes.PA_FullMask);
        }

        public bool IsProcessorArchitectureNone => (Attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_None;

        public bool IsProcessorArchitectureMSIL => (Attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_MSIL;

        public bool IsProcessorArchitectureX86 => (Attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_x86;

        public bool IsProcessorArchitectureIA64 => (Attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_IA64;

        public bool IsProcessorArchitectureX64 => (Attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_AMD64;

        public bool IsProcessorArchitectureARM => (Attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_ARM;

        public bool IsProcessorArchitectureNoPlatform => (Attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_NoPlatform;

        public bool IsProcessorArchitectureSpecified
        {
            get => (Attributes & AssemblyAttributes.PA_Specified) != 0;
            set => ModifyAttributes(value, AssemblyAttributes.PA_Specified);
        }

        public bool EnableJITcompileTracking
        {
            get => (Attributes & AssemblyAttributes.EnableJITcompileTracking) != 0;
            set => ModifyAttributes(value, AssemblyAttributes.EnableJITcompileTracking);
        }

        public bool DisableJITcompileOptimizer
        {
            get => (Attributes & AssemblyAttributes.DisableJITcompileOptimizer) != 0;
            set => ModifyAttributes(value, AssemblyAttributes.DisableJITcompileOptimizer);
        }

        public bool IsRetargetable
        {
            get => (Attributes & AssemblyAttributes.Retargetable) != 0;
            set => ModifyAttributes(value, AssemblyAttributes.Retargetable);
        }

        public AssemblyAttributes ContentType
        {
            get => Attributes & AssemblyAttributes.ContentType_Mask;
            set => ModifyAttributes(~AssemblyAttributes.ContentType_Mask, value & AssemblyAttributes.ContentType_Mask);
        }

        public bool IsContentTypeDefault => (Attributes & AssemblyAttributes.ContentType_Mask) == AssemblyAttributes.ContentType_Default;

        public bool IsContentTypeWindowsRuntime => (Attributes & AssemblyAttributes.ContentType_Mask) == AssemblyAttributes.ContentType_WindowsRuntime;

    }
}