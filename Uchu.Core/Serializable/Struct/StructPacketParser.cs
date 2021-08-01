using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading;
using RakDotNet.IO;

namespace Uchu.Core
{
    public class StructPacketParser
    {
        /// <summary>
        /// Cache of the packet properties.
        /// These are compiled once and used for each read and write.
        /// </summary>
        private static readonly Dictionary<Type, StructPacketProperty> CachePacketProperties = new Dictionary<Type, StructPacketProperty>();

        /// <summary>
        /// Semaphore for accessing CachePacketProperties. In practice,
        /// state corruption was being logged.
        /// </summary>
        private static readonly SemaphoreSlim CachePacketPropertiesSemaphore = new SemaphoreSlim(1);
        
        /// <summary>
        /// Returns a list of packet properties for writing and reading packets.
        /// 
        /// </summary>
        /// <param name="type">Type to check the properties of.</param>
        /// <returns>List of properties to write.</returns>
        /// <exception cref="InvalidOperationException">Properties are invalid.</exception>
        public static StructPacketProperty GetPacketProperties(Type type)
        {
            // Populate the cache entry if needed.
            CachePacketPropertiesSemaphore.Wait();
            if (!CachePacketProperties.ContainsKey(type))
            {
                CachePacketProperties[type] = new StructPacketProperty(type);
            }
            CachePacketPropertiesSemaphore.Release();
            
            // Return the cache entry.
            return CachePacketProperties[type];
        }

        /// <summary>
        /// Writes the given packet struct to a memory stream.
        /// </summary>
        /// <param name="packet">Packet struct to write.</param>
        /// <typeparam name="T">Type of the packet.</typeparam>
        /// <returns>Memory stream of the packet to write to the network.</returns>
        public static MemoryStream WritePacket<T>(T packet) where T : struct
        {
            // Create the bit writer.
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream, leaveOpen: true);

            // Write the properties.
            var writtenProperties = new Dictionary<string, object>();
            GetPacketProperties(typeof(T)).Write(packet, bitWriter, writtenProperties);
            
            // Return the stream.
            bitWriter.Dispose();
            return stream;
        }
        
        /// <summary>
        /// Writes the given packet struct to a writer.
        /// </summary>
        /// <param name="packet">Packet struct to write.</param>
        /// <param name="bitWriter">Packet writer to write to.</param>
        /// <typeparam name="T">Type of the packet.</typeparam>
        public static void WritePacket<T>(T packet, BitWriter bitWriter) where T : struct
        {
            // Write the properties.
            var writtenProperties = new Dictionary<string, object>();
            GetPacketProperties(typeof(T)).Write(packet, bitWriter, writtenProperties);
        }
        
        /// <summary>
        /// Creates the given packet struct from the given memory stream.
        /// </summary>
        /// <param name="packetType">Type of the packet to read.</param>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="context">Properties that provide context for reading, such as world zone ids.</param>
        /// <returns>Packet object that was read.</returns>
        public static object ReadPacket(Type packetType, Stream stream, Dictionary<string, object> context = null)
        {
            // Create the bit reader.
            var bitReader = new BitReader(stream);

            // Read the properties.
            var packet = Activator.CreateInstance(packetType);
            var writtenProperties = new Dictionary<string, object>();
            GetPacketProperties(packetType).Read(packet, bitReader, writtenProperties, context);
            
            // Return the packet.
            bitReader.Dispose();
            return packet;
        }
    }
}