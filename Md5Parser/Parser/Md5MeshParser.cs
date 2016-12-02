using System.Collections.Generic;
using Parser.Data.Mesh;

namespace Parser
{
    public class Md5MeshParser : Md5Parser
    {
        public Md5MeshFile Parse(IEnumerable<Token> tokens)
        {
            TokenQueue = new Queue<Token>(tokens);

            ParseKeyword("MD5Version");
            var version = ParseInt("version");

            ParseKeyword("commandline");
            var commandLineParameters = ParseQuotedString("command line parameters");


            ParseKeyword("numJoints");
            var numJoints = ParseInt("number of joints");

            ParseKeyword("numMeshes");
            var numMeshes = ParseInt("number of meshes");


            var joints = ParseJoints();
            AssertCorrectListSize("joints", joints, numJoints);

            var meshes = ParseMeshes();
            AssertCorrectListSize("meshes", meshes, numMeshes);

            return new Md5MeshFile(version, commandLineParameters, joints, meshes);
        }

        private IList<Mesh> ParseMeshes()
        {
            var meshes = new List<Mesh>();

            while (TokenQueue.Peek().Type == TokenType.Word && TokenQueue.Peek().Value == "mesh")
            {
                ParseKeyword("mesh");
                ParseLeftBrace();

                ParseKeyword("shader");
                var shader = ParseQuotedString("shader");


                ParseKeyword("numverts");
                var numberOfVertices = ParseInt("number of vertices");

                var vertices = new List<Vertex>();
                while (TokenQueue.Peek().Type == TokenType.Word && TokenQueue.Peek().Value == "vert")
                {
                    ParseKeyword("vert");
                    var index = ParseInt("index");

                    ParseLeftParenthesis();
                    var texU = ParseFloat("tex U");
                    var texV = ParseFloat("tex V");
                    ParseRightParenthesis();

                    var weightIndex = ParseInt("weight index");
                    var weightElem = ParseInt("weight elements");

                    vertices.Add(new Vertex(index, texU, texV, weightIndex, weightElem));
                }

                AssertCorrectListSize("vertices", vertices, numberOfVertices);


                ParseKeyword("numtris");
                var numberOfTriangles = ParseInt("number of triangles");

                var triangles = new List<Triangle>();
                while (TokenQueue.Peek().Type == TokenType.Word && TokenQueue.Peek().Value == "tri")
                {
                    ParseKeyword("tri");

                    var index = ParseInt("index");

                    var vertex1 = ParseInt("vertex 1 index");
                    var vertex2 = ParseInt("vertex 2 index");
                    var vertex3 = ParseInt("vertex 3 index");

                    triangles.Add(new Triangle(index, vertex1, vertex2, vertex3));
                }

                AssertCorrectListSize("triangles", triangles, numberOfTriangles);


                ParseKeyword("numweights");
                var numberOfWeights = ParseInt("number of weights");

                var weights = new List<Weight>();
                while (TokenQueue.Peek().Type == TokenType.Word && TokenQueue.Peek().Value == "weight")
                {
                    ParseKeyword("weight");
                    var index = ParseInt("index");
                    var jointIndex = ParseInt("joint index");
                    var weightValue = ParseFloat("weight value");

                    ParseLeftParenthesis();
                    var posX = ParseFloat("position X");
                    var posY = ParseFloat("position Y");
                    var posZ = ParseFloat("position Z");
                    ParseRightParenthesis();

                    weights.Add(new Weight(index, jointIndex, weightValue, posX, posY, posZ));
                }

                AssertCorrectListSize("weights", weights, numberOfWeights);


                meshes.Add(new Mesh(shader, vertices, triangles, weights));

                ParseRightBrace();
            }

            return meshes;
        }

        private IList<Joint> ParseJoints()
        {
            return ParseList("joints", () =>
            {
                var boneName = ParseQuotedString("bone name");
                var parentIndex = ParseInt("parent index");

                ParseLeftParenthesis();
                var posX = ParseFloat("position x");
                var posY = ParseFloat("position y");
                var posZ = ParseFloat("position z");
                ParseRightParenthesis();

                ParseLeftParenthesis();
                var orientX = ParseFloat("orientation x");
                var orientY = ParseFloat("orientation y");
                var orientZ = ParseFloat("orientation z");
                ParseRightParenthesis();

                return new Joint(boneName, parentIndex, posX, posY, posZ, orientX, orientY, orientZ);
            });
        }
    }
}