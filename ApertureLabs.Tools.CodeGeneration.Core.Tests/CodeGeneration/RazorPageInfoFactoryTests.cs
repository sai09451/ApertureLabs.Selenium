using ApertureLabs.Tools.CodeGeneration.Core.CodeGeneration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace ApertureLabs.Tools.CodeGeneration.Core.Tests.CodeGeneration
{
    [TestClass]
    public class RazorPageInfoFactoryTests
    {
        public TestContext TestContext { get; set; }

        private const string PATH_TO_RAZOR_PROJECT = @"C:\Users\Alexander\Documents\GitHub\ApertureLabs.Selenium\MockServer\MockServer.csproj";

        private static RazorPageInfoFactory RazorPageInfoFactory;

        [ClassInitialize]
        public static void ClassStartup(TestContext testContext)
        {
            var cacheOptions = Microsoft.Extensions.Options.Options
                .Create(new MemoryCacheOptions());

            RazorPageInfoFactory = new RazorPageInfoFactory(
                PATH_TO_RAZOR_PROJECT,
                new MemoryCache(cacheOptions));
        }

        [TestMethod]
        public void GenerateRazorPagesTest()
        {
            var razorInfoPages = RazorPageInfoFactory.GenerateRazorInfoPages();

            var kgridInfo = razorInfoPages.FirstOrDefault(
                f => Path.GetFileNameWithoutExtension(f.RelativePath).Equals(
                    "KGrid",
                    StringComparison.Ordinal));

            Assert.IsTrue(razorInfoPages.Any());
        }
    }
}
