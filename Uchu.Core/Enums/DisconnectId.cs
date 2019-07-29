namespace Uchu.Core
{
    public enum DisconnectId : uint
    {
        UnknownServerError,
        DuplicateLogin = 0x04,
        ServerShutdown,
        MapLoadFail,
        InvalidSessionKey,
        AccountNotPending,
        CharacterNotFound,
        CharacterCorruption,
        Kick,
        FreeTrialExpired = 0x0d,
        PlayScheduleEnd
    }
}