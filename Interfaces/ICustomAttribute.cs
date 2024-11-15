namespace ILSpy.Interfaces
{
    public interface ICustomAttribute {
        ITypeDefOrRef? AttributeType { get; }
        string TypeFullName { get; }
        bool HasNamedArguments { get; }
        IList<ICustomAttributeNamedArgument> NamedArguments { get; }
        IEnumerable<ICustomAttributeNamedArgument> Fields { get; }
        IEnumerable<ICustomAttributeNamedArgument> Properties { get; }
    }
}