namespace ILSpy.IO
{
    public interface IFileSection
    {
        FileOffset StartOffset { get; }
        FileOffset EndOffset { get; }
    }
}
