using System;

namespace Uchu.Core
{
    [Flags]
    public enum ObjectIdFlags : long
    {
        Persistent = 1L << 32,
        Client = 1L << 46,
        Spawned = 1L << 58,
        Character = 1L << 60
    }
}