namespace Parser.Data
{
    public class Joint
    {
        public Joint(string boneName, int parentIndex, 
            double posX, double poxY, double posZ, 
            double orientX, double orientY, double orientZ)
        {
            BoneName = boneName;
            ParentIndex = parentIndex;
            PosX = posX;
            PoxY = poxY;
            PosZ = posZ;
            OrientX = orientX;
            OrientY = orientY;
            OrientZ = orientZ;
        }

        public string BoneName { get; private set; }
        public int ParentIndex { get; private set; }
        public double PosX { get; private set; }
        public double PoxY { get; private set; }
        public double PosZ { get; private set; }
        public double OrientX { get; private set; }
        public double OrientY { get; private set; }
        public double OrientZ { get; private set; }
    }
}