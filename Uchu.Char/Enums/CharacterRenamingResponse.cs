namespace Uchu.Char
{
    public enum CharacterRenamingResponse : byte
    {
        Success,
        UnknownError,
        NameUnavailable,
        NameAlreadyInUse
    }
}