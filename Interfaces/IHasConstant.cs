namespace ILSpy.Interfaces
{
    public interface IHasConstant : ICodedToken, IHasCustomAttribute, IFullName
    {
        int HasConstantTag { get; }

        IConstant Constant { get; set; }
    }

}