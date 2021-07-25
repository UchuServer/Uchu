using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using RakDotNet.IO;

namespace Uchu.Core
{
    public class StructPacketProperty : IPacketProperty
    {
        /// <summary>
        /// Property that is read from or read to.
        /// </summary>
        public PropertyInfo StructProperty { get; }

        /// <summary>
        /// Properties to write of the struct.
        /// </summary>
        public List<IPacketProperty> Properties { get; } = new List<IPacketProperty>();
        
        /// <summary>
        /// Creates the struct packet property.
        /// </summary>
        /// <param name="property">Struct property to write.</param>
        public StructPacketProperty(PropertyInfo property) : this(property?.PropertyType)
        {
            this.StructProperty = property;
        }
        
        /// <summary>
        /// Creates the struct packet property.
        /// </summary>
        /// <param name="structType">Type of the struct.</param>
        public StructPacketProperty(Type structType)
        {
            // Add the packet information.
            if (structType.GetCustomAttribute(typeof(PacketStructAttribute)) is PacketStructAttribute packetStruct)
            {
                this.Properties.Add(new PacketInformation(packetStruct.MessageIdentifier, packetStruct.RemoteConnectionType, packetStruct.PacketId));
            }
            
            // Convert the properties to PacketProperties.
            foreach (var property in structType?.GetProperties())
            {
                // Create the base packet property.
                IPacketProperty packetProperty = null;
                if (property.PropertyType == typeof(string))
                {
                    packetProperty = new StringPacketProperty(property);
                }
                else if (property.PropertyType == typeof(Quaternion) && property.GetCustomAttribute<NiQuaternionAttribute>() != null)
                {
                    packetProperty = new NiQuaternionProperty(property);
                }
                else if (property.PropertyType.IsValueType && property.PropertyType.GetCustomAttribute<StructAttribute>() != null)
                {
                    packetProperty = new StructPacketProperty(property);
                }
                else
                {
                    packetProperty = new PacketProperty(property);
                }
                if ((property.GetCustomAttribute(typeof(DefaultAttribute)) is DefaultAttribute defaultAttribute))
                {
                    packetProperty = new FlagPacketProperty(packetProperty, defaultAttribute.ValueToIgnore);
                }
                
                // Wrap the required properties.
                var requiredProperties = new Dictionary<string, RequiredPacketProperty>();
                foreach (var requiredAttribute in property.GetCustomAttributes<RequiresAttribute>())
                {
                    if (!requiredProperties.ContainsKey(requiredAttribute.PropertyName))
                    {
                        var requiredPacketProperty = new RequiredPacketProperty(packetProperty, requiredAttribute.PropertyName);
                        requiredProperties.Add(requiredAttribute.PropertyName, requiredPacketProperty);
                        packetProperty = requiredPacketProperty;
                    }
                    requiredProperties[requiredAttribute.PropertyName].RequiredValues.Add(requiredAttribute.ValueToRequire);
                }
                
                // Add the packet property.
                this.Properties.Add(packetProperty);
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
            // Convert the object if it is a property.
            if (this.StructProperty != null)
            {
                objectToWrite = this.StructProperty.GetValue(objectToWrite);
            }
            
            // Write the properties.
            var subWrittenProperties = new Dictionary<string, object>();
            foreach (var property in this.Properties)
            {
                property.Write(objectToWrite, writer, subWrittenProperties);
            }

            // Store the written string.
            if (writtenProperties != null && this.StructProperty != null)
            {
                writtenProperties[this.StructProperty.Name] = objectToWrite;
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
            // Convert the object if it is a property.
            var baseObjectToWrite = objectToWrite;
            if (this.StructProperty != null)
            {
                objectToWrite = Activator.CreateInstance(this.StructProperty.PropertyType);
            }
            
            // Read the struct.
            var subReadProperties = new Dictionary<string, object>();
            foreach (var property in this.Properties)
            {
                property.Read(objectToWrite, reader, subReadProperties, context);
            }

            // Store the written property.
            if (readProperties != null && this.StructProperty != null)
            {
                readProperties[this.StructProperty.Name] = objectToWrite;
                this.StructProperty.SetValue(baseObjectToWrite, objectToWrite);
            }
        }
    }
}