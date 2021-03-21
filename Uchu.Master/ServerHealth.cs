namespace Uchu.Master
{
    public enum ServerHealth
    {
        Dead, // Missed all heart beats
        Unhealthy, // Missed 75% of the required heart beats
        SeverelyLagging, // Missed 50% of the required heart beats
        Lagging, // Missed a heart beat
        Healthy, // Hits all heart beats
    }
}