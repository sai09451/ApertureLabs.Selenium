using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.WebElements.Inputs;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.UnitTests.PageObjects
{
    #region PageObjects

    public interface IMockA : IStaticPageObject
    { }

    public class MockA : StaticPageObject, IMockA
    {
        public MockA(IWebDriver driver)
            : base(driver, new Uri("https://www.google.com"))
        { }
    }

    public interface IMockB : IMockA
    { }

    public class MockB : StaticPageObject, IMockB
    {
        public MockB(IMockA mockA, IWebDriver driver)
            : base(driver, new Uri("https://www.google.com"))
        { }
    }

    public interface IMockC : IPageObject
    { }

    public abstract class MockC : ParameterPageObject, IMockC
    {
        public MockC(IMockA mockA, IWebDriver driver, UriTemplate template)
            : base(driver,
                  new Uri("www.google.com"),
                  template)
        { }
    }

    public interface IMockD : IMockC
    { }

    public class MockD : ParameterPageObject, IMockD
    {
        public MockD(IMockA mockA,
            IMockB mockB,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver)
            : base(driver,
                  new Uri("https://google.com"),
                  new UriTemplate("search?q={q}"))
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

        [ExpectedException(typeof(Exception), AllowDerivedTypes = false)]
        [TestMethod]
        public void FluidTest()
        {
            var factory = new WebDriverFactory();
            var driver = factory.CreateDriver(
                MajorWebDriver.Chrome,
                WindowSize.DefaultDesktop);

            pageObjectFactory = new PageObjectFactory(driver);

            using (driver)
            {
                pageObjectFactory
                    .StartWithPage<IMockA>()
                    .ContinueWithPage<IMockA, IMockB>(
                        pageA =>
                        {
                            var searchBar = new InputElement(
                                pageA.WrappedDriver.FindElement(
                                    By.CssSelector("*[name='q']")));

                            searchBar.SetValue("Testing 1 2 3");
                            searchBar.SendKeys(Keys.Enter);
                        },
                        pageA => pageObjectFactory.PreparePage<IMockB>())
                    .ContinueWithPage<IMockB, IMockD>(
                        pageB =>
                        {
                            Console.WriteLine("Testing with IMockB.");
                        },
                        pageB =>
                        {
                            var pageA = pageObjectFactory.PreparePage<IMockA>();
                            var pageD = new MockD(pageA,
                                pageB,
                                pageObjectFactory,
                                pageB.WrappedDriver);

                            pageD.Load(
                                new Dictionary<string, string>
                                {
                                    { "q", "selenium" }
                                });

                            return pageD;
                        })
                    .ContinueWithPage<IMockD>(
                        pageC =>
                        {
                            Console.WriteLine("Testing with IMockC.");

                            pageC.WrappedDriver
                                .Navigate()
                                .GoToUrl("https://www.google.com/search?q=testing");

                            Console.WriteLine("Should not reach here.");
                            throw new InternalTestFailureException();
                        });
            }
        }

        #endregion
    }
}
