namespace Uchu.Core
{
    public enum AMF3Marker : byte
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