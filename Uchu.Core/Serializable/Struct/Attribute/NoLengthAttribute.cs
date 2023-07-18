using System;

namespace Uchu.Core
{
    /// <summary>
    /// Uses a break-bit for ending the array
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NoLengthAttribute : Attribute
    {
    }
}