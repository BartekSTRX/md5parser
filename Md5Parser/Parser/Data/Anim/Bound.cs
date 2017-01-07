namespace Parser.Data.Anim
{
    public class Bound
    {
        public Bound(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            MinX = minX;
            MinY = minY;
            MinZ = minZ;
            MaxX = maxX;
            MaxY = maxY;
            MaxZ = maxZ;
        }

        public float MinX { get; private set; }
        public float MinY { get; private set; }
        public float MinZ { get; private set; }
        public float MaxX { get; private set; }
        public float MaxY { get; private set; }
        public float MaxZ { get; private set; }
    }
}