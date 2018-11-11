namespace Uchu.Core
{
    public enum MissionState
    {
        Unavailable,
        Available,
        Active,
        ReadyToComplete = 4,
        Completed = 8,
        CompletedAvailable,
        CompletedActive,
        CompletedReadyToComplete = 12
    }
}