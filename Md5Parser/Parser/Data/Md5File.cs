using System.Collections.Generic;

namespace Parser.Data
{
    public class Md5File
    {
        public Md5File(int version, string commandLine, IEnumerable<Joint> joints, IEnumerable<Mesh> meshes)
        {
            Version = version;
            CommandLine = commandLine;
            Joints = joints;
            Meshes = meshes;
        }

        public int Version { get; private set; }
        public string CommandLine { get; private set; }
        public IEnumerable<Joint> Joints { get; private set; }
        public IEnumerable<Mesh> Meshes { get; private set; }
    }
}