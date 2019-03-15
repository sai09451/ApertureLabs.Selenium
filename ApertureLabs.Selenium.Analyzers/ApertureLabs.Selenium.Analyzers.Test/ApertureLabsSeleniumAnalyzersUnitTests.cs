using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using ApertureLabs.Selenium.Analyzers;

namespace ApertureLabs.Selenium.Analyzers.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class My2Class : ApertureLabs.Selenium.PageObjects.IPageObject, IDisposable
        {
            public void Test() { }

            public void Dispose() { }

            //void IDisposable.Dispose() { }

            public virtual ILoadableComponent Load() { return this; }
        }

        public abstract class My1Page : IMyPage
        {
            public abstract void Test2();

            void IMyPage.Test() { }

            public virtual ILoadableComponent Load() { return this; }
        }

        public interface IMyPage : ApertureLabs.PageObjects.IPageObject
        {
            void Test();
        }
    }

    namespace ApertureLabs.Selenium.PageObjects
    {
        public interface IPageObject : OpenQA.Selenium.Support.PageObjects.ILoadableComponent { }

        public interface IPageComponent : OpenQA.Selenium.Support.PageObjects.ILoadableComponent { }
    }

    namespace OpenQA.Selenium.Support.PageObjects
    {
        public interface ILoadableComponent
        {
            ILoadableComponent Load();
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class My2ClassPage : ApertureLabs.Selenium.PageObjects.IPageObject, IDisposable
        {
            public void Test() { }

            public void Dispose() { }

            //void IDisposable.Dispose() { }

            public virtual ILoadableComponent Load() { return this; }
        }

        public abstract class My1Page : IMyPage
        {
            public abstract void Test2();

            void IMyPage.Test() { }

            public virtual ILoadableComponent Load() { return this; }
        }

        public interface IMyPage : ApertureLabs.PageObjects.IPageObject
        {
            void Test();
        }
    }

    namespace ApertureLabs.Selenium.PageObjects
    {
        public interface IPageObject : OpenQA.Selenium.Support.PageObjects.ILoadableComponent { }

        public interface IPageComponent : OpenQA.Selenium.Support.PageObjects.ILoadableComponent { }
    }

    namespace OpenQA.Selenium.Support.PageObjects
    {
        public interface ILoadableComponent
        {
            ILoadableComponent Load();
        }
    }";

            var expected1 = new DiagnosticResult
            {
                Id = "ApertureLabsSeleniumAnalyzersSuffix",
                Message = "Classes implementing IPageObject or IPageComponent should be suffixed with 'Page' and 'Component' respectively.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 22)
                    }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "ApertureLabsSeleniumAnalyzersPublicMembersVirtual",
                Message = "The public members of IPageObjects and IPageComponents should be virtual.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 25)
                    }
            };

            var expected3 = new DiagnosticResult
            {
                Id = "ApertureLabsSeleniumAnalyzersSuffix",
                Message = "The public members of IPageObjects and IPageComponents should be virtual.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 13, 25)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);
            VerifyCSharpFix(test, fixtest, 1);
        }



        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ApertureLabsSeleniumAnalyzersCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ApertureLabsSeleniumAnalyzersAnalyzer();
        }
    }
}
