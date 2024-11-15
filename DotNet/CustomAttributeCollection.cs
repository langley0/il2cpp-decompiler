using ILSpy.Interfaces;

namespace ILSpy.DotNet
{
    public class CustomAttributeCollection : ICustomAttributeCollection
    {
        public int Count => throw new NotImplementedException();

        public ICustomAttribute this[int index] => throw new NotImplementedException();
    }
}