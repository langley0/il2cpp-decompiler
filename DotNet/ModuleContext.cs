using ILSpy.Interfaces;


namespace ILSpy.DotNet
{
	public sealed class ModuleContext
	{
		IAssemblyResolver? assemblyResolver;
		IResolver? resolver;
		readonly OpCode?[][] experimentalOpCodes = new OpCode?[12][];

		public IAssemblyResolver AssemblyResolver
		{
			get
			{
				if (assemblyResolver is null)
				{
					Interlocked.CompareExchange(ref assemblyResolver, NullResolver.Instance, null);
					ArgumentNullException.ThrowIfNull(assemblyResolver);
				}
				return assemblyResolver;
			}
			set => assemblyResolver = value;
		}

		public IResolver Resolver
		{
			get
			{
				if (resolver is null)
				{
					Interlocked.CompareExchange(ref resolver, NullResolver.Instance, null);
					ArgumentNullException.ThrowIfNull(resolver);
				}
				return resolver;
			}
			set => resolver = value;
		}

		public ModuleContext()
		{
		}

		public ModuleContext(IAssemblyResolver? assemblyResolver)
			: this(assemblyResolver, new Resolver(assemblyResolver))
		{
		}

		public ModuleContext(IResolver? resolver)
			: this(null, resolver)
		{
		}

		public ModuleContext(IAssemblyResolver? assemblyResolver, IResolver? resolver)
		{
			this.assemblyResolver = assemblyResolver;
			this.resolver = resolver;
			if (resolver is null && assemblyResolver is not null)
			{
				this.resolver = new Resolver(assemblyResolver);
			}
		}

		public void RegisterExperimentalOpCode(OpCode opCode)
		{
			byte high = (byte)((ushort)opCode.Value >> 8);
			byte low = (byte)opCode.Value;
			OpCode?[] array = experimentalOpCodes[high - 0xF0] ??= new OpCode[256];

			array[low] = opCode;
		}

		public void ClearExperimentalOpCode(byte high, byte low)
		{
			OpCode?[] array = experimentalOpCodes[high - 0xF0];

			if (array != null)
			{
				array[low] = null;
			}
		}

		public OpCode? GetExperimentalOpCode(byte high, byte low)
		{
			return experimentalOpCodes[high - 0xF0][low];
		}
	}
}