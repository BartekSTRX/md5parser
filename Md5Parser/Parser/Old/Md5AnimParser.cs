using System.Collections.Generic;
using Parser.Data.Anim;

namespace Parser.Old
{
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
            return ParseList("hierarchy", () =>
            {
                var boneName = ParseQuotedString("bone name");
                var parentIndex = ParseInt("parent index");
                var flags = ParseInt("flags");
                var startIndex = ParseInt("start index");

                return new SkeletonJoint(boneName, parentIndex, flags, startIndex);
            });
        }

        private IList<Bound> ParseBounds()
        {
            return ParseList("bounds", () =>
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

                return new Bound(minX, minY, minZ, maxX, maxY, maxZ);
            });
        }

        private IList<JointPosition> ParseBaseFrames()
        {
            return ParseList("baseframe", () =>
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

                return new JointPosition(positionX, positionY, positionZ, orientX, orientY, orientZ);
            });
        }

        private IList<Frame> ParseFrames(int numAnimatedComponents)
        {
            var frames = new List<Frame>();
            while (TokenQueue.Peek().Type == TokenType.Word && TokenQueue.Peek().Value == "frame")
            {
                ParseKeyword("frame");

                var frameIndex = ParseInt("frame index");

                ParseLeftBrace();

                var parameters = new List<float>();
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