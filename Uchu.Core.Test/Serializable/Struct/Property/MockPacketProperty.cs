using System.Collections.Generic;
using System.Reflection;
using RakDotNet.IO;

namespace Uchu.Core.Test.Serializable.Structure.Property
{
    public class MockPacketProperty : IPacketProperty
    {
        /// <summary>
        /// Property that is read from or read to.
        /// </summary>
        public PropertyInfo StructProperty { get; }

        /// <summary>
        /// Whether the mock packet property was written.
        /// </summary>
        public bool DataWritten { get; set; } = false;
        
        /// <summary>
        /// Whether the mock packet property was read from.
        /// </summary>
        public bool DataRead { get; set; } = false;
        
        
        /// <summary>
        /// Creates the packet property.
        /// </summary>
        /// <param name="property">Property to write.</param>
        public MockPacketProperty(PropertyInfo property)
        {
            this.StructProperty = property;
        }

        /// <summary>
        /// Writes the property to the writer.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to write.</param>
        /// <param name="writer">Bit writer to write to.</param>
        /// <param name="writtenProperties">Properties that were previously written.</param>
        public void Write(object objectToWrite, BitWriter writer, Dictionary<string, object> writtenProperties)
        {
            this.DataWritten = true;
        }

        /// <summary>
        /// Reads the property to the reader.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to read.</param>
        /// <param name="reader">Bit reader to read to.</param>
        /// <param name="readProperties">Properties that were previously read.</param>
        /// <param name="context">Properties that provide context for reading, such as world zone ids.</param>
        public void Read(object objectToWrite, BitReader reader, Dictionary<string, object> readProperties, Dictionary<string, object> context)
        {
            this.DataRead = true;
        }
    }
}