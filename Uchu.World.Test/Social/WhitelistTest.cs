using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Uchu.Core.IO;
using Uchu.World.Social;

namespace Uchu.World.Test.Social
{
    public class MockFileResources : IFileResources
    {
        public string RootPath { get; }
        public Task<string> ReadTextAsync(string path)
        {
            throw new System.NotImplementedException();
        }

        public Task<byte[]> ReadBytesAsync(string path)
        {
            throw new System.NotImplementedException();
        }

        public byte[] ReadBytes(string path)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetAllFilesWithExtension(string extension)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetAllFilesWithExtension(string location, string extension)
        {
            throw new System.NotImplementedException();
        }

        public Stream GetStream(string path)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class WhitelistTest
    {
        private Whitelist _whitelist;

        /// <summary>
        /// Sets up the test.
        /// </summary>
        [SetUp]
        public void SetUpTest()
        {
            // Create the whitelist test object.
            _whitelist = new Whitelist(new MockFileResources());
            
            // Add the words used by the tests.
            _whitelist.AddWord("Test1");
            _whitelist.AddWord("Test2");
            _whitelist.AddWord("Test3");
            _whitelist.AddWord("Testn't");
            _whitelist.AddWord("non-Test");
            _whitelist.AddWord("Space Test");
            _whitelist.AddWord("Test1 Test3");
            _whitelist.AddWord("Test1 Test4");
        }

        /// <summary>
        /// Tests validating single words.
        /// </summary>
        [Test]
        public void TestSingleWords()
        {
            // Test normal words.
            Assert.AreEqual(_whitelist.CheckPhrase("Test1"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("test1"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("Test2"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("Test3"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("Test4"), new (byte, byte)[1] { (0, 5) });
            Assert.AreEqual(_whitelist.CheckPhrase("Space"), new (byte, byte)[1] { (0, 5) });
            
            // Test words with characters.
            Assert.AreEqual(_whitelist.CheckPhrase("Testn't"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("non-test"), new (byte, byte)[0]);
            
            // Test words with spaces.
            // TODO: Unimplemented; test would fail.
        }

        /// <summary>
        /// Tests validating multiple words.
        /// </summary>
        [Test]
        public void TestMultipleWords()
        {
            // Test normal words.
            Assert.AreEqual(_whitelist.CheckPhrase("Test1 Test2"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("Test1 Test2 Test3"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("   Test1 Test2"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("   Test5 Test2"), new (byte, byte)[1] { (3, 5) });
            Assert.AreEqual(_whitelist.CheckPhrase("   Test5 Test6"), new (byte, byte)[2] { (3, 5), (9,5) });
            
            // Test words with characters.
            Assert.AreEqual(_whitelist.CheckPhrase("Test1 Testn't"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("non-Test Testn't"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("Test4 Testn't"), new (byte, byte)[1] { (0, 5)});
            
            // Test words with spaces.
            // TODO: Unimplemented; test would fail.
        }

        /// <summary>
        /// Tests validating with punctuation.
        /// </summary>
        [Test]
        public void TestPunctuation()
        {
            Assert.AreEqual(_whitelist.CheckPhrase("Test1?"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("Test1!"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("Test1?!"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("Test1?Test2"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("(Test1 Test2)"), new (byte, byte)[0]);
            Assert.AreEqual(_whitelist.CheckPhrase("Test1-?"), new (byte, byte)[1] { (0, 6) });
            Assert.AreEqual(_whitelist.CheckPhrase("Test4?"), new (byte, byte)[1] { (0, 5) });
        }
    }
}