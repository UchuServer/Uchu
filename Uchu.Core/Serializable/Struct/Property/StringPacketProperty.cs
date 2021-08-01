using System.Collections.Generic;
using System.Reflection;
using RakDotNet.IO;

namespace Uchu.Core
{
    public class StringPacketProperty : IPacketProperty
    {
        /// <summary>
        /// Property that is read from or read to.
        /// </summary>
        public PropertyInfo StructProperty { get; }

        /// <summary>
        /// Length of the string to read or write.
        /// </summary>
        private int _length = 0;
        
        /// <summary>
        /// Whether the encoded string is "wide" (2 bytes per character).
        /// </summary>
        private bool _isWide = false;
        
        /// <summary>
        /// Creates the string packet property.
        /// </summary>
        /// <param name="property">String property to write.</param>
        public StringPacketProperty(PropertyInfo property)
        {
            // Store the property.
            this.StructProperty = property;
            
            // Determine the string properties.
            if (property.GetCustomAttribute(typeof(LengthAttribute)) is LengthAttribute length)
            {
                this._length = length.StringLength;
            }
            if (property.GetCustomAttribute(typeof(WideAttribute)) is WideAttribute)
            {
                this._isWide = true;
            }
        }

        /// <summary>
        /// Writes the property to the writer.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to write.</param>
        /// <param name="writer">Bit writer to write to.</param>
        /// <param name="writtenProperties">Properties that were previously written.</param>
        public void Write(object objectToWrite, BitWriter writer, Dictionary<string, object> writtenProperties)
        {
            // Write the string.
            var value = (string) this.StructProperty.GetValue(objectToWrite) ?? "";
            var length = this._length;
            if (length == 0)
            {
                length = value.Length;
                writer.Write<uint>((uint) length);
            }
            writer.WriteString(value, length, this._isWide);

            // Store the written string.
            if (writtenProperties != null)
            {
                writtenProperties[this.StructProperty.Name] = value;
            }
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
            // Read the property.
            var length = this._length;
            if (length == 0)
            {
                length = reader.Read<int>();
            }
            var value = reader.ReadString(length, this._isWide);
            this.StructProperty.SetValue(objectToWrite, value);

            // Store the written property.
            if (readProperties != null)
            {
                readProperties[this.StructProperty.Name] = this.StructProperty.GetValue(objectToWrite);
            }
        }
    }
}