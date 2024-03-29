﻿namespace Parser.Data.Mesh
{
    public class Vertex
    {
        public Vertex(int index, float texU, float texV, int weightIndex, int weightElem)
        {
            Index = index;
            TexU = texU;
            TexV = texV;
            WeightIndex = weightIndex;
            WeightElem = weightElem;
        }

        public int Index { get; private set; }
        public float TexU { get; private set; }
        public float TexV { get; private set; }
        public int WeightIndex { get; private set; }
        public int WeightElem { get; private set; }
    }
}