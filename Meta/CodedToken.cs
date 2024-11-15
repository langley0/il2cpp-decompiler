namespace ILSpy.Meta
{
    public sealed class CodedToken
    {
        public static readonly CodedToken TypeDefOrRef = new(2, [
            TableType.TypeDef, TableType.TypeRef, TableType.TypeSpec,
        ]);


        public static readonly CodedToken HasConstant = new(2, [
            TableType.Field, TableType.Param, TableType.Property,
        ]);


        public static readonly CodedToken HasCustomAttribute = new(5, [
            TableType.Method, TableType.Field, TableType.TypeRef, TableType.TypeDef,
            TableType.Param, TableType.InterfaceImpl, TableType.MemberRef, TableType.Module,
            TableType.DeclSecurity, TableType.Property, TableType.Event, TableType.StandAloneSig,
            TableType.ModuleRef, TableType.TypeSpec, TableType.Assembly, TableType.AssemblyRef,
            TableType.File, TableType.ExportedType, TableType.ManifestResource, TableType.GenericParam,
            TableType.GenericParamConstraint, TableType.MethodSpec, 0, 0,
        ]);


        public static readonly CodedToken HasFieldMarshal = new(1, [
            TableType.Field, TableType.Param,
        ]);

        public static readonly CodedToken HasDeclSecurity = new(2, [
            TableType.TypeDef, TableType.Method, TableType.Assembly,
        ]);

        public static readonly CodedToken MemberRefParent = new(3, [
            TableType.TypeDef, TableType.TypeRef, TableType.ModuleRef, TableType.Method,
            TableType.TypeSpec,
        ]);

        public static readonly CodedToken HasSemantic = new(1, [
            TableType.Event, TableType.Property,
        ]);

        public static readonly CodedToken MethodDefOrRef = new(1, [
            TableType.Method, TableType.MemberRef,
        ]);

        public static readonly CodedToken MemberForwarded = new(1, [
            TableType.Field, TableType.Method,
        ]);

        public static readonly CodedToken Implementation = new(2, [
            TableType.File, TableType.AssemblyRef, TableType.ExportedType,
        ]);

        public static readonly CodedToken CustomAttributeType = new(3, [
            0, 0, TableType.Method, TableType.MemberRef, 0,
        ]);

        public static readonly CodedToken ResolutionScope = new(2, [
            TableType.Module, TableType.ModuleRef, TableType.AssemblyRef, TableType.TypeRef,
        ]);

        public static readonly CodedToken TypeOrMethodDef = new(1, [
            TableType.TypeDef, TableType.Method,
        ]);

        public static readonly CodedToken HasCustomDebugInformation = new(5, [
            TableType.Method, TableType.Field, TableType.TypeRef, TableType.TypeDef,
            TableType.Param, TableType.InterfaceImpl, TableType.MemberRef, TableType.Module,
            TableType.DeclSecurity, TableType.Property, TableType.Event, TableType.StandAloneSig,
            TableType.ModuleRef, TableType.TypeSpec, TableType.Assembly, TableType.AssemblyRef,
            TableType.File, TableType.ExportedType, TableType.ManifestResource, TableType.GenericParam,
            TableType.GenericParamConstraint, TableType.MethodSpec, TableType.Document, TableType.LocalScope,
            TableType.LocalVariable, TableType.LocalConstant, TableType.ImportScope,
        ]);

        readonly TableType[] tableTypes;
        readonly int bits;
        readonly int mask;

        public TableType[] TableTypes => tableTypes;

        public int Bits => bits;

        internal CodedToken(int bits, TableType[] tableTypes)
        {
            this.bits = bits;
            mask = (1 << bits) - 1;
            this.tableTypes = tableTypes;
        }

        public uint Encode(MetadataToken token) => Encode(token.Raw);

        public uint Encode(uint token)
        {
            Encode(token, out uint codedToken);
            return codedToken;
        }

        public bool Encode(MetadataToken token, out uint codedToken) => Encode(token.Raw, out codedToken);

        public bool Encode(uint token, out uint codedToken)
        {
            int index = Array.IndexOf(tableTypes, MetadataToken.ToTableType(token));
            if (index < 0)
            {
                codedToken = uint.MaxValue;
                return false;
            }
            // This shift can never overflow a uint since bits < 8 (it's at most 5), and
            // ToRid() returns an integer <= 0x00FFFFFF.
            codedToken = (MetadataToken.ToRID(token) << bits) | (uint)index;
            return true;
        }

        public MetadataToken Decode2(uint codedToken)
        {
            Decode(codedToken, out uint token);
            return new MetadataToken(token);
        }

        public uint Decode(uint codedToken)
        {
            Decode(codedToken, out uint token);
            return token;
        }

        public bool Decode(uint codedToken, out MetadataToken token)
        {
            bool result = Decode(codedToken, out uint decodedToken);
            token = new MetadataToken(decodedToken);
            return result;
        }

        public bool Decode(uint codedToken, out uint token)
        {
            uint rid = codedToken >> bits;
            int index = (int)(codedToken & mask);
            if (rid > MetadataToken.RID_MAX || index >= tableTypes.Length)
            {
                token = 0;
                return false;
            }

            token = ((uint)tableTypes[index] << MetadataToken.TABLE_SHIFT) | rid;
            return true;
        }
    }
}