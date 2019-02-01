using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests.PageObjects
{
    #region PageObjects

    public interface IMockA : IPageObject
    { }

    public class MockA : PageObject, IMockA
    {
        public MockA(IWebDriver driver) : base(driver)
        { }
    }

    public interface IMockB : IMockA
    { }

    public class MockB : PageObject, IMockB
    {
        public MockB(IMockA mockA, IWebDriver driver) : base(driver)
        { }
    }

    public interface IMockC : IPageObject
    { }

    public abstract class MockC : PageObject, IMockC
    {
        public MockC(IMockA mockA, IWebDriver driver) : base(driver)
        { }
    }

    public interface IMockD : IMockC
    { }

    public class MockD : PageObject, IMockD
    {
        public MockD(IMockA mockA, IMockB mockB, IWebDriver driver)
            : base(driver)
        { }
    }

    #endregion

    #region Test IOrderedModules

    public class ModuleA : Module, IOrderedModule
    {
        public int Order => 0;

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<MockA>()
                .As<IMockA>()
                .SingleInstance();

            base.Load(builder);
        }
    }

    public class ModuleB : Module, IOrderedModule
    {
        public int Order => 1;

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<MockB>()
                .As<IMockB>()
                .SingleInstance();

            builder
                .RegisterType<MockD>()
                .As<IMockD>()
                .SingleInstance();

            base.Load(builder);
        }
    }

    #endregion

    [TestClass]
    public class PageObjectFactoryTests
    {
        #region Fields

        private IPageObjectFactory pageObjectFactory;

        public TestContext TestContext { get; set; }

        #endregion

        #region Setup/Teardown

        #endregion

        #region Tests

        [TestMethod]
        public void PageObjectFactoryTest1()
        {
            var driver = new MockWebDriver();
            pageObjectFactory = new PageObjectFactory(driver);
            var loadedModules = pageObjectFactory.GetImportedModules();

            Assert.IsTrue(loadedModules.Count == 2);
            Assert.IsInstanceOfType(loadedModules[0], typeof(ModuleA));
            Assert.IsInstanceOfType(loadedModules[1], typeof(ModuleB));
        }

        [TestMethod]
        public void PageObjectFactoryTest2()
        {
            var driver = new MockWebDriver();
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IWebDriver>(driver);
            pageObjectFactory = new PageObjectFactory(
                services: serviceCollection,
                scanAssemblies: false,
                loadModules: true);
            var loadedModules = pageObjectFactory.GetImportedModules();

            Assert.IsTrue(loadedModules.Count == 2);
            Assert.IsInstanceOfType(loadedModules[0], typeof(ModuleA));
            Assert.IsInstanceOfType(loadedModules[1], typeof(ModuleB));
        }

        [TestMethod]
        public void PreparePageTest()
        {
            var driver = new MockWebDriver();
            pageObjectFactory = new PageObjectFactory(driver);

            var pageA = pageObjectFactory.PreparePage<IMockA>();
            var pageB = pageObjectFactory.PreparePage<IMockB>();
            var pageD = pageObjectFactory.PreparePage<IMockD>();

            Assert.IsNotNull(pageA);
            Assert.IsNotNull(pageB);
            Assert.IsNotNull(pageD);
        }

        #endregion
    }
}
