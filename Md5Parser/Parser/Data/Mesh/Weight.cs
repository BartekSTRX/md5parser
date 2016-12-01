namespace Parser.Data.Mesh
{
    public class Weight
    {
        public Weight(int index, int jointIndex, double value, double posX, double posY, double posZ)
        {
            Index = index;
            JointIndex = jointIndex;
            Value = value;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
        }

        public int Index { get; private set; }
        public int JointIndex { get; private set; }
        public double Value { get; private set; }
        public double PosX { get; private set; }
        public double PosY { get; private set; }
        public double PosZ { get; private set; }
    }
}