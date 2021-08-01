using System;

namespace Uchu.Core
{
    /// <summary>
    /// Indicates a different length to write for
    /// the applied string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LengthAttribute : Attribute
    {
        /// <summary>
        /// Length of the string to read or write.
        /// </summary>
        public int StringLength { get; }

        /// <summary>
        /// Creates the length attribute.
        /// </summary>
        /// <param name="stringLength"></param>
        public LengthAttribute(int length)
        {
            this.StringLength = length;
        }
    }
}