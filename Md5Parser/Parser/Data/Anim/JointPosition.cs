namespace Parser.Data.Anim
{
    public class JointPosition
    {
        public JointPosition(float posX, float posY, float posZ, float orientX, float orientY, float orientZ)
        {
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            OrientX = orientX;
            OrientY = orientY;
            OrientZ = orientZ;
        }

        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public float OrientX { get; private set; }
        public float OrientY { get; private set; }
        public float OrientZ { get; private set; }
    }
}