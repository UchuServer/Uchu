using System;

namespace Uchu.Core
{
    /// <summary>
    /// Overrides the type used for the length of an array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class StoreLengthAsAttribute : Attribute
    {
        /// <summary>
        /// Type that is used for the array length. 
        /// </summary>
        public Type Type { get; }
        
        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="type">Type that is used for the array length.</param>
        public StoreLengthAsAttribute(Type type)
        {
            this.Type = type;
        }
    }
}