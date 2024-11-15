namespace ILSpy
{
    public interface IListListener<TListValue>
    {
        void OnLazyAdd(int index, ref TListValue value);

        void OnAdd(int index, TListValue value);

        void OnRemove(int index, TListValue value);

        void OnResize(int index);

        void OnClear();
    }
}