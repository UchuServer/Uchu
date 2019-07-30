namespace Uchu.World
{
    public enum RebuildState : uint
    {
        Open,
        Completed = 2,
        Resetting = 4,
        Building,
        Incomplete
    }
}