using System.Collections.Generic;
using NUnit.Framework;
using Uchu.Core.Config;

namespace Uchu.Core.Test.Config
{
    public class TestConfiguration
    {
        public string TestString { get; set; } = "test";
        [ConfigurationPath]
        public string TestPath1 { get; set; } = "test1";
        [ConfigurationPath]
        public string TestPath2 { get; set; } = "test1/test2";
        [ConfigurationPath]
        public string TestPath3 { get; set; } = "C:/test";
        [ConfigurationPath]
        public string TestPath4 { get; set; } = "C:\\test";
        [ConfigurationPath]
        public string TestPath5 { get; set; } = "/test";
        [ConfigurationPath]
        public string[] TestPaths1 { get; set; } = new string[]
        {
            "test1",
            "test1/test2",
            "/test",
        };
        [ConfigurationPath]
        public List<string> TestPaths2 { get; set; } = new List<string>
        {
            "test1",
            "test1/test2",
            "/test",
        };
    }
    
    public class ConfigurationPathAttributeTest
    {
        /// <summary>
        /// Asserts 2 paths are the same.
        /// </summary>
        /// <param name="expected">Expected path.</param>
        /// <param name="actual">Actual path.</param>
        public static void AssertPath(string expected, string actual)
        {
            Assert.AreEqual(expected.Replace('\\', '/'), actual.Replace('\\', '/'));
        }

        /// <summary>
        /// Tests replacing a null object to verify it doesn't fail.
        /// </summary>
        [Test]
        public void TestNull()
        {
            ConfigurationPathAttribute.ReplaceFilePaths("", null);
        }

        /// <summary>
        /// Tests replacing paths on Windows.
        /// </summary>
        [Test]
        public void TestWindowsPaths()
        {
            // Replace the configuration paths.
            var configuration = new TestConfiguration();
            ConfigurationPathAttribute.ReplaceFilePaths("C:\\test1\\test2\\config.xml", configuration);
            
            // Assert the strings are correct.
            AssertPath(configuration.TestString, "test");
            AssertPath(configuration.TestPath1, "C:\\test1\\test2\\test1");
            AssertPath(configuration.TestPath2, "C:\\test1\\test2\\test1\\test2");
            AssertPath(configuration.TestPath3, "C:\\test");
            AssertPath(configuration.TestPath4, "C:\\test");
            AssertPath(configuration.TestPath5, "\\test");
            
            // Assert the array of strings is correct.
            AssertPath(configuration.TestPaths1[0], "C:\\test1\\test2\\test1");
            AssertPath(configuration.TestPaths1[1], "C:\\test1\\test2\\test1\\test2");
            AssertPath(configuration.TestPaths1[2], "\\test");
            
            // Assert the list of strings is correct.
            AssertPath(configuration.TestPaths2[0], "C:\\test1\\test2\\test1");
            AssertPath(configuration.TestPaths2[1], "C:\\test1\\test2\\test1\\test2");
            AssertPath(configuration.TestPaths2[2], "\\test");
        }
        
        /// <summary>
        /// Tests replacing paths on Linux.
        /// </summary>
        [Test]
        public void TestLinuxPaths()
        {
            // Replace the configuration paths.
            var configuration = new TestConfiguration();
            ConfigurationPathAttribute.ReplaceFilePaths("/test1/test2/config.xml", configuration);
            
            // Assert the strings are correct.
            AssertPath(configuration.TestString, "test");
            AssertPath(configuration.TestPath1, "/test1/test2/test1");
            AssertPath(configuration.TestPath2, "/test1/test2/test1/test2");
            AssertPath(configuration.TestPath3, "C:/test");
            AssertPath(configuration.TestPath4, "C:/test");
            AssertPath(configuration.TestPath5, "/test");
            
            // Assert the array of strings is correct.
            AssertPath(configuration.TestPaths1[0], "/test1/test2/test1");
            AssertPath(configuration.TestPaths1[1], "/test1/test2/test1/test2");
            AssertPath(configuration.TestPaths1[2], "/test");
            
            // Assert the list of strings is correct.
            AssertPath(configuration.TestPaths2[0], "/test1/test2/test1");
            AssertPath(configuration.TestPaths2[1], "/test1/test2/test1/test2");
            AssertPath(configuration.TestPaths2[2], "/test");
        }
    }
}
