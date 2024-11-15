using ILSpy.Decompiler;
using ILSpy.Meta;

namespace ILSpy
{
	class Program
	{
		static void Main(string[] args)
		{
			Decompile();
		}

		static void Decompile()
		{
			var meta = MetadataFactory.Load("C:\\Users\\langley\\Downloads\\Assembly-CSharp.dll", Enums.CLRRuntimeReaderKind.Mono);
			//var peImage = new PE.PEImage("C:\\Users\\langley\\Downloads\\Assembly-CSharp.dll");
		}
	}

	// class MSDecompiler {
	//     string language = Constants.LANGUAGE_CSHARP.ToString();

	//     public static void Decompile() {
	//         // Decompiler.Decompile();
	//     }

	//     IDecompiler GetLanguage() => GetLanguageOrNull() ?? throw new InvalidOperationException();
	// 	IDecompiler? GetLanguageOrNull() {
	// 		bool hasGuid = Guid.TryParse(language, out var guid);
	// 		return AllLanguages.FirstOrDefault(a => {
	// 			if (StringComparer.OrdinalIgnoreCase.Equals(language, a.UniqueNameUI))
	// 				return true;
	// 			if (hasGuid && (guid.Equals(a.UniqueGuid) || guid.Equals(a.GenericGuid)))
	// 				return true;
	// 			return false;
	// 		});
	// 	}

	//     static IEnumerable<IDecompiler> GetAllLanguages() {
	// 		var asmNames = new string[] {
	// 			"dnSpy.Decompiler.ILSpy.Core",
	// 		};
	// 		foreach (var asmName in asmNames) {
	// 			foreach (var l in GetLanguagesInAssembly(asmName))
	// 				yield return l;
	// 		}
	// 	}
	// }



}