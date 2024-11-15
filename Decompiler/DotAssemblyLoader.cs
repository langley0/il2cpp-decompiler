using System.Reflection;
using System.Runtime.Loader;

namespace ILSpy.Decompiler
{
	sealed class DotAssemblyLoader
	{
		string[] searchPaths;
		readonly HashSet<string> searchPathsHash;
		static readonly string[] assemblyExtensions = new string[] { ".dll" };

		public DotAssemblyLoader(AssemblyLoadContext loadContext)
		{
			searchPaths = Array.Empty<string>();
			searchPathsHash = new HashSet<string>(StringComparer.Ordinal);
			loadContext.Resolving += Resolving;
		}

		public void AddSearchPath(string path)
		{
			if (!Directory.Exists(path))
			{
				return;
			}
			if (!searchPathsHash.Add(path))
			{
				return;
			}
			var searchPaths = new string[this.searchPaths.Length + 1];
			Array.Copy(this.searchPaths, 0, searchPaths, 0, this.searchPaths.Length);
			searchPaths[searchPaths.Length - 1] = path;
			this.searchPaths = searchPaths;
		}

		Assembly? Resolving(AssemblyLoadContext context, AssemblyName name)
		{
			foreach (var path in searchPaths)
			{
				foreach (var asmExt in assemblyExtensions)
				{
					var filename = Path.Combine(path, name.Name + asmExt);
					if (File.Exists(filename))
						return context.LoadFromAssemblyPath(filename);
				}
			}
			return null;
		}
	}
}