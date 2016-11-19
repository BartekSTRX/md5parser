using System;
using System.Collections.Generic;
using System.Globalization;
using Parser.Data;

namespace Parser
{
    public class Md5Parser
    {
        private Queue<Token> _tokenQueue;

        public Md5File Parse(IEnumerable<Token> tokens)
        {
            _tokenQueue = new Queue<Token>(tokens);

            ParseKeyword("MD5Version");
            var version = ParseInt("version");

            ParseKeyword("commandline");
            var commandLineParameters = ParseQuotedString("command line parameters");


            ParseKeyword("numJoints");
            var numJoints = ParseInt("number of joints");

            ParseKeyword("numMeshes");
            var numMeshes = ParseInt("number of meshes");


            var joints = ParseJoints();
            if (joints.Count != numJoints)
            {
                throw new Exception(string.Format(
                    "number of parsed joints ({0}) is different than expected ({1})", joints.Count, numJoints));
            }

            var meshes = ParseMeshes();
            if (meshes.Count != numMeshes)
            {
                throw new Exception(string.Format(
                    "number of parsed meshes ({0}) is different than expected ({1})", meshes.Count, numMeshes));
            }

            return new Md5File(version, commandLineParameters, joints, meshes);
        }

        private IList<Mesh> ParseMeshes()
        {
            var meshes = new List<Mesh>();

            while (_tokenQueue.Peek().Type == TokenType.Word &&_tokenQueue.Peek().Value == "mesh")
            {                
                ParseKeyword("mesh");
                ParseLeftBrace();

                ParseKeyword("shader");
                var shader = ParseQuotedString("shader");


                ParseKeyword("numverts");
                var numberOfVertices = ParseInt("number of vertices");

                var vertices = new List<Vertex>();
                while (_tokenQueue.Peek().Type == TokenType.Word && _tokenQueue.Peek().Value == "vert")
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
                if (vertices.Count != numberOfVertices)
                {
                    throw new Exception(
                        string.Format("number of vertices defined in file {0} is different than numverts value {1}",
                            vertices.Count, numberOfVertices));
                }


                ParseKeyword("numtris");
                var numberOfTriangles = ParseInt("number of triangles");

                var triangles = new List<Triangle>();
                while (_tokenQueue.Peek().Type == TokenType.Word && _tokenQueue.Peek().Value == "tri")
                {
                    ParseKeyword("tri");

                    var index = ParseInt("index");

                    var vertex1 = ParseInt("vertex 1 index");
                    var vertex2 = ParseInt("vertex 2 index");
                    var vertex3 = ParseInt("vertex 3 index");

                    triangles.Add(new Triangle(index, vertex1, vertex2, vertex3));
                }
                if (triangles.Count != numberOfTriangles)
                {
                    throw new Exception(
                        string.Format("number of triangles defined in file {0} is differnt than numtris value {1}",
                            triangles.Count, numberOfTriangles));
                }


                ParseKeyword("numweights");
                var numberOfWeights = ParseInt("number of weights");

                var weights = new List<Weight>();
                while (_tokenQueue.Peek().Type == TokenType.Word && _tokenQueue.Peek().Value == "weight")
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
                if (weights.Count != numberOfWeights)
                {
                    throw new Exception(
                        string.Format("number of weight in file {0} is different than numweights value {1}",
                            weights.Count, numberOfWeights));
                }


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
            while (_tokenQueue.Peek().Type != TokenType.RBrace)
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

        private void ParseLeftParenthesis()
        {
            var token = _tokenQueue.Dequeue();
            if (token.Type != TokenType.LParen)
            {
                throw new Exception("left parenthesis '(' expected");
            }
        }
        private void ParseRightParenthesis()
        {
            var token = _tokenQueue.Dequeue();
            if (token.Type != TokenType.RParen)
            {
                throw new Exception("right parenthesis ')' expected");
            }
        }

        private void ParseLeftBrace()
        {
            var token = _tokenQueue.Dequeue();
            if (token.Type != TokenType.LBrace)
            {
                throw new Exception("left brace '{' expected");
            }
        }
        private void ParseRightBrace()
        {
            var token = _tokenQueue.Dequeue();
            if (token.Type != TokenType.RBrace)
            {
                throw new Exception("right brace '}' expected");
            }
        }

        private int ParseInt(string name)
        {
            var token = _tokenQueue.Dequeue();
            if (token.Type != TokenType.Integer && token.Type != TokenType.Float)
            {
                throw new Exception(name + " expected (integer number)");
            }
            return int.Parse(token.Value);
        }

        private double ParseFloat(string name)
        {
            var token = _tokenQueue.Dequeue();
            if (token.Type != TokenType.Float)
            {
                throw new Exception(name + " expected (floating point number)");
            }
            return double.Parse(token.Value, CultureInfo.InvariantCulture);
        }

        private string ParseQuotedString(string name)
        {
            var token = _tokenQueue.Dequeue();
            if (token.Type != TokenType.QuotedString)
            {
                throw new Exception(name + " expected (quoted string)");
            }
            return token.Value;
        }

        private void ParseKeyword(string name)
        {
            var token = _tokenQueue.Dequeue();
            if (!(token.Type == TokenType.Word && token.Value == name))
            {
                throw new Exception(name + " keyword expected");
            }
        }
    }
}