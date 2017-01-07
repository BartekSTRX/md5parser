namespace Parser.Data.Mesh
{
    public class Weight
    {
        public Weight(int index, int jointIndex, float value, float posX, float posY, float posZ)
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
        public float Value { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
    }
}