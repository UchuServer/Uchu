using System;

namespace Uchu.Core
{
    /// <summary>
    /// Indicates that a struct is part of a struct packet
    /// and had properties that require special writing,
    /// like default properties and classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    public class StructAttribute : Attribute
    {
        
    }
}