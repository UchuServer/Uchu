using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using NUnit.Framework;

namespace Uchu.Core.Test.Serializable.Structure
{
    public class StructurePacketTest
    {
        /// <summary>
        /// Base test packet that has the abstract fields populated.
        /// </summary>
        public class TestStructurePacket : StructurePacket
        {
            public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;
            public override uint PacketId => 0x0;
        }
        
        /// <summary>
        /// Test class with a single property.
        /// </summary>
        public class SinglePropertyPacket : TestStructurePacket
        {
            [First]
            public string TestProperty1 { get; set; }
        }
        
        /// <summary>
        /// Test class with multiple properties.
        /// </summary>
        public class MultiplePropertyPacket : TestStructurePacket
        {
            public string TestProperty1 { get; set; }
            
            [After("TestProperty3")]
            public string TestProperty2 { get; set; }
            
            [After("TestProperty1")]
            public string TestProperty3 { get; set; }
            
            [After("TestProperty2")]
            public string TestProperty4 { get; set; }
        }
        
        /// <summary>
        /// Test class with multiple properties and an
        /// explicit first property.
        /// </summary>
        public class MultiplePropertyFirstPacket : TestStructurePacket
        {
            [First]
            public string TestProperty1 { get; set; }
            
            [After("TestProperty3")]
            public string TestProperty2 { get; set; }
            
            [After("TestProperty1")]
            public string TestProperty3 { get; set; }
            
            [After("TestProperty2")]
            public string TestProperty4 { get; set; }
        }
        
        /// <summary>
        /// Test class with a missing previous property.
        /// </summary>
        public class MissingPreviousPropertyPacket : TestStructurePacket
        {
            public string TestProperty1 { get; set; }
            
            [After("TestProperty2")]
            public string TestProperty2 { get; set; }
            
            [After("TestProperty4")]
            public string TestProperty3 { get; set; }
        }
        
        /// <summary>
        /// Test class with multiple starts.
        /// </summary>
        public class MultipleStartPropertiesPacket : TestStructurePacket
        {
            public string TestProperty1 { get; set; }
            
            [After("TestProperty1")]
            public string TestProperty2 { get; set; }
            
            public string TestProperty3 { get; set; }
            
            [After("TestProperty3")]
            public string TestProperty4 { get; set; }
        }
        
        /// <summary>
        /// Test class with cyclic properties.
        /// </summary>
        public class CyclicPropertiesPacket : TestStructurePacket
        {
            [After("TestProperty3")]
            public string TestProperty1 { get; set; }
            
            [After("TestProperty1")]
            public string TestProperty2 { get; set; }
            
            [After("TestProperty2")]
            public string TestProperty3 { get; set; }
        }
        
        /// <summary>
        /// Packet with HasFlag.
        /// </summary>
        public class HasFlagPacket : TestStructurePacket
        {
            [HasFlag]
            public Vector3 TestProperty1 { get; set; }
            
            [After("TestProperty1")]
            [HasFlag("<0, 0, 0>")]
            public Vector3 TestProperty2 { get; set; }
            
            [After("TestProperty2")]
            [HasFlag]
            public int TestProperty3 { get; set; }
            
            [After("TestProperty3")]
            [HasFlag(4)]
            public int TestProperty4 { get; set; }
            
            [After("TestProperty4")]
            public int TestProperty5 { get; set; }
        }
        
        /// <summary>
        /// Asserts the property chain is correct.
        /// </summary>
        /// <param name="properties">Property chain from GetWritableProperties</param>
        /// <param name="expectedProperties">List of the names of the expected properties</param>
        public static void AssertPropertyChain(StructurePacket.LinkedNode<PropertyInfo> properties, List<string> expectedProperties)
        {
            // Convert the properties chain to a list.
            var actualProperties = new List<string>();
            while (properties != null)
            {
                actualProperties.Add(properties.Value.Name);
                properties = properties.Next;
            }
            
            // Assert the properties are the same.
            Assert.AreEqual(actualProperties, expectedProperties);
        }
        
        /// <summary>
        /// Tests GetWritableProperties with normal cases.
        /// </summary>
        [Test]
        public void TestGetWritableProperties()
        {
            Assert.IsNull(StructurePacket.GetWritableProperties(typeof(TestStructurePacket)));
            AssertPropertyChain(StructurePacket.GetWritableProperties(typeof(SinglePropertyPacket)), new List<string>() { "TestProperty1" });
            AssertPropertyChain(StructurePacket.GetWritableProperties(typeof(MultiplePropertyPacket)), new List<string>() { "TestProperty1", "TestProperty3", "TestProperty2", "TestProperty4" });
            AssertPropertyChain(StructurePacket.GetWritableProperties(typeof(MultiplePropertyFirstPacket)), new List<string>() { "TestProperty1", "TestProperty3", "TestProperty2", "TestProperty4" });
        }
        
        /// <summary>
        /// Tests GetWritableProperties with error cases.
        /// </summary>
        [Test]
        public void TestGetWritablePropertiesErrorCases()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                StructurePacket.GetWritableProperties(typeof(MissingPreviousPropertyPacket));
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                StructurePacket.GetWritableProperties(typeof(MultipleStartPropertiesPacket));
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                StructurePacket.GetWritableProperties(typeof(CyclicPropertiesPacket));
            });
        }

        /// <summary>
        /// Tests GetPacketProperties with only PacketProperties (no wrappers).
        /// </summary>
        [Test]
        public void TestGetPacketPropertiesPacketProperties()
        {
            // Test a packet with a single property.
            var singlePropertyPacket = StructurePacket.GetPacketProperties(typeof(SinglePropertyPacket));
            Assert.AreEqual(singlePropertyPacket.Count, 1);
            Assert.IsTrue(singlePropertyPacket[0] is PacketProperty);
            Assert.AreEqual(singlePropertyPacket[0].Property, typeof(SinglePropertyPacket).GetProperty("TestProperty1"));
            
            // Test a packet with multiple properties.
            var multiplePropertyPacket = StructurePacket.GetPacketProperties(typeof(MultiplePropertyPacket));
            Assert.AreEqual(multiplePropertyPacket.Count, 4);
            Assert.IsTrue(multiplePropertyPacket[0] is PacketProperty);
            Assert.AreEqual(multiplePropertyPacket[0].Property, typeof(MultiplePropertyPacket).GetProperty("TestProperty1"));
            Assert.IsTrue(multiplePropertyPacket[1] is PacketProperty);
            Assert.AreEqual(multiplePropertyPacket[1].Property, typeof(MultiplePropertyPacket).GetProperty("TestProperty3"));
            Assert.IsTrue(multiplePropertyPacket[2] is PacketProperty);
            Assert.AreEqual(multiplePropertyPacket[2].Property, typeof(MultiplePropertyPacket).GetProperty("TestProperty2"));
            Assert.IsTrue(multiplePropertyPacket[3] is PacketProperty);
            Assert.AreEqual(multiplePropertyPacket[3].Property, typeof(MultiplePropertyPacket).GetProperty("TestProperty4"));
        }
        
        /// <summary>
        /// Tests GetPacketProperties with flags.
        /// </summary>
        [Test]
        public void TestGetPacketPropertiesFlags()
        {
            var hasFlagPropertyPacket = StructurePacket.GetPacketProperties(typeof(HasFlagPacket));
            Assert.AreEqual(hasFlagPropertyPacket.Count, 5);
            Assert.IsTrue(hasFlagPropertyPacket[0] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket[0].Property, typeof(HasFlagPacket).GetProperty("TestProperty1"));
            Assert.IsTrue(hasFlagPropertyPacket[1] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket[1].Property, typeof(HasFlagPacket).GetProperty("TestProperty2"));
            Assert.IsTrue(hasFlagPropertyPacket[2] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket[2].Property, typeof(HasFlagPacket).GetProperty("TestProperty3"));
            Assert.IsTrue(hasFlagPropertyPacket[3] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket[3].Property, typeof(HasFlagPacket).GetProperty("TestProperty4"));
            Assert.IsTrue(hasFlagPropertyPacket[4] is PacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket[4].Property, typeof(HasFlagPacket).GetProperty("TestProperty5"));
        }
    }
}