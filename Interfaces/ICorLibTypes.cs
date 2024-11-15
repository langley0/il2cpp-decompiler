namespace ILSpy.Interfaces
{
    public interface ICorLibTypes {
		ICorLibTypeSig Void { get; }

		ICorLibTypeSig Boolean { get; }

		ICorLibTypeSig Char { get; }

		ICorLibTypeSig SByte { get; }

		ICorLibTypeSig Byte { get; }

		ICorLibTypeSig Int16 { get; }

		ICorLibTypeSig UInt16 { get; }

		ICorLibTypeSig Int32 { get; }

		ICorLibTypeSig UInt32 { get; }

		ICorLibTypeSig Int64 { get; }

		ICorLibTypeSig UInt64 { get; }

		ICorLibTypeSig Single { get; }

		ICorLibTypeSig Double { get; }

		ICorLibTypeSig String { get; }

		ICorLibTypeSig TypedReference { get; }

		ICorLibTypeSig IntPtr { get; }

		ICorLibTypeSig UIntPtr { get; }

		ICorLibTypeSig Object { get; }

		IAssemblyRef AssemblyRef { get; }

		ITypeRef GetTypeRef(string @namespace, string name);
	}
}