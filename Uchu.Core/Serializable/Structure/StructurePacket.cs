using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core.Resources;

namespace Uchu.Core
{
    public abstract class StructurePacket : IPacket
    {
        /// <summary>
        /// Data class for a linked list node.
        /// LinkedListNode is not used due Next being read-only.
        /// </summary>
        /// <typeparam name="T">Type of value in the node.</typeparam>
        public class LinkedNode<T>
        {
            public T Value { get; set; }
            public LinkedNode<T> Next { get; set; }

            /// <summary>
            /// Creates the linked list node.
            /// </summary>
            /// <param name="value">Value of the node.</param>
            public LinkedNode(T value)
            {
                this.Value = value;
            }
            
            /// <summary>
            /// Returns the linked list as a string.
            /// </summary>
            /// <returns>Linked list as a string.</returns>
            public override string ToString()
            {
                return Value + " -> " + Next;
            }
        }
        
        /// <summary>
        /// Type of the connection.
        /// </summary>
        public abstract RemoteConnectionType RemoteConnectionType { get; }

        /// <summary>
        /// Id of the packet.
        /// </summary>
        public abstract uint PacketId { get; }
        
        /// <summary>
        /// Cache of the packet properties.
        /// These are compiled once and used for each read and write.
        /// </summary>
        private static readonly Dictionary<Type, List<IPacketProperty>> CachePacketProperties = new Dictionary<Type, List<IPacketProperty>>();

        /// <summary>
        /// Returns a linked list of writable properties for the given type.
        /// This is intended for the initial compile step and should only be called
        /// once per type.
        /// </summary>
        /// <param name="type">Type to check the properties of.</param>
        /// <returns>List of properties to write.</returns>
        /// <exception cref="InvalidOperationException">Properties are invalid.</exception>
        public static LinkedNode<PropertyInfo> GetWritableProperties(Type type)
        {
            var propertyNodes = new List<LinkedNode<PropertyInfo>>();
            var propertyChains = new List<LinkedNode<PropertyInfo>>();

            // Create the property nodes.
            foreach (var property in type.GetProperties())
            {
                // Add the property if it has a First attribute.
                // Only intended if the packet has a single property.
                if (property.GetCustomAttribute(typeof(First)) != null)
                {
                    var firstPropertyNode = new LinkedNode<PropertyInfo>(property);
                    propertyNodes.Add(firstPropertyNode);
                    propertyChains.Add(firstPropertyNode);
                    continue;
                }

                // Ignore the property if there is no After attribute (defines what property it follows).
                if (!(property.GetCustomAttribute(typeof(After)) is After afterAttribute)) continue;

                // Add the property node of the containing property.
                var propertyNode = new LinkedNode<PropertyInfo>(property);
                propertyNodes.Add(propertyNode);
                propertyChains.Add(propertyNode);

                // Throw an exception if the property it follows doesn't exist.
                var beforeProperty = type.GetProperty(afterAttribute.PropertyName);
                if (beforeProperty == null)
                {
                    throw new InvalidOperationException(property.Name + " comes after " +
                                                        afterAttribute.PropertyName + ", but doesn't exist.");
                }

                // Add the previous property if it doesn't have an After or First tag.
                if (beforeProperty.GetCustomAttribute(typeof(After)) != null ||
                    beforeProperty.GetCustomAttribute(typeof(First)) != null) continue;
                var beforePropertyNode = new LinkedNode<PropertyInfo>(beforeProperty);
                propertyNodes.Add(beforePropertyNode);
                propertyChains.Add(beforePropertyNode);
            }

            // Connect the properties.
            foreach (var propertyChain in propertyNodes.ToArray())
            {
                if (!(propertyChain.Value.GetCustomAttribute(typeof(After)) is After afterAttribute)) continue;

                // Throw an error if 1 property leads to 2 or more properties.
                var previousPropertyChain = propertyNodes.Find((existingProperty) =>
                    existingProperty.Value.Name == afterAttribute.PropertyName);
                if (previousPropertyChain == null) continue;
                if (previousPropertyChain.Next != null)
                {
                    throw new InvalidOperationException(afterAttribute.PropertyName + " has 2 or more next properties: " +
                                                        previousPropertyChain.Next.Value.Name + ", " + propertyChain.Value.Name);
                }

                // Set the next property.
                previousPropertyChain.Next = propertyChain;
                propertyChains.Remove(propertyChain);
            }

            // Return null if there is no property chain.
            if (propertyChains.Count == 0)
            {
                // Throw an exception if all property nodes were removed (no node is a start node).
                if (propertyNodes.Count > 0)
                {
                    throw new InvalidOperationException("Property order is cyclic.");
                }
                return null;
            }
            
            // Throw an exception if there is 2 or more valid starting properties.
            if (propertyChains.Count > 1)
            {
                throw new InvalidOperationException(
                    "There is more than 1 chain of properties (at least 1 After attribute is missing or incorrect). Starting values: " +
                    string.Join(", ", propertyChains.Select((chain) => chain.Value.Name).ToArray()));
            }

            // Return the only chain.
            return propertyChains[0];
        }

        /// <summary>
        /// Returns a list of packet properties for writing and reading packets.
        /// 
        /// </summary>
        /// <param name="type">Type to check the properties of.</param>
        /// <returns>List of properties to write.</returns>
        /// <exception cref="InvalidOperationException">Properties are invalid.</exception>
        public static List<IPacketProperty> GetPacketProperties(Type type)
        {
            // Populate the cache entry if needed.
            if (!CachePacketProperties.ContainsKey(type))
            {
                // Get the writable properties.
                var properties = GetWritableProperties(type);
                
                // Create the list to store the packet properties.
                var packetProperties = new List<IPacketProperty>();
                
                // Convert the properties to PacketProperties.
                while (properties != null)
                {
                    // Create the base packet property.
                    // TODO: Special cases required for string, wstring, and LDF
                    var propertyInfo = properties.Value;
                    IPacketProperty packetProperty = new PacketProperty(propertyInfo);
                    if ((propertyInfo.GetCustomAttribute(typeof(HasFlag)) is HasFlag hasFlagAttribute))
                    {
                        packetProperty = new FlagPacketProperty(packetProperty, hasFlagAttribute.ValueToIgnore);
                    }
                    packetProperties.Add(packetProperty);
                    
                    // Set the next property.
                    properties = properties.Next;
                }


                // Store the packet properties.
                CachePacketProperties[type] = packetProperties;
            }
            
            // Return the cache entry.
            return CachePacketProperties[type];
        }

        /// <summary>
        /// Serializes the packet to the bit writer.
        /// </summary>
        /// <param name="writer">Writer to serialize the data to.</param>
        /// <exception cref="ArgumentNullException">Writer is null.</exception>
        public virtual void Serialize(BitWriter writer)
        {
            // Throw an exception if the writer is null.
            if (writer == null)
                throw new ArgumentNullException(nameof(writer), 
                    ResourceStrings.Packet_Serialize_WriterNullException);
            
            // Write the base packet information.
            writer.Write((byte) MessageIdentifier.UserPacketEnum);
            writer.Write((ushort) RemoteConnectionType);
            writer.Write(PacketId);
            writer.Write<byte>(0);

            // Write the data.
            this.SerializePacket(writer);
        }
        
        /// <summary>
        /// Serializes the packet to the bit writer.
        /// </summary>
        /// <param name="writer">Writer to serialize the data to.</param>
        public virtual void SerializePacket(BitWriter writer)
        {
            var writtenProperties = new Dictionary<string, object>();
            foreach (var property in GetPacketProperties(this.GetType()))
            {
                property.Write(this, writer, writtenProperties);
            }
        }

        /// <summary>
        /// Deserializes the packet to the bit writer.
        /// </summary>
        /// <param name="reader">Reader to deserialize the data from.</param>
        public virtual void Deserialize(BitReader reader)
        {
            var readProperties = new Dictionary<string, object>();
            foreach (var property in GetPacketProperties(this.GetType()))
            {
                property.Read(this, reader, readProperties);
            }
        }
    }
}