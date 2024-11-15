#pragma warning disable IDE0066 // Convert switch statement to expression

namespace ILSpy.Enums
{
    public enum ElementType : byte
    {
        End = 0x00,
        Void = 0x01,
        Boolean = 0x02,
        Char = 0x03,
        I1 = 0x04,
        U1 = 0x05,
        I2 = 0x06,
        U2 = 0x07,
        I4 = 0x08,
        U4 = 0x09,
        I8 = 0x0A,
        U8 = 0x0B,
        R4 = 0x0C,
        R8 = 0x0D,
        String = 0x0E,
        Ptr = 0x0F,
        ByRef = 0x10,
        ValueType = 0x11,
        Class = 0x12,
        Var = 0x13,
        Array = 0x14,
        GenericInst = 0x15,
        TypedByRef = 0x16,
        ValueArray = 0x17,
        I = 0x18,
        U = 0x19,
        R = 0x1A,
        FnPtr = 0x1B,
        Object = 0x1C,
        SZArray = 0x1D,
        MVar = 0x1E,
        CModReqd = 0x1F,
        CModOpt = 0x20,
        Internal = 0x21,
        Module = 0x3F,
        Sentinel = 0x41,
        Pinned = 0x45,
    }

    
	public static partial class Extensions {
		public static bool IsPrimitive(this ElementType etype) {
			switch (etype) {
			case ElementType.Boolean:
			case ElementType.Char:
			case ElementType.I1:
			case ElementType.U1:
			case ElementType.I2:
			case ElementType.U2:
			case ElementType.I4:
			case ElementType.U4:
			case ElementType.I8:
			case ElementType.U8:
			case ElementType.R4:
			case ElementType.R8:
			case ElementType.I:
			case ElementType.U:
			case ElementType.R:
				return true;

			default:
				return false;
			}
		}

		public static int GetPrimitiveSize(this ElementType etype, int ptrSize = -1) {
			switch (etype) {
			case ElementType.Boolean:
			case ElementType.I1:
			case ElementType.U1:
				return 1;

			case ElementType.Char:
			case ElementType.I2:
			case ElementType.U2:
				return 2;

			case ElementType.I4:
			case ElementType.U4:
			case ElementType.R4:
				return 4;

			case ElementType.I8:
			case ElementType.U8:
			case ElementType.R8:
				return 8;

			case ElementType.Ptr:
			case ElementType.FnPtr:
			case ElementType.I:
			case ElementType.U:
				return ptrSize;

			default:
				return -1;
			}
		}

		public static bool IsValueType(this ElementType etype) {
            switch (etype) {
			case ElementType.Void:
			case ElementType.Boolean:
			case ElementType.Char:
			case ElementType.I1:
			case ElementType.U1:
			case ElementType.I2:
			case ElementType.U2:
			case ElementType.I4:
			case ElementType.U4:
			case ElementType.I8:
			case ElementType.U8:
			case ElementType.R4:
			case ElementType.R8:
			case ElementType.ValueType:
			case ElementType.TypedByRef:
			case ElementType.ValueArray:
			case ElementType.I:
			case ElementType.U:
			case ElementType.R:
				return true;

			case ElementType.GenericInst:
				// We don't have enough info to determine whether this is a value type
				return false;

			case ElementType.End:
			case ElementType.String:
			case ElementType.Ptr:
			case ElementType.ByRef:
			case ElementType.Class:
			case ElementType.Var:
			case ElementType.Array:
			case ElementType.FnPtr:
			case ElementType.Object:
			case ElementType.SZArray:
			case ElementType.MVar:
			case ElementType.CModReqd:
			case ElementType.CModOpt:
			case ElementType.Internal:
			case ElementType.Module:
			case ElementType.Sentinel:
			case ElementType.Pinned:
			default:
				return false;
			}
        }
	}
}