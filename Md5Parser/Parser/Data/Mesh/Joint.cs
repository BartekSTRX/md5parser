namespace Parser.Data.Mesh
{
    public class Joint
    {
        public Joint(string boneName, int parentIndex, 
            float posX, float poxY, float posZ, 
            float orientX, float orientY, float orientZ)
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
        public float PosX { get; private set; }
        public float PoxY { get; private set; }
        public float PosZ { get; private set; }
        public float OrientX { get; private set; }
        public float OrientY { get; private set; }
        public float OrientZ { get; private set; }
    }
}