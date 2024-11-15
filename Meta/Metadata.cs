using ILSpy.PE;

namespace ILSpy.Meta
{
	public abstract class Metadata : IDisposable
	{
		public abstract bool IsCompressed { get; }

		public abstract bool IsStandalonePortablePdb { get; }

		public abstract IPEImage? PEImage { get; }

		public abstract ImageCor20Header? ImageCor20Header { get; }

		public abstract uint Version { get; }

		public abstract string VersionString { get; }

		public abstract MetadataHeader? MetadataHeader { get; }

		public abstract StringsStream? StringsStream { get; }

		public abstract USStream? USStream { get; }

		public abstract BlobStream? BlobStream { get; }

		public abstract GuidStream? GuidStream { get; }

		public abstract TablesStream? TablesStream { get; }

		public abstract PdbStream? PdbStream { get; }

		public abstract IList<DotNetStream>? AllStreams { get; }

		public abstract RidList GetTypeDefRidList();

		public abstract RidList GetExportedTypeRidList();

		public abstract RidList GetFieldRidList(uint typeDefRid);

		public abstract RidList GetMethodRidList(uint typeDefRid);

		public abstract RidList GetParamRidList(uint methodRid);

		public abstract RidList GetEventRidList(uint eventMapRid);

		public abstract RidList GetPropertyRidList(uint propertyMapRid);

		public abstract RidList GetInterfaceImplRidList(uint typeDefRid);

		public abstract RidList GetGenericParamRidList(TableType table, uint rid);

		public abstract RidList GetGenericParamConstraintRidList(uint genericParamRid);

		public abstract RidList GetCustomAttributeRidList(TableType table, uint rid);

		public abstract RidList GetDeclSecurityRidList(TableType table, uint rid);

		public abstract RidList GetMethodSemanticsRidList(TableType table, uint rid);

		public abstract RidList GetMethodImplRidList(uint typeDefRid);

		public abstract uint GetClassLayoutRid(uint typeDefRid);

		public abstract uint GetFieldLayoutRid(uint fieldRid);

		public abstract uint GetFieldMarshalRid(TableType table, uint rid);

		public abstract uint GetFieldRVARid(uint fieldRid);

		public abstract uint GetImplMapRid(TableType table, uint rid);

		public abstract uint GetNestedClassRid(uint typeDefRid);

		public abstract uint GetEventMapRid(uint typeDefRid);

		public abstract uint GetPropertyMapRid(uint typeDefRid);

		public abstract uint GetConstantRid(TableType table, uint rid);

		public abstract uint GetOwnerTypeOfField(uint fieldRid);

		public abstract uint GetOwnerTypeOfMethod(uint methodRid);

		public abstract uint GetOwnerTypeOfEvent(uint eventRid);

		public abstract uint GetOwnerTypeOfProperty(uint propertyRid);

		public abstract uint GetOwnerOfGenericParam(uint gpRid);

		public abstract uint GetOwnerOfGenericParamConstraint(uint gpcRid);

		public abstract uint GetOwnerOfParam(uint paramRid);

		public abstract RidList GetNestedClassRidList(uint typeDefRid);

		public abstract RidList GetNonNestedClassRidList();

		public abstract RidList GetLocalScopeRidList(uint methodRid);

		public abstract RidList GetLocalVariableRidList(uint localScopeRid);

		public abstract RidList GetLocalConstantRidList(uint localScopeRid);

		public abstract uint GetStateMachineMethodRid(uint methodRid);

		public abstract RidList GetCustomDebugInformationRidList(TableType table, uint rid);

		public abstract void Dispose();
	}
}
