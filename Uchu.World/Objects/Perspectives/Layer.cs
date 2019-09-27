namespace Uchu.World
{
    public static class Layer
    {
        public const long None = default;

        public const long Default = 1;

        public const long Environment = 1 << 1;

        public const long Npc = 1 << 2;

        public const long Smashable = 1 << 3;

        public const long Player = 1 << 4;

        public const long Enemy = 1 << 5;

        public const long Spawner = 1 << 6;

        public const long Hidden = 1 << 7;

        public const long All = long.MaxValue;
    }
}