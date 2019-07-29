namespace Uchu.Auth
{
    public enum LoginCode : byte
    {
        Success = 0x01,
        Banned = 0x02,
        InsufficientPermissions = 0x05,
        InvalidLogin = 0x06,
        Locked = 0x07,
        InvalidUsername = 0x08,
        ActivationPending = 0x09,
        AccountDisabled = 0x0A,
        TimeExpired = 0x0B,
        FreeTrialEnded = 0x0C,
        NotAllowedByPlaySchedule = 0x0D,
        AccountUnactived = 0x0E
    }
}