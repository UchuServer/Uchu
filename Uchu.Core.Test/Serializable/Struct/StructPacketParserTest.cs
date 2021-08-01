using System.IO;
using System.Numerics;
using NUnit.Framework;
using RakDotNet.IO;

namespace Uchu.Core.Test.Serializable.Structure
{
    public class StructPacketParserTest
    {
        /// <summary>
        /// Packet with a single property.
        /// </summary>
        public struct SinglePropertyPacket
        {
            public int TestProperty1 { get; set; }
        }
        
        /// <summary>
        /// Packet with multiple properties.
        /// </summary>
        public struct MultiplePropertyPacket
        {
            public int TestProperty1 { get; set; }
            public int TestProperty2 { get; set; }
            public string TestProperty3 { get; set; }
            public string TestProperty4 { get; set; }
            public Quaternion TestProperty5 { get; set; }
            [NiQuaternion]
            public Quaternion TestProperty6 { get; set; }
        }
        
        /// <summary>
        /// Packet with Default.
        /// </summary>
        [Struct]
        public struct DefaultPacket
        {
            [Default]
            public Vector3 TestProperty1 { get; set; }
            [Default("<0, 0, 0>")]
            public Vector3 TestProperty2 { get; set; }
            [Default]
            public int TestProperty3 { get; set; }
            [Default(4)]
            public int TestProperty4 { get; set; }
            public int TestProperty5 { get; set; }
        }
        
        /// <summary>
        /// Packet with a nested struct.
        /// </summary>
        [Struct]
        public struct NestedPacket
        {
            public int Value { get; set; }
            [Default]
            public DefaultPacket SubPacket { get; set; }
            public DefaultPacket[] SubPackets { get; set; }
        }
        
        /// <summary>
        /// Tests GetPacketProperties with only PacketProperties (no wrappers).
        /// </summary>
        [Test]
        public void TestGetPacketPropertiesPacketProperties()
        {
            // Test a packet with a single property.
            var singlePropertyPacket = StructPacketParser.GetPacketProperties(typeof(SinglePropertyPacket));
            Assert.AreEqual(singlePropertyPacket.Properties.Count, 1);
            Assert.IsTrue(singlePropertyPacket.Properties[0] is PacketProperty);
            Assert.AreEqual(singlePropertyPacket.Properties[0].StructProperty, typeof(SinglePropertyPacket).GetProperty("TestProperty1"));
            
            // Test a packet with multiple properties.
            var multiplePropertyPacket = StructPacketParser.GetPacketProperties(typeof(MultiplePropertyPacket));
            Assert.AreEqual(multiplePropertyPacket.Properties.Count, 6);
            Assert.IsTrue(multiplePropertyPacket.Properties[0] is PacketProperty);
            Assert.AreEqual(multiplePropertyPacket.Properties[0].StructProperty, typeof(MultiplePropertyPacket).GetProperty("TestProperty1"));
            Assert.IsTrue(multiplePropertyPacket.Properties[1] is PacketProperty);
            Assert.AreEqual(multiplePropertyPacket.Properties[1].StructProperty, typeof(MultiplePropertyPacket).GetProperty("TestProperty2"));
            Assert.IsTrue(multiplePropertyPacket.Properties[2] is StringPacketProperty);
            Assert.AreEqual(multiplePropertyPacket.Properties[2].StructProperty, typeof(MultiplePropertyPacket).GetProperty("TestProperty3"));
            Assert.IsTrue(multiplePropertyPacket.Properties[3] is StringPacketProperty);
            Assert.AreEqual(multiplePropertyPacket.Properties[3].StructProperty, typeof(MultiplePropertyPacket).GetProperty("TestProperty4"));
            Assert.IsTrue(multiplePropertyPacket.Properties[4] is PacketProperty);
            Assert.AreEqual(multiplePropertyPacket.Properties[4].StructProperty, typeof(MultiplePropertyPacket).GetProperty("TestProperty5"));
            Assert.IsTrue(multiplePropertyPacket.Properties[5] is NiQuaternionProperty);
            Assert.AreEqual(multiplePropertyPacket.Properties[5].StructProperty, typeof(MultiplePropertyPacket).GetProperty("TestProperty6"));
        }
        
        /// <summary>
        /// Tests GetPacketProperties with defaults.
        /// </summary>
        [Test]
        public void TestGetPacketPropertiesDefaults()
        {
            var hasFlagPropertyPacket = StructPacketParser.GetPacketProperties(typeof(DefaultPacket));
            Assert.AreEqual(hasFlagPropertyPacket.Properties.Count, 5);
            Assert.IsTrue(hasFlagPropertyPacket.Properties[0] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket.Properties[0].StructProperty, typeof(DefaultPacket).GetProperty("TestProperty1"));
            Assert.IsTrue(hasFlagPropertyPacket.Properties[1] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket.Properties[1].StructProperty, typeof(DefaultPacket).GetProperty("TestProperty2"));
            Assert.IsTrue(hasFlagPropertyPacket.Properties[2] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket.Properties[2].StructProperty, typeof(DefaultPacket).GetProperty("TestProperty3"));
            Assert.IsTrue(hasFlagPropertyPacket.Properties[3] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket.Properties[3].StructProperty, typeof(DefaultPacket).GetProperty("TestProperty4"));
            Assert.IsTrue(hasFlagPropertyPacket.Properties[4] is PacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket.Properties[4].StructProperty, typeof(DefaultPacket).GetProperty("TestProperty5"));
        }

        /// <summary>
        /// Tests WritePacket.
        /// </summary>
        [Test]
        public void TestWritePacket()
        {
            // Create the test packet.
            var packet = new DefaultPacket()
            {
                TestProperty1 = Vector3.One,
                TestProperty2 = Vector3.Zero,
                TestProperty3 = 4,
                TestProperty4 = 4,
                TestProperty5 = 4,
            };
            
            // Test writing the packet.
            var reader = new BitReader(StructPacketParser.WritePacket(packet));
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<Vector3>(), Vector3.One);
            Assert.IsFalse(reader.ReadBit());
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<int>(), 4);
            Assert.IsFalse(reader.ReadBit());
            Assert.AreEqual(reader.Read<int>(), 4);
        }
        
        /// <summary>
        /// Tests ReadPacket.
        /// </summary>
        [Test]
        public void TestReadPacket()
        {
            // Write the data.
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            bitWriter.WriteBit(true);
            bitWriter.Write<Vector3>(Vector3.One);
            bitWriter.WriteBit(true);
            bitWriter.Write<Vector3>(Vector3.One);
            bitWriter.WriteBit(true);
            bitWriter.Write<int>(2);
            bitWriter.WriteBit(false);
            bitWriter.Write<int>(3);

            // Test reading the packet.
            var packet = (DefaultPacket) StructPacketParser.ReadPacket(typeof(DefaultPacket), stream);
            Assert.AreEqual(packet.TestProperty1, Vector3.One);
            Assert.AreEqual(packet.TestProperty2, Vector3.One);
            Assert.AreEqual(packet.TestProperty3, 2);
            Assert.AreEqual(packet.TestProperty4, 4);
            Assert.AreEqual(packet.TestProperty5, 3);
        }

        /// <summary>
        /// Tests WritePacket with a nested struct.
        /// </summary>
        [Test]
        public void TestWritePacketNested()
        {
            // Create the test packet.
            var packet = new NestedPacket()
            {
                Value = 2,
                SubPacket = new DefaultPacket()
                {
                    TestProperty1 = Vector3.One,
                    TestProperty2 = Vector3.Zero,
                    TestProperty3 = 4,
                    TestProperty4 = 4,
                    TestProperty5 = 4,
                },
                SubPackets = new DefaultPacket[]
                {
                    new DefaultPacket()
                    {
                        TestProperty1 = Vector3.One,
                        TestProperty2 = Vector3.Zero,
                        TestProperty3 = 5,
                        TestProperty4 = 5,
                        TestProperty5 = 5,
                    },
                    new DefaultPacket()
                    {
                        TestProperty1 = Vector3.One,
                        TestProperty2 = Vector3.Zero,
                        TestProperty3 = 6,
                        TestProperty4 = 6,
                        TestProperty5 = 6,
                    },
                }
            };
            
            // Test writing the packet.
            var reader = new BitReader(StructPacketParser.WritePacket(packet));
            Assert.AreEqual(reader.Read<int>(), 2);
            Assert.IsTrue(reader.ReadBit());
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<Vector3>(), Vector3.One);
            Assert.IsFalse(reader.ReadBit());
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<int>(), 4);
            Assert.IsFalse(reader.ReadBit());
            Assert.AreEqual(reader.Read<int>(), 4);
            Assert.AreEqual(reader.Read<int>(), 2);
            
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<Vector3>(), Vector3.One);
            Assert.IsFalse(reader.ReadBit());
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<int>(), 5);
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<int>(), 5);
            Assert.AreEqual(reader.Read<int>(), 5);
            
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<Vector3>(), Vector3.One);
            Assert.IsFalse(reader.ReadBit());
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<int>(), 6);
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<int>(), 6);
            Assert.AreEqual(reader.Read<int>(), 6);
        }
        
        /// <summary>
        /// Tests ReadPacket with a nest struct.
        /// </summary>
        [Test]
        public void TestReadPacketNested()
        {
            // Write the data.
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            bitWriter.Write<int>(1);
            bitWriter.WriteBit(true);
            bitWriter.WriteBit(true);
            bitWriter.Write<Vector3>(Vector3.One);
            bitWriter.WriteBit(true);
            bitWriter.Write<Vector3>(Vector3.One);
            bitWriter.WriteBit(true);
            bitWriter.Write<int>(2);
            bitWriter.WriteBit(false);
            bitWriter.Write<int>(3);
            bitWriter.Write<int>(2);
            bitWriter.WriteBit(true);
            bitWriter.Write<Vector3>(Vector3.One);
            bitWriter.WriteBit(true);
            bitWriter.Write<Vector3>(Vector3.One);
            bitWriter.WriteBit(true);
            bitWriter.Write<int>(5);
            bitWriter.WriteBit(false);
            bitWriter.Write<int>(6);
            bitWriter.WriteBit(true);
            bitWriter.Write<Vector3>(Vector3.One);
            bitWriter.WriteBit(true);
            bitWriter.Write<Vector3>(Vector3.One);
            bitWriter.WriteBit(true);
            bitWriter.Write<int>(7);
            bitWriter.WriteBit(true);
            bitWriter.Write<int>(8);
            bitWriter.Write<int>(9);

            // Test reading the packet.
            var packet = (NestedPacket) StructPacketParser.ReadPacket(typeof(NestedPacket), stream);
            Assert.AreEqual(packet.Value, 1);
            Assert.AreEqual(packet.SubPacket.TestProperty1, Vector3.One);
            Assert.AreEqual(packet.SubPacket.TestProperty2, Vector3.One);
            Assert.AreEqual(packet.SubPacket.TestProperty3, 2);
            Assert.AreEqual(packet.SubPacket.TestProperty4, 4);
            Assert.AreEqual(packet.SubPacket.TestProperty5, 3);
            Assert.AreEqual(packet.SubPackets.Length, 2);
            Assert.AreEqual(packet.SubPackets[0].TestProperty1, Vector3.One);
            Assert.AreEqual(packet.SubPackets[0].TestProperty2, Vector3.One);
            Assert.AreEqual(packet.SubPackets[0].TestProperty3, 5);
            Assert.AreEqual(packet.SubPackets[0].TestProperty4, 4);
            Assert.AreEqual(packet.SubPackets[0].TestProperty5, 6);
            Assert.AreEqual(packet.SubPackets[1].TestProperty1, Vector3.One);
            Assert.AreEqual(packet.SubPackets[1].TestProperty2, Vector3.One);
            Assert.AreEqual(packet.SubPackets[1].TestProperty3, 7);
            Assert.AreEqual(packet.SubPackets[1].TestProperty4, 8);
            Assert.AreEqual(packet.SubPackets[1].TestProperty5, 9);
        }
    }
}