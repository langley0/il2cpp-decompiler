namespace ILSpy.Interfaces
{
    public interface IHasFieldMarshal : ICodedToken, IHasCustomAttribute, IHasConstant, IFullName
    {
        int HasFieldMarshalTag { get; }

        IMarshalType MarshalType { get; set; }

        bool HasMarshalType { get; }
    }
}