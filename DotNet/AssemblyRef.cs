using ILSpy.Interfaces;
using ILSpy.Enums;

namespace ILSpy.DotNet
{
    public abstract class AssemblyRef : IAssemblyRef
    {
        public static readonly AssemblyRef CurrentAssembly = new AssemblyRefUser("<CURRENT_ASSEMBLY>");

        protected Version version;
        protected int attributes;
        protected IPublicKey publicKeyOrToken;
        protected UTF8String name;
        protected UTF8String culture;
        protected byte[] hashValue;
        protected IList<IPdbCustomDebugInfo>? customDebugInfos;

        protected uint rid;

        public AssemblyRef(UTF8String? name, Version? version, IPublicKey? publicKey, UTF8String? locale, int? attributes)
        {
            this.version = version ?? new Version(0, 0, 0, 0);
            publicKeyOrToken = publicKey ?? new PublicKey();
            this.name = UTF8String.IsNullOrEmpty(name) ? UTF8String.Empty : name;
            culture = locale ?? UTF8String.Empty;
            this.attributes = attributes ?? (int)(publicKey is PublicKey ? AssemblyAttributes.PublicKey : AssemblyAttributes.None);
            hashValue = [];
        }

        public IMetadataToken Token
        {
            get => throw new NotImplementedException();

        }

        public uint Rid
        {
            get => rid;
            set => rid = value;
        }

        public int HasCustomAttributeTag => 15;

        public int ImplementationTag => 1;

        public int ResolutionScopeTag => 2;

        public ScopeType ScopeType => ScopeType.AssemblyRef;

        public string ScopeName => FullName;

        public Version Version
        {
            get => version;
            set => version = value;
        }

        public AssemblyAttributes Attributes
        {
            get => (AssemblyAttributes)attributes;
            set => attributes = (int)value;
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

        public byte[] Hash
        {
            get => hashValue;
            set => hashValue = value;
        }

        public ICustomAttributeCollection CustomAttributes
        {
            get
            {
                if (customAttributes is null)
                {
                    InitializeCustomAttributes();
                    ArgumentNullException.ThrowIfNull(customAttributes);
                }
                return customAttributes;
            }
        }
        protected ICustomAttributeCollection? customAttributes;

        protected virtual void InitializeCustomAttributes()
        {
            Interlocked.CompareExchange(ref customAttributes, new CustomAttributeCollection(), null);
        }

        public bool HasCustomAttributes => CustomAttributes.Count > 0;

        public int HasCustomDebugInformationTag => 15;

        public bool HasCustomDebugInfos => CustomDebugInfos.Count > 0;

        public IList<IPdbCustomDebugInfo> CustomDebugInfos
        {
            get
            {
                if (customDebugInfos is null)
                {
                    InitializeCustomDebugInfos();
                    ArgumentNullException.ThrowIfNull(customDebugInfos);
                }
                return customDebugInfos;
            }
        }

        protected virtual void InitializeCustomDebugInfos() =>
            Interlocked.CompareExchange(ref customDebugInfos, new List<IPdbCustomDebugInfo>(), null);

        public string FullName => FullNameToken;

        public string RealFullName => FullNameFactory.AssemblyFullName(this, false);

        public string FullNameToken => FullNameFactory.AssemblyFullName(this, true);

        void ModifyAttributes(AssemblyAttributes andMask, AssemblyAttributes orMask)
        {
            attributes = (attributes & (int)andMask) | (int)orMask;
        }

        void ModifyAttributes(bool set, AssemblyAttributes flags)
        {
            if (set)
                attributes |= (int)flags;
            else
                attributes &= ~(int)flags;
        }

        public bool HasPublicKey
        {
            get => ((AssemblyAttributes)attributes & AssemblyAttributes.PublicKey) != 0;
            set => ModifyAttributes(value, AssemblyAttributes.PublicKey);
        }


        public AssemblyAttributes ProcessorArchitecture
        {
            get => (AssemblyAttributes)attributes & AssemblyAttributes.PA_Mask;
            set => ModifyAttributes(~AssemblyAttributes.PA_Mask, value & AssemblyAttributes.PA_Mask);
        }

        public AssemblyAttributes ProcessorArchitectureFull
        {
            get => (AssemblyAttributes)attributes & AssemblyAttributes.PA_FullMask;
            set => ModifyAttributes(~AssemblyAttributes.PA_FullMask, value & AssemblyAttributes.PA_FullMask);
        }

        public bool IsProcessorArchitectureNone => ((AssemblyAttributes)attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_None;

        public bool IsProcessorArchitectureMSIL => ((AssemblyAttributes)attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_MSIL;

        public bool IsProcessorArchitectureX86 => ((AssemblyAttributes)attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_x86;

        public bool IsProcessorArchitectureIA64 => ((AssemblyAttributes)attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_IA64;

        public bool IsProcessorArchitectureX64 => ((AssemblyAttributes)attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_AMD64;

        public bool IsProcessorArchitectureARM => ((AssemblyAttributes)attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_ARM;

        public bool IsProcessorArchitectureNoPlatform => ((AssemblyAttributes)attributes & AssemblyAttributes.PA_Mask) == AssemblyAttributes.PA_NoPlatform;

        public bool IsProcessorArchitectureSpecified
        {
            get => ((AssemblyAttributes)attributes & AssemblyAttributes.PA_Specified) != 0;
            set => ModifyAttributes(value, AssemblyAttributes.PA_Specified);
        }

        public bool EnableJITcompileTracking
        {
            get => ((AssemblyAttributes)attributes & AssemblyAttributes.EnableJITcompileTracking) != 0;
            set => ModifyAttributes(value, AssemblyAttributes.EnableJITcompileTracking);
        }

        public bool DisableJITcompileOptimizer
        {
            get => ((AssemblyAttributes)attributes & AssemblyAttributes.DisableJITcompileOptimizer) != 0;
            set => ModifyAttributes(value, AssemblyAttributes.DisableJITcompileOptimizer);
        }

        public bool IsRetargetable
        {
            get => ((AssemblyAttributes)attributes & AssemblyAttributes.Retargetable) != 0;
            set => ModifyAttributes(value, AssemblyAttributes.Retargetable);
        }

        public AssemblyAttributes ContentType
        {
            get => (AssemblyAttributes)attributes & AssemblyAttributes.ContentType_Mask;
            set => ModifyAttributes(~AssemblyAttributes.ContentType_Mask, value & AssemblyAttributes.ContentType_Mask);
        }

        public bool IsContentTypeDefault => ((AssemblyAttributes)attributes & AssemblyAttributes.ContentType_Mask) == AssemblyAttributes.ContentType_Default;

        public bool IsContentTypeWindowsRuntime => ((AssemblyAttributes)attributes & AssemblyAttributes.ContentType_Mask) == AssemblyAttributes.ContentType_WindowsRuntime;

        public override string ToString() => FullName;
    }
}