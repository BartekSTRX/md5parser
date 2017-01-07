using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Parser.Data.Anim;
using Parser.Data.Mesh;

namespace Parser
{
    public class ParserMd5
    {
        private int ParseInt(string s)
        {
            return int.Parse(s, CultureInfo.InvariantCulture);
        }

        private float ParseFloat(string s)
        {
            return float.Parse(s, CultureInfo.InvariantCulture);
        }

        private string[] ParseTokens(string str)
        {
            return str.Split(' ')
                .Where(s => !s.All(char.IsWhiteSpace))
                .ToArray();
        }

        public Md5AnimFile ParseAnim(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            int version = 0;
            string commandLine = null;

            int numFrames = 0;
            int numJoints = 0;
            int frameRate = 0;
            int numAnimatedComponents = 0;

            IList<SkeletonJoint> skeletonJoints = new List<SkeletonJoint>();
            IList<Bound> bounds = new List<Bound>();
            IList<JointPosition> jointPositions = new List<JointPosition>();
            IList<Frame> frames = new List<Frame>();

            for (int index = 0; index < lines.Length; index++)
            {
                var tokens = ParseTokens(lines[index]);

                if (tokens.Length == 2 && tokens[0] == "MD5Version")
                {
                    version = ParseInt(tokens[1]);
                }
                else if (tokens.Length == 2 && tokens[0] == "commandline")
                {
                    commandLine = tokens[1];
                }
                else if (tokens.Length == 2 && tokens[0] == "numFrames")
                {
                    numFrames = ParseInt(tokens[1]);
                }
                else if (tokens.Length == 2 && tokens[0] == "numJoints")
                {
                    numJoints = ParseInt(tokens[1]);
                }
                else if (tokens.Length == 2 && tokens[0] == "frameRate")
                {
                    frameRate = ParseInt(tokens[1]);
                }
                else if (tokens.Length == 2 && tokens[0] == "numAnimatedComponents")
                {
                    numAnimatedComponents = ParseInt(tokens[1]);
                }
                else if (tokens.Length == 2 && tokens[0] == "hierarchy")
                {
                    index++;

                    for (int jointIndex = 0; jointIndex < numJoints; jointIndex++, index++)
                    {
                        var jointTokens = ParseTokens(lines[index]);

                        var name = jointTokens[0];
                        var parent = ParseInt(jointTokens[1]);
                        var flags = ParseInt(jointTokens[2]);
                        var startIndex = ParseInt(jointTokens[3]);

                        skeletonJoints.Add(new SkeletonJoint(name, parent, flags, startIndex));
                    }
                }
                else if (tokens.Length == 2 && tokens[0] == "bounds")
                {
                    index++;

                    for (int boundIndex = 0; boundIndex < numFrames; boundIndex++, index++)
                    {
                        var boundTokens = ParseTokens(lines[index]);

                        var minX = ParseFloat(boundTokens[1]);
                        var minY = ParseFloat(boundTokens[2]);
                        var minZ = ParseFloat(boundTokens[3]);

                        var maxX = ParseFloat(boundTokens[6]);
                        var maxY = ParseFloat(boundTokens[7]);
                        var maxZ = ParseFloat(boundTokens[8]);
                        
                        bounds.Add(new Bound(minX, minY, minZ, maxX, maxY, maxZ));
                    }
                }
                else if (tokens.Length == 2 && tokens[0] == "baseframe")
                {
                    index++;

                    for (int jointIndex = 0; jointIndex < numJoints; jointIndex++)
                    {
                        var jointTokens = ParseTokens(lines[index]);

                        var posX = ParseFloat(jointTokens[1]);
                        var posY = ParseFloat(jointTokens[2]);
                        var posZ = ParseFloat(jointTokens[3]);

                        var orientX = ParseFloat(jointTokens[6]);
                        var orientY = ParseFloat(jointTokens[7]);
                        var orientZ = ParseFloat(jointTokens[8]);

                        jointPositions.Add(new JointPosition(posX, posY, posZ, orientX, orientY, orientZ));
                    }
                }
                else if (tokens.Length == 3 && tokens[0] == "frame")
                {
                    index++;

                    var frameNumber = ParseInt(tokens[1]);

                    var parameters = new List<float>();

                    while (parameters.Count < numAnimatedComponents)
                    {
                        var frameValues = ParseTokens(lines[index]).Select((string s) => ParseFloat(s));

                        parameters.AddRange(frameValues);
                        index++;
                    }

                    frames.Add(new Frame(frameNumber, parameters));
                }
            }

            return new Md5AnimFile(version, commandLine, frameRate, skeletonJoints, bounds, jointPositions, frames);
        }


        public Md5MeshFile ParseMesh(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            int version = 0;
            string commandLine = null;
            int numJoints = 0;
            int numMeshes = 0;
            IList<Joint> joints = new List<Joint>();
            IList<Mesh> meshes = new List<Mesh>();

            for (int index = 0; index < lines.Length; index++)
            {
                var tokens = ParseTokens(lines[index]);

                if (tokens.Length == 2 && tokens[0] == "MD5Version")
                {
                    version = ParseInt(tokens[1]);
                }
                else if (tokens.Length == 2 && tokens[0] == "commandline")
                {
                    commandLine = tokens[1];
                }
                else if (tokens.Length == 2 && tokens[0] == "numJoints")
                {
                    numJoints = ParseInt(tokens[1]);
                }
                else if (tokens.Length == 2 && tokens[0] == "numMeshes")
                {
                    numMeshes = ParseInt(tokens[1]);
                }
                else if (tokens.Length == 2 && tokens[0] == "joints")
                {
                    index++;

                    for (int jointIndex = 0; jointIndex < numJoints; jointIndex++, index++)
                    {
                        var jointTokens = ParseTokens(lines[index]);

                        var name = jointTokens[0];
                        var parentIndex = ParseInt(jointTokens[1]);
                        var posX = ParseFloat(jointTokens[3]);
                        var poxY = ParseFloat(jointTokens[4]);
                        var posZ = ParseFloat(jointTokens[5]);

                        var orientX = ParseFloat(jointTokens[8]);
                        var orientY = ParseFloat(jointTokens[9]);
                        var orientZ = ParseFloat(jointTokens[10]);

                        joints.Add(new Joint(name, parentIndex, posX, poxY, posZ, orientX, orientY, orientZ));
                    }
                }
                else if (tokens.Length == 2 && tokens[0] == "mesh")
                {
                    index++;

                    var shader = ParseTokens(lines[index])[1];
                    index++;

                    var numVerts = ParseInt(ParseTokens(lines[index])[1]);
                    index++;

                    var vertices = new List<Vertex>();
                    for (int vertIndex = 0; vertIndex < numVerts; vertIndex++, index++)
                    {
                        var vertTokens = ParseTokens(lines[index]);

                        var idx = ParseInt(vertTokens[1]);
                        var texU = ParseFloat(vertTokens[3]);
                        var texV = ParseFloat(vertTokens[4]);
                        var weightIndex = ParseInt(vertTokens[6]);
                        var weightElems = ParseInt(vertTokens[7]);

                        vertices.Add(new Vertex(idx, texU, texV, weightIndex, weightElems));
                    }

                    var numtris = ParseInt(ParseTokens(lines[index])[1]);
                    index++;

                    var triangles = new List<Triangle>();
                    for (int triIndex = 0; triIndex < numtris; triIndex++, index++)
                    {
                        var triTokens = ParseTokens(lines[index]);

                        var idx = ParseInt(triTokens[1]);
                        var v1 = ParseInt(triTokens[2]);
                        var v2 = ParseInt(triTokens[3]);
                        var v3 = ParseInt(triTokens[4]);

                        triangles.Add(new Triangle(idx, v1, v2, v3));
                    }

                    var numWeights = ParseInt(ParseTokens(lines[index])[1]);
                    index++;

                    var weights = new List<Weight>();
                    for (int weightIndex = 0; weightIndex < numWeights; weightIndex++, index++)
                    {
                        var weightTokens = ParseTokens(lines[index]);

                        var idx = ParseInt(weightTokens[1]);
                        var jointIndex = ParseInt(weightTokens[2]);
                        var value = ParseFloat(weightTokens[3]);
                        var posX = ParseFloat(weightTokens[5]);
                        var posY = ParseFloat(weightTokens[6]);
                        var posZ = ParseFloat(weightTokens[7]);

                        weights.Add(new Weight(idx, jointIndex, value, posX, posY, posZ));
                    }

                    meshes.Add(new Mesh(shader, vertices, triangles, weights));
                }
            }

            return new Md5MeshFile(version, commandLine, joints, meshes);
        }
    }
}