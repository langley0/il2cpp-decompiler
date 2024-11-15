namespace ILSpy.Pdb
{
    public sealed class SequencePoint
    {
        required public PdbDocument Document { get; set; }

        public int StartLine { get; set; }

        public int StartColumn { get; set; }

        public int EndLine { get; set; }

        public int EndColumn { get; set; }

        public SequencePoint Clone()
        {
            return new SequencePoint()
            {
                Document = Document,
                StartLine = StartLine,
                StartColumn = StartColumn,
                EndLine = EndLine,
                EndColumn = EndColumn,
            };
        }
    }
}