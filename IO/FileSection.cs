namespace ILSpy.IO
{
    public class FileSection : IFileSection {
        public FileOffset StartOffset { get; private set;}
        public FileOffset EndOffset { get; private set; }

        protected void SetStartOffset(uint currentOffset) {
            StartOffset = (FileOffset)currentOffset;
        }

        protected void SetEndOffset(uint currentOffset) {
            EndOffset = (FileOffset)currentOffset;
        }
    }
}