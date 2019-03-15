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

    public interface IMockAPage : IStaticPageObject
    { }

    public class MockAPage : StaticPageObject, IMockAPage
    {
        public MockAPage(IWebDriver driver)
            : base(driver, new Uri("https://www.google.com"))
        { }
    }

    public interface IMockBPage : IMockAPage
    { }

    public class MockBPage : StaticPageObject, IMockBPage
    {
        public MockBPage(IMockAPage mockA, IWebDriver driver)
            : base(driver, new Uri("https://www.google.com"))
        { }
    }

    public interface IMockCPage : IPageObject
    { }

    public abstract class MockCPage : ParameterPageObject, IMockCPage
    {
        public MockCPage(IMockAPage mockA, IWebDriver driver, UriTemplate template)
            : base(driver,
                  new Uri("www.google.com"),
                  template)
        { }
    }

    public interface IMockDPage : IMockCPage
    { }

    public class MockDPage : ParameterPageObject, IMockDPage
    {
        public MockDPage(IMockAPage mockA,
            IMockBPage mockB,
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
                .RegisterType<MockAPage>()
                .As<IMockAPage>()
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
                .RegisterType<MockBPage>()
                .As<IMockBPage>()
                .SingleInstance();

            builder
                .RegisterType<MockDPage>()
                .As<IMockDPage>()
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

            var pageA = pageObjectFactory.PreparePage<IMockAPage>();
            var pageB = pageObjectFactory.PreparePage<IMockBPage>();
            var pageD = pageObjectFactory.PreparePage<IMockDPage>();

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
                    .StartWithPage<IMockAPage>()
                    .ContinueWithPage<IMockAPage, IMockBPage>(
                        pageA =>
                        {
                            var searchBar = new InputElement(
                                pageA.WrappedDriver.FindElement(
                                    By.CssSelector("*[name='q']")));

                            searchBar.SetValue("Testing 1 2 3");
                            searchBar.SendKeys(Keys.Enter);
                        },
                        pageA => pageObjectFactory.PreparePage<IMockBPage>())
                    .ContinueWithPage<IMockBPage, IMockDPage>(
                        pageB =>
                        {
                            Console.WriteLine("Testing with IMockB.");
                        },
                        pageB =>
                        {
                            var pageA = pageObjectFactory.PreparePage<IMockAPage>();
                            var pageD = new MockDPage(pageA,
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
                    .ContinueWithPage<IMockDPage>(
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
