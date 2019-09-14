namespace Uchu.World
{
    public static class Layer
    {
        public static long None = default;
        
        public static long Default = 1;
        
        public static long Environment = 1 << 1;
        
        public static long Npc = 1 << 2;
        
        public static long Smashable = 1 << 3;
        
        public static long Player = 1 << 4;
        
        public static long Enemy = 1 << 5;
        
        public static long Spawner = 1 << 6;
        
        public static long Hidden = 1 << 7;

        public static long All = long.MaxValue;
    }
}