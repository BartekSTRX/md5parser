namespace Parser.Data
{
    public class Vertex
    {
        public Vertex(int index, double texU, double texV, int weightIndex, int weightElem)
        {
            Index = index;
            TexU = texU;
            TexV = texV;
            WeightIndex = weightIndex;
            WeightElem = weightElem;
        }

        public int Index { get; private set; }
        public double TexU { get; private set; }
        public double TexV { get; private set; }
        public int WeightIndex { get; private set; }
        public int WeightElem { get; private set; }
    }
}