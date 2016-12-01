using System;
using System.Collections.Generic;
using System.Globalization;
using Parser.Data.Anim;
using Parser.Data.Mesh;

namespace Parser
{
    public abstract class Md5Parser
    {
        protected Queue<Token> TokenQueue;

        private void ParseSimpleToken(TokenType expectedTokenType, string errorMessage)
        {
            if (TokenQueue.Dequeue().Type != expectedTokenType)
            {
                throw new Exception(errorMessage);
            }
        }

        protected void ParseLeftParenthesis()
        {
            ParseSimpleToken(TokenType.LParen, "left parenthesis '(' expected");
        }

        protected void ParseRightParenthesis()
        {
            ParseSimpleToken(TokenType.RParen, "right parenthesis ')' expected");
        }

        protected void ParseLeftBrace()
        {
            ParseSimpleToken(TokenType.LBrace, "left brace '{' expected");
        }

        protected void ParseRightBrace()
        {
            ParseSimpleToken(TokenType.RBrace, "right brace '}' expected");
        }

        protected int ParseInt(string name)
        {
            var token = TokenQueue.Dequeue();
            if (token.Type != TokenType.Integer && token.Type != TokenType.Float)
            {
                throw new Exception(name + " expected (integer number)");
            }
            return int.Parse(token.Value);
        }

        protected double ParseFloat(string name)
        {
            var token = TokenQueue.Dequeue();
            if (token.Type != TokenType.Float && token.Type != TokenType.Integer)
            {
                throw new Exception(name + " expected (floating point number)");
            }
            return double.Parse(token.Value, CultureInfo.InvariantCulture);
        }

        protected string ParseQuotedString(string name)
        {
            var token = TokenQueue.Dequeue();
            if (token.Type != TokenType.QuotedString)
            {
                throw new Exception(name + " expected (quoted string)");
            }
            return token.Value;
        }

        protected void ParseKeyword(string name)
        {
            var token = TokenQueue.Dequeue();
            if (!(token.Type == TokenType.Word && token.Value == name))
            {
                throw new Exception(name + " keyword expected");
            }
        }

        protected void AssertCorrectListSize<T>(string name, IList<T> list, int expectedSize)
        {
            if (list.Count != expectedSize)
            {
                throw new Exception(string.Format(
                    "number of parsed {0} ({1}) is different than expected ({2})", name, list.Count, expectedSize));
            }
        }
    }


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
            ParseKeyword("joints");

            ParseLeftBrace();

            var joints = new List<Joint>();
            while (TokenQueue.Peek().Type != TokenType.RBrace)
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

                joints.Add(new Joint(boneName, parentIndex, posX, posY, posZ, orientX, orientY, orientZ));
            }
            ParseRightBrace();

            return joints;
        }
    }


    public class Md5AnimParser : Md5Parser
    {
        public Md5AnimFile Parse(IEnumerable<Token> tokens)
        {
            TokenQueue = new Queue<Token>(tokens);

            ParseKeyword("MD5Version");
            var version = ParseInt("version");

            ParseKeyword("commandline");
            var commandLineParameters = ParseQuotedString("command line parameters");


            ParseKeyword("numFrames");
            var numFrames = ParseInt("number of frames");

            ParseKeyword("numJoints");
            var numJoints = ParseInt("number of joints");

            ParseKeyword("frameRate");
            var frameRate = ParseInt("frame rate");

            ParseKeyword("numAnimatedComponents");
            var numAnimatedComponents = ParseInt("number of animated components");


            var skeletonJoints = ParseSkeletonJoints();
            AssertCorrectListSize("skeleton joints", skeletonJoints, numJoints);

            var bounds = ParseBounds();
            AssertCorrectListSize("frame bounds", bounds, numFrames);

            var baseFrames = ParseBaseFrames();
            AssertCorrectListSize("base frames", baseFrames, numJoints);

            var frames = ParseFrames(numAnimatedComponents);
            AssertCorrectListSize("frames", frames, numFrames);

            return new Md5AnimFile(version, commandLineParameters, frameRate, skeletonJoints, bounds, baseFrames, frames);
        }

        private IList<SkeletonJoint> ParseSkeletonJoints()
        {
            ParseKeyword("hierarchy");

            ParseLeftBrace();

            var joints = new List<SkeletonJoint>();
            while (TokenQueue.Peek().Type != TokenType.RBrace)
            {
                var boneName = ParseQuotedString("bone name");
                var parentIndex = ParseInt("parent index");
                var flags = ParseInt("flags");
                var startIndex = ParseInt("start index");

                joints.Add(new SkeletonJoint(boneName, parentIndex, flags, startIndex));
            }
            ParseRightBrace();

            return joints;
        }

        private IList<Bound> ParseBounds()
        {
            ParseKeyword("bounds");

            ParseLeftBrace();

            var bounds = new List<Bound>();
            while (TokenQueue.Peek().Type != TokenType.RBrace)
            {
                ParseLeftParenthesis();
                var minX = ParseFloat("min x");
                var minY = ParseFloat("min y");
                var minZ = ParseFloat("min z");
                ParseRightParenthesis();

                ParseLeftParenthesis();
                var maxX = ParseFloat("max x");
                var maxY = ParseFloat("max y");
                var maxZ = ParseFloat("max z");
                ParseRightParenthesis();

                bounds.Add(new Bound(minX, minY, minZ, maxX, maxY, maxZ));
            }
            ParseRightBrace();

            return bounds;
        }

        private IList<JointPosition> ParseBaseFrames()
        {
            ParseKeyword("baseframe");

            ParseLeftBrace();

            var bounds = new List<JointPosition>();
            while (TokenQueue.Peek().Type != TokenType.RBrace)
            {
                ParseLeftParenthesis();
                var positionX = ParseFloat("position x");
                var positionY = ParseFloat("position y");
                var positionZ = ParseFloat("position z");
                ParseRightParenthesis();

                ParseLeftParenthesis();
                var orientX = ParseFloat("orientation x");
                var orientY = ParseFloat("orientation y");
                var orientZ = ParseFloat("orientation z");
                ParseRightParenthesis();

                bounds.Add(new JointPosition(positionX, positionY, positionZ, orientX, orientY, orientZ));
            }
            ParseRightBrace();

            return bounds;
        }

        private IList<Frame> ParseFrames(int numAnimatedComponents)
        {
            var frames = new List<Frame>();
            while (TokenQueue.Peek().Type == TokenType.Word && TokenQueue.Peek().Value == "frame")
            {
                ParseKeyword("frame");

                var frameIndex = ParseInt("frame index");

                ParseLeftBrace();

                var parameters = new List<double>();
                while (TokenQueue.Peek().Type == TokenType.Float || TokenQueue.Peek().Type == TokenType.Integer)
                {
                    parameters.Add(ParseFloat("frame parameter"));
                }

                ParseRightBrace();

                AssertCorrectListSize("frame parameters", parameters, numAnimatedComponents);

                frames.Add(new Frame(frameIndex, parameters));
            }

            return frames;
        }
    }
}