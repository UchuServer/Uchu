using System;

namespace Uchu.Core
{
    public enum ServerType
    {
        Authentication,
        Character,
        World,
        
        [Obsolete("Chat is handled by World", true)]
        Chat
    }
}