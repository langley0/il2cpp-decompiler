namespace ILSpy.Enums
{
    [Flags]
    public enum EventAttributes : ushort
    {
        SpecialName = 0x0200,
        RTSpecialName = 0x0400,
    }
}