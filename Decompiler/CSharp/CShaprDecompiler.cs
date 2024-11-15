namespace ILSpy.Decompiler.CSharp
{
    public sealed class CSharpDecompiler : DecompilerBase
    {
        public override string UniqueNameUI => "C#";
        public override string GenericNameUI => "C#";
        public override Guid UniqueGuid => Constants.LANGUAGE_CSHARP;
        public override Guid GenericGuid => Constants.LANGUAGE_CSHARP;
    }
}