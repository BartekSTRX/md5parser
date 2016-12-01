namespace Parser.Data.Anim
{
    public class JointPosition
    {
        public JointPosition(double posX, double posY, double posZ, double orientX, double orientY, double orientZ)
        {
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            OrientX = orientX;
            OrientY = orientY;
            OrientZ = orientZ;
        }

        public double PosX { get; private set; }
        public double PosY { get; private set; }
        public double PosZ { get; private set; }
        public double OrientX { get; private set; }
        public double OrientY { get; private set; }
        public double OrientZ { get; private set; }
    }
}