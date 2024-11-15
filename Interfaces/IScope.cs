namespace ILSpy.Interfaces
{
    public enum ScopeType
    {
        AssemblyRef,
        ModuleRef,
        ModuleDef,
    }

    public interface IScope
    {
        /// <summary>
        /// Gets the scope type
        /// </summary>
        ScopeType ScopeType { get; }

        /// <summary>
        /// Gets the scope name
        /// </summary>
        string ScopeName { get; }
    }
}