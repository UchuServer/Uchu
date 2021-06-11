using System;

namespace Uchu.Core
{
    /// <summary>
    /// Defines the property that is before the applied
    /// property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class After : Attribute
    {
        /// <summary>
        /// Property that is written/read before the property
        /// the attribute is applied to.
        /// </summary>
        public string PropertyName { get; } 
  
        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="propertyName">Property that is before the applied property.</param>
        public After(string propertyName)  
        {  
            this.PropertyName = propertyName;
        }  
    }
}