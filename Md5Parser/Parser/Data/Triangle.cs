namespace Parser.Data
{
    public class Triangle
    {
        public Triangle(int index, int vertex1Index, int vertex2Index, int vertex3Index)
        {
            Index = index;
            Vertex1Index = vertex1Index;
            Vertex2Index = vertex2Index;
            Vertex3Index = vertex3Index;
        }

        public int Index { get; private set; }
        public int Vertex1Index { get; private set; }
        public int Vertex2Index { get; private set; }
        public int Vertex3Index { get; private set; }
    }
}