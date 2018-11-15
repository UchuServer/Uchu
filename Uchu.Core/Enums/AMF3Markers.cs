namespace Uchu.Core
{
    public enum AMF3Markers : byte
    {
        Undefined,
        Null,
        False,
        True,
        Integer,
        Double,
        String,
        XML,
        Date,
        Array,
        Object,
        XMLEnd,
        ByteArray
    }
}