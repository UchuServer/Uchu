namespace Uchu.World
{
    public struct LootMatrix
    {
        public int Minimum { get; set; }
        
        public int Maximum { get; set; }
        
        public float Percentage { get; set; }
        
        public LootMatrixEntry[] Entries { get; set; }
    }
}