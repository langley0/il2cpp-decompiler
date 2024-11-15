using ILSpy.Interfaces;
using ILSpy.Enums;
using System.Reflection;

namespace ILSpy.DotNet
{
    public class AssemblyRefUser : AssemblyRef
    {
        public static AssemblyRefUser CreateMscorlibReferenceCLR10() => new("mscorlib", new Version(1, 0, 3300, 0), new PublicKeyToken("b77a5c561934e089"));

        public static AssemblyRefUser CreateMscorlibReferenceCLR11() => new("mscorlib", new Version(1, 0, 5000, 0), new PublicKeyToken("b77a5c561934e089"));

        public static AssemblyRefUser CreateMscorlibReferenceCLR20() => new("mscorlib", new Version(2, 0, 0, 0), new PublicKeyToken("b77a5c561934e089"));

        public static AssemblyRefUser CreateMscorlibReferenceCLR40() => new("mscorlib", new Version(4, 0, 0, 0), new PublicKeyToken("b77a5c561934e089"));

        public AssemblyRefUser()
            : this(UTF8String.Empty)
        {
        }

        public AssemblyRefUser(UTF8String name)
            : this(name, new Version(0, 0, 0, 0))
        {
        }

        public AssemblyRefUser(UTF8String? name, Version? version)
            : this(name, version, new PublicKey())
        {
        }

        public AssemblyRefUser(UTF8String? name, Version? version, IPublicKey? publicKey)
            : this(name, version, publicKey, UTF8String.Empty)
        {
        }

        public AssemblyRefUser(UTF8String? name, Version? version, IPublicKey? publicKey, UTF8String? locale)
        : base(name, version, publicKey, locale, (int)(publicKey is PublicKey ? AssemblyAttributes.PublicKey : AssemblyAttributes.None))
        {
        }

        public AssemblyRefUser(AssemblyName asmName)
            : this(new AssemblyNameInfo(asmName)) => attributes = (int)asmName.Flags;

        public AssemblyRefUser(IAssembly assembly)
            : base(
                assembly.Name,
                assembly.Version,
                assembly.PublicKeyOrToken,
                assembly.Culture,
                (int)assembly.ContentType)
        {
        }
    }
}