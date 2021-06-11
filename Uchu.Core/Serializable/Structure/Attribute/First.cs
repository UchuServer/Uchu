using System;

namespace Uchu.Core
{
    /// <summary>
    /// Attribute for a packet property that is serialized
    /// first. This attribute is only required if there is
    /// no "After" property that would define this property
    /// as before.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class First : Attribute
    {
        
    }
}