namespace ILSpy.Decompiler
{
    public abstract class DecompilerBase : IDecompiler
    {
        public abstract string UniqueNameUI { get; }
        public abstract string GenericNameUI { get; }
        public abstract Guid UniqueGuid { get; }
        public abstract Guid GenericGuid { get; }


        // /// <summary>
		// /// Decompiles a method
		// /// </summary>
		// /// <param name="method">Method</param>
		// /// <param name="output">Output</param>
		// /// <param name="ctx">Context</param>
		// void Decompile(MethodDef method, IDecompilerOutput output, DecompilationContext ctx);

		// /// <summary>
		// /// Decompiles a property
		// /// </summary>
		// /// <param name="property">Property</param>
		// /// <param name="output">Output</param>
		// /// <param name="ctx">Context</param>
		// void Decompile(PropertyDef property, IDecompilerOutput output, DecompilationContext ctx);

		// /// <summary>
		// /// Decompiles a field
		// /// </summary>
		// /// <param name="field">Field</param>
		// /// <param name="output">Output</param>
		// /// <param name="ctx">Context</param>
		// void Decompile(FieldDef field, IDecompilerOutput output, DecompilationContext ctx);

		// /// <summary>
		// /// Decompiles an event
		// /// </summary>
		// /// <param name="ev">Event</param>
		// /// <param name="output">Output</param>
		// /// <param name="ctx">Context</param>
		// void Decompile(EventDef ev, IDecompilerOutput output, DecompilationContext ctx);

		// /// <summary>
		// /// Decompiles a type
		// /// </summary>
		// /// <param name="type">Type</param>
		// /// <param name="output">Output</param>
		// /// <param name="ctx">Context</param>
		// void Decompile(TypeDef type, IDecompilerOutput output, DecompilationContext ctx);
    }
}