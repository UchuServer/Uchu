namespace Uchu.World
{
    public struct LootMatrixEntry
    {
        public Lot Lot { get; set; }
        
        public int Priority { get; set; }
        
        public bool Mission { get; set; }
    }
}