using System.Text;
using ILSpy.Interfaces;

namespace ILSpy.DotNet
{
    public struct FullNameFactory
    {
        public static string AssemblyFullName(IAssembly assembly, bool withToken, StringBuilder? sb = null)
        {
            return AssemblyFullNameSB(assembly, withToken, sb).ToString();
        }

        public static StringBuilder AssemblyFullNameSB(IAssembly assembly, bool withToken, StringBuilder? sb = null)
        {
            // var fnc = new FullNameFactory(false, null, sb);
            // fnc.CreateAssemblyFullName(assembly, withToken);
            // return fnc.sb ?? new StringBuilder();

            throw new NotImplementedException();
        }
    }
}