using ApertureLabs.Selenium.CodeGeneration;
using ApertureLabs.Tools.CodeGeneration.Core.CodeGeneration;
using ApertureLabs.Tools.CodeGeneration.Core.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Tools.CodeGeneration.Core.Services
{
    public class SeleniumCodeGenerator : CodeGenerator
    {
        public override async Task<Project> Generate(
            Project originalProject,
            Project destinationProject,
            IProgress<CodeGenerationProgress> progress,
            CancellationToken cancellationToken)
        {
            originalProject = originalProject.AddRazorFiles();

            var memOptions = Microsoft.Extensions
                .Options
                .Options
                .Create(new MemoryCacheOptions());

            var razorPageInfoFactory = new RazorPageInfoFactory(
                originalProject.FilePath,
                new MemoryCache(memOptions));

            var razorPages = razorPageInfoFactory.GenerateRazorInfoPages();
            var imports = new[]
            {
                "ApertureLabs.Selenium",
                "ApertureLabs.Selenium.PageObjects",
                "OpenQA.Selenium",
                "OpenQA.Selenium.Support.Events",
                "OpenQA.Selenium.Support.PageObjects",
                "OpenQA.Selenium.Support.UI",
                "System",
                "System.Collections",
                "System.Collections.Generic"
            };

            foreach (var razorPage in razorPages)
            {
                var originalFile = originalProject.Documents.First(
                    d => d.FilePath.Equals(
                        razorPage.PhysicalPath,
                        StringComparison.Ordinal));

                destinationProject = GetOrCreateDocumentWithSameRelativePath(
                    originalFile,
                    destinationProject,
                    razorPage.GeneratedInterfaceName,
                    out var generatedInterfaceDoc,
                    out var interfaceRelativePath);

                destinationProject = await razorPageInfoFactory.GenerateInterfaceDocumentAsync(
                        imports,
                        razorPage,
                        generatedInterfaceDoc)
                    .ConfigureAwait(false);

                destinationProject = GetOrCreateDocumentWithSameRelativePath(
                    originalFile,
                    destinationProject,
                    razorPage.GeneratedClassName,
                    out var generatedClassDoc,
                    out var classRelativePath);

                destinationProject = await razorPageInfoFactory.GeneratedClassDocumentAsync(
                        imports,
                        razorPage,
                        generatedClassDoc)
                    .ConfigureAwait(false);
            }

            return destinationProject;
        }
    }
}