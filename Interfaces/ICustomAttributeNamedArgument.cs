namespace ILSpy.Interfaces
{
    public interface ICustomAttributeNamedArgument
    {
        bool IsField
        {
            get;
            set;
        }

        bool IsProperty
        {
            get;
            set;
        }

        public UTF8String Name
        {
            get;
            set;
        }
    }
}