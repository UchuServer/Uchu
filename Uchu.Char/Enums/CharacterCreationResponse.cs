namespace Uchu.Char
{
    public enum CharacterCreationResponse : byte
    {
        Success,
        NameNotAllowed = 0x02,
        PredefinedNameInUse,
        CustomNameInUse
    }
}