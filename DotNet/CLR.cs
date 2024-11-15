using ILSpy.Interfaces;
using ILSpy.Enums;

namespace ILSpy.DotNet
{
    public static class CLR
    {
        readonly struct ClassName : IEquatable<ClassName>
        {
            public readonly UTF8String Namespace;
            public readonly UTF8String Name;
            // Not used when comparing for equality etc
            public readonly bool IsValueType;

            public ClassName(UTF8String ns, UTF8String name, bool isValueType = false)
            {
                Namespace = ns;
                Name = name;
                IsValueType = isValueType;
            }

            public ClassName(string ns, string name, bool isValueType = false)
            {
                Namespace = ns;
                Name = name;
                IsValueType = isValueType;
            }

            public static bool operator ==(ClassName a, ClassName b) => a.Equals(b);
            public static bool operator !=(ClassName a, ClassName b) => !a.Equals(b);

            public bool Equals(ClassName other) =>
                // Don't check IsValueType
                UTF8String.Equals(Namespace, other.Namespace) && UTF8String.Equals(Name, other.Name);

            public override bool Equals(object? obj)
            {
                if (obj is ClassName classObj)
                {
                    return Equals(classObj);
                }
                return false;
            }

            public override int GetHashCode() =>
                // Don't use IsValueType
                UTF8String.GetHashCode(Namespace) ^ UTF8String.GetHashCode(Name);

            public override string ToString() => $"{Namespace}.{Name}";
        }

        sealed class ProjectedClass
        {
            public readonly ClassName WinMDClass;
            public readonly ClassName ClrClass;
            public readonly ClrAssembly ClrAssembly;
            public readonly ClrAssembly ContractAssembly;

            public ProjectedClass(string mdns, string mdname, string clrns, string clrname, ClrAssembly clrAsm, ClrAssembly contractAsm, bool winMDValueType, bool clrValueType)
            {
                WinMDClass = new ClassName(mdns, mdname, winMDValueType);
                ClrClass = new ClassName(clrns, clrname, clrValueType);
                ClrAssembly = clrAsm;
                ContractAssembly = contractAsm;
            }

            public override string ToString() => $"{WinMDClass} <-> {ClrClass}, {CreateAssembly(null, ContractAssembly)}";
        }

        static readonly Dictionary<ClassName, ProjectedClass> classMapper = [];

        static IAssemblyRef CreateAssembly(IModuleDef? module, ClrAssembly clrAsm)
        {
            // var mscorlib = module?.CorLibTypes.AssemblyRef;
            // var asm = new AssemblyRefUser(GetName(clrAsm), contractAsmVersion, new PublicKeyToken(GetPublicKeyToken(clrAsm)), UTF8String.Empty);

            // if (mscorlib is not null && mscorlib.Name == mscorlibName && IsValidMscorlibVersion(mscorlib.Version))
            // {
            //     asm.Version = mscorlib.Version;
            // }
            // if (module is ModuleDefMD mod)
            // {
            //     Version ver = null;
            //     foreach (var asmRef in mod.GetAssemblyRefs())
            //     {
            //         if (asmRef.IsContentTypeWindowsRuntime)
            //             continue;
            //         if (asmRef.Name != asm.Name)
            //             continue;
            //         if (asmRef.Culture != asm.Culture)
            //             continue;
            //         if (!PublicKeyBase.TokenEquals(asmRef.PublicKeyOrToken, asm.PublicKeyOrToken))
            //             continue;
            //         if (!IsValidMscorlibVersion(asmRef.Version))
            //             continue;

            //         if (ver is null || asmRef.Version > ver)
            //             ver = asmRef.Version;
            //     }
            //     if (ver is not null)
            //         asm.Version = ver;
            // }

            // return asm;
            throw new System.NotImplementedException();
        }

        static IAssemblyRef? ToCLR(IModuleDef module, ref UTF8String ns, ref UTF8String name)
        {
            if (!classMapper.TryGetValue(new ClassName(ns, name), out var pc))
            {
                return null;
            }

            ns = pc.ClrClass.Namespace;
            name = pc.ClrClass.Name;
            return CreateAssembly(module, pc.ContractAssembly);
        }

        public static IMemberRef ToCLR(IModuleDef?module, IMemberRef? mr) {
			// if (mr is null)
			// 	return null;
			// if (mr.Name != CloseName)
			// 	return null;

			// var msig = mr.MethodSig;
			// if (msig is null)
			// 	return null;

			// var cl = mr.Class;
			// IMemberRefParent newCl;
			// TypeSpec ts;
			// if (cl is TypeRef tr) {
			// 	var newTr = ToCLR(module, tr);
			// 	if (newTr is null || !IsIDisposable(newTr))
			// 		return null;

			// 	newCl = newTr;
			// }
			// else if ((ts = cl as TypeSpec) is not null) {
			// 	var gis = ts.TypeSig as GenericInstSig;
			// 	if (gis is null || !(gis.GenericType is ClassSig))
			// 		return null;
			// 	tr = gis.GenericType.TypeRef;
			// 	if (tr is null)
			// 		return null;

			// 	var newTr = ToCLR(module, tr, out bool isClrValueType);
			// 	if (newTr is null || !IsIDisposable(newTr))
			// 		return null;

			// 	newCl = new TypeSpecUser(new GenericInstSig(isClrValueType ?
			// 					(ClassOrValueTypeSig)new ValueTypeSig(newTr) :
			// 					new ClassSig(newTr), gis.GenericArguments));
			// }
			// else
			// 	return null;

			// return new MemberRefUser(mr.Module, DisposeName, msig, newCl);
            throw new System.NotImplementedException();
		}
    }
}