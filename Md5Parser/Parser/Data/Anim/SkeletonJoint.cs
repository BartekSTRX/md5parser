namespace Parser.Data.Anim
{
    public class SkeletonJoint
    {
        public SkeletonJoint(string name, int parent, int flags, int startIndex)
        {
            Name = name;
            Parent = parent;
            Flags = flags;
            StartIndex = startIndex;
        }

        public string Name { get; private set; }
        public int Parent { get; private set; }
        public int Flags { get; private set; }
        public int StartIndex { get; private set; }
    }
}