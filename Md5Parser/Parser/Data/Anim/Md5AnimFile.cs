using System.Collections.Generic;

namespace Parser.Data.Anim
{
    public class Md5AnimFile
    {
        public Md5AnimFile(int version, string commandLine, int frameRate, IEnumerable<SkeletonJoint> skeletonJoints, IEnumerable<Bound> bounds, IEnumerable<JointPosition> baseFrames, IEnumerable<Frame> frames)
        {
            Version = version;
            CommandLine = commandLine;
            FrameRate = frameRate;
            SkeletonJoints = skeletonJoints;
            Bounds = bounds;
            BaseFrames = baseFrames;
            Frames = frames;
        }

        public int Version { get; private set; }
        public string CommandLine { get; private set; }
        public int FrameRate { get; private set; }

        public IEnumerable<SkeletonJoint> SkeletonJoints { get; private set; }
        public IEnumerable<Bound> Bounds { get; private set; }
        public IEnumerable<JointPosition> BaseFrames { get; private set; }
        public IEnumerable<Frame> Frames { get; private set; }
    }
}