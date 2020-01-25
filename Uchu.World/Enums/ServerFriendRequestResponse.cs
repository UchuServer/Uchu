namespace Uchu.World
{
    public enum ServerFriendRequestResponse : byte
    {
        Accepted,
        AlreadyFriends,
        InvalidName,
        UnknownError,
        FriendListFull,
        FriendsFriendListFull,
        Declined,
        PlayerIsBusy,
        PlayerNotOnline,
        PendingApproval,
        CannotAddMythran,
        CancelledInvite,
        CannotAddFreeTrail
    }
}