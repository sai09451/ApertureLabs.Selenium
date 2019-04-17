using ApertureLabs.Selenium.CodeGeneration;
using ApertureLabs.Tools.CodeGeneration.Core.CodeGeneration;
using ApertureLabs.Tools.CodeGeneration.Core.RazorParser;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
            var memOptions = Microsoft.Extensions
                .Options
                .Options
                .Create(new MemoryCacheOptions());

            var razorPageInfoFactory = new RazorPageInfoFactory(
                originalProject.FilePath,
                new MemoryCache(memOptions));

            var razorPages = razorPageInfoFactory.GenerateRazorInfoPages();

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
                        new[]
                        {
                            "ApertureLabs.Selenium",
                            "ApertureLabs.Selenium.PageObjects",
                            "System",
                            "System.Collections",
                            "System.Collections.Generic"
                        },
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
                        new[] { "System" },
                        razorPage,
                        generatedClassDoc)
                    .ConfigureAwait(false);
            }

            return destinationProject;
        }
    }
}