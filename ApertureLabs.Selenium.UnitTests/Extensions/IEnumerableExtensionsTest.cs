using System.Linq;
using ApertureLabs.Selenium.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApertureLabs.Selenium.UnitTests.Extensions
{
    [TestClass]
    public class IEnumerableExtensionsTest
    {
        [TestMethod]
        public void ChunkTest1()
        {
            var initialItems = new[]
            {
                "first_a", "second_a", "third_a", "fourth_a",
                "first_b", "second_b", "third_b", "fourth_b",
                "first_c", "second_c", "third_c", "fourth_c",
                "first_d", "second_d", "third_d", "fourth_d",
                "first_e", "second_e", "third_e", "fourth_e",
                "first_f", "second_f", "third_f", "fourth_f",
                "first_g"
            };

            var chunked = initialItems.Chunk(4);
            var chunkedTotalCount = chunked.SelectMany(c => c).Count();

            Assert.AreEqual(chunkedTotalCount, initialItems.Count());
            Assert.AreEqual(1, chunked.Last().Count);
        }

        [TestMethod]
        public void ChunkTest2()
        {
            var initialItems = new[]
            {
                "first_g"
            };

            var chunked = initialItems.Chunk(4);
            var chunkedTotalCount = chunked.SelectMany(c => c).Count();

            Assert.AreEqual(chunkedTotalCount, initialItems.Count());
            Assert.AreEqual(1, chunked.Last().Count);
        }
    }
}
