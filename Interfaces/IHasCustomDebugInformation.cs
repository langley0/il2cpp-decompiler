namespace ILSpy.Interfaces
{
    public interface IHasCustomDebugInformation
    {
        int HasCustomDebugInformationTag { get; }

        IList<IPdbCustomDebugInfo> CustomDebugInfos { get; }

        bool HasCustomDebugInfos { get; }
    }
}