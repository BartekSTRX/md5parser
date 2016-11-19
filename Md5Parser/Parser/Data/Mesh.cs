using System.Collections.Generic;

namespace Parser.Data
{
    public class Mesh
    {
        public Mesh(string shader, IList<Vertex> vertices, IList<Triangle> triangles, IList<Weight> weights)
        {
            Shader = shader;
            Vertices = vertices;
            Triangles = triangles;
            Weights = weights;
        }

        public string Shader { get; private set; }
        public IList<Vertex> Vertices { get; private set; }
        public IList<Triangle> Triangles { get; private set; }
        public IList<Weight> Weights { get; private set; }
    }
}