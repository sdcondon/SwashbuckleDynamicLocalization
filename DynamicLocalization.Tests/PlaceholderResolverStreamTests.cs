namespace Swashbuckle.DynamicLocalization.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.IO;
    using System.Text;

    [TestClass]
    public class PlaceholderResolverStreamTests
    {
        [TestMethod]
        public void Read()
        {
            RunTest("1%(a)2", s => "VALUE", "1VALUE2");
        }

        [TestMethod]
        public void Read2()
        {
            RunTest("1%)2", s => "VALUE", "1%)2");
        }

        [TestMethod]
        public void Read3()
        {
            RunTest("1%(2", s => "VALUE", "1%(2");
        }

        [TestMethod]
        public void Read4()
        {
            RunTest("1%(\r)2", s => "VALUE", "1%(\r)2");
        }

        private void RunTest(string testInput, Func<string, string> resolve, string expectedOutput)
        {
            using (Stream innerStream = new MemoryStream(Encoding.UTF8.GetBytes(testInput)))
            using (Stream resolverStream = new PlaceholderResolverStream(innerStream, resolve))
            using (StreamReader reader = new StreamReader(resolverStream))
            {
                Assert.AreEqual(expectedOutput, reader.ReadToEnd());
            }
        }
    }
}
