using System;

namespace Uchu.World
{
    /// <summary>
    /// This component is required on very object which is created with it and cannot be removed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EssentialAttribute : Attribute
    {
        
    }
}