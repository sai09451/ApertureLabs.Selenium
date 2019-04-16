using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ApertureLabs.Tools.CodeGeneration.Core.CodeGeneration
{
    public class RazorPageInfoFactory
    {
        #region Fields

        private readonly string razorProjectPath;
        private readonly string razorProjectDirectory;
        private readonly IMemoryCache cache;
        private readonly RazorProjectEngine razorProjectEngine;

        #endregion

        #region Constructor

        public RazorPageInfoFactory(string razorProjectPath, IMemoryCache cache)
        {
            this.razorProjectPath = razorProjectPath
                ?? throw new ArgumentNullException(nameof(razorProjectPath));
            this.cache = cache
                ?? throw new ArgumentNullException(nameof(cache));

            razorProjectDirectory = new FileInfo(razorProjectPath)
                .Directory
                .FullName;

            razorProjectEngine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(razorProjectDirectory));
        }

        #endregion

        #region Methods

        public CodeCompileUnit CreateCodeCompileUnit(
            IEnumerable<string> defaultImports,
            IEnumerable<RazorPageInfo> razorInfoPages)
        {
            var compileUnit = new CodeCompileUnit();

            foreach (var razorInfoPage in razorInfoPages)
            {
                // Retrieve or create the namespace.
                var nsUnit = compileUnit.Namespaces
                    .OfType<CodeNamespace>()
                    .FirstOrDefault(
                        n => n.Name.Equals(
                            razorInfoPage.Namespace,
                            StringComparison.Ordinal));

                if (nsUnit == null)
                {
                    nsUnit = new CodeNamespace(razorInfoPage.Namespace);
                    compileUnit.Namespaces.Add(nsUnit);
                }

                // Add the imports.
                foreach (var import in defaultImports)
                    nsUnit.Imports.Add(new CodeNamespaceImport(import));

                // Add interface.
                var interfaceUnit = new CodeTypeDeclaration(razorInfoPage.GeneratedInterfaceName);
                interfaceUnit.IsInterface = true;
                interfaceUnit.TypeAttributes = TypeAttributes.Public;

                if (razorInfoPage.Layout != null)
                {
                    interfaceUnit.BaseTypes.Add(
                        new CodeTypeReference(
                            razorInfoPage.Layout.GeneratedInterfaceName));
                }
                else if (razorInfoPage.IsViewComponent)
                {
                    interfaceUnit.BaseTypes.Add(new CodeTypeReference("IPageComponent"));
                }
                else
                {
                    interfaceUnit.BaseTypes.Add(new CodeTypeReference("IPageComponent"));
                }

                foreach (var viewComponent in razorInfoPage.IncludedViewComponents)
                {
                    var viewComponentMember = new CodeMemberProperty();
                    viewComponentMember.Name = viewComponent.Name;
                    viewComponentMember.Type = new CodeTypeReference(viewComponent.GeneratedFullInterfaceName);
                    viewComponentMember.HasGet = true;
                    interfaceUnit.Members.Add(viewComponentMember);
                }

                foreach (var partialPage in razorInfoPage.IncludedPartialPages)
                {
                    var partialViewProperty = new CodeMemberProperty();
                    partialViewProperty.Name = partialPage.Name;
                    partialViewProperty.Type = new CodeTypeReference(partialPage.GeneratedFullInterfaceName);
                    partialViewProperty.HasGet = true;
                    interfaceUnit.Members.Add(partialViewProperty);
                }

                nsUnit.Types.Add(interfaceUnit);

                // Add class.
                var classUnit = new CodeTypeDeclaration(razorInfoPage.GeneratedClassName);
                classUnit.IsClass = true;
                classUnit.TypeAttributes = TypeAttributes.Public;
                classUnit.BaseTypes.Add(new CodeTypeReference(interfaceUnit.Name));

                // Add constructor.
                var classCtorUnit = new CodeConstructor();
                classCtorUnit.Attributes = MemberAttributes.Public|MemberAttributes.Final;

                foreach (var viewComponent in razorInfoPage.IncludedViewComponents)
                {
                    var viewComponentMember = new CodeMemberProperty();
                    viewComponentMember.Name = viewComponent.Name;
                    viewComponentMember.Type = new CodeTypeReference(viewComponent.GeneratedFullInterfaceName);
                    viewComponentMember.HasGet = true;
                    viewComponentMember.Attributes = MemberAttributes.Public|MemberAttributes.ScopeMask;
                    interfaceUnit.Members.Add(viewComponentMember);
                }

                nsUnit.Types.Add(classUnit);
                compileUnit.Namespaces.Add(nsUnit);
            }

            return compileUnit;
        }

        public IEnumerable<RazorPageInfo> GenerateRazorInfoPages()
        {
            // Steps:
            // 1) Get a list of all razor pages and their razor names (the name
            //    that would be called for @Html.Partial(name)).
            // 2) For each razor page do the following:
            // 2.a) Determine if it's a view component.
            // 2.b) Get the Layout.
            // 2.c) Get all partial pages included and match them to the razor
            //      pages.
            // 2.d) Get all view components included and match them to the
            //      razor pages.
            // 2.e) Retrieve all model properties being referenced in the view.
            // 2.e.1) Should also include ViewBag and ViewData properties.

            // Get all razor files.
            var fileInfoDictionary = new Dictionary<RazorProjectItem, RazorPageInfo>();

            // Step 1.
            foreach (var file in EnumerateRazorFiles())
            {
                var razorInfo = new RazorPageInfo
                {
                    PhysicalPath = file.PhysicalPath,
                    RelativePath = file.RelativePhysicalPath
                };

                fileInfoDictionary.Add(file, razorInfo);
            }

            // Step 2.
            foreach (var (file, fileInfo) in fileInfoDictionary)
            {
                var text = File.ReadAllText(file.PhysicalPath);

                // Step 2.a.
                fileInfo.IsViewComponent = IsViewComponent(file.PhysicalPath);
                fileInfo.Namespace = GetNamespace(file);

                // Step 2.b.
                var layout = GetLayout(text);

                if (!String.IsNullOrEmpty(layout))
                {
                    fileInfo.Layout = LocateRazorPageInfo(
                        layout,
                        fileInfo.PhysicalPath,
                        fileInfoDictionary);
                }

                // Step 2.c.
                foreach (var partialPageName in GetAllPartialPages(text))
                {
                    var resolvedPage = LocateRazorPageInfo(
                        partialPageName,
                        file.PhysicalPath,
                        fileInfoDictionary);

                    fileInfo.IncludedPartialPages.Add(resolvedPage);
                }

                // Step 2.d.
                foreach (var viewComponent in GetAllViewComponents(text))
                {
                    var resolvedPage = LocateRazorPageInfo(
                        viewComponent,
                        file.PhysicalPath,
                        fileInfoDictionary);

                    fileInfo.IncludedViewComponents.Add(resolvedPage);
                }

                // Step 2.e.
                foreach (var propertyReference in GetAllPropertyReferences(text))
                {
                    fileInfo.IncludedProperties.Add(propertyReference);
                }
            }

            return fileInfoDictionary.Values;
        }

        private RazorPageInfo LocateRazorPageInfo(string desiredName,
            string currentViewPath,
            IDictionary<RazorProjectItem, RazorPageInfo> fileInfoDictionary)
        {
            if (String.IsNullOrEmpty(desiredName))
                throw new ArgumentNullException(nameof(desiredName));
            else if (String.IsNullOrEmpty(currentViewPath))
                throw new ArgumentNullException(nameof(currentViewPath));
            else if (fileInfoDictionary == null)
                throw new ArgumentNullException(nameof(fileInfoDictionary));

            var key = $"{nameof(RazorPageInfoFactory)}-{desiredName}-{currentViewPath}";
            var result = cache.Get<RazorPageInfo>(key);

            // Use cached version if available.
            if (result != null)
                return result;

            var desiredFileName = Path.GetFileNameWithoutExtension(desiredName);

            // Get all files matching the desiredFileName.
            var matchingFiles = fileInfoDictionary.Keys.Where(
                    f => Path
                        .GetFileNameWithoutExtension(f.FilePath)
                        .Equals(desiredFileName, StringComparison.Ordinal));

            // Check if there is a relative portion of the desiredName.
            var desiredParentFolders = GetParentFolders(desiredName);

            if (desiredParentFolders.Any())
            {
                var toBeRemoved = new List<RazorProjectItem>();

                // Verify the relative roots match up.
                foreach (var matchingFile in matchingFiles)
                {
                    var matchingParentFolders = GetParentFolders(matchingFile.FilePath)
                        .Take(desiredParentFolders.Count());

                    if (!Enumerable.SequenceEqual(matchingParentFolders, desiredParentFolders))
                        toBeRemoved.Add(matchingFile);
                }

                matchingFiles = matchingFiles.Except(toBeRemoved);
            }

            // Use the first matching project item if any.
            if (matchingFiles.Any())
            {
                var matchingFile = matchingFiles.First();
                result = fileInfoDictionary[matchingFile];
            }

            if (result == null)
            {
                // Return instance of the unknown page info if not found.
                result = RazorPageInfo.UnkownPageInfo();
            }

            // Cache and return the result.
            return cache.GetOrCreate(key, entry => result);
        }

        private static IEnumerable<string> GetParentFolders(string name)
        {
            char seperator;

            if (name.Contains(Path.PathSeparator, StringComparison.Ordinal))
                seperator = Path.PathSeparator;
            else if (name.Contains(Path.DirectorySeparatorChar, StringComparison.Ordinal))
                seperator = Path.DirectorySeparatorChar;
            else if (name.Contains(Path.AltDirectorySeparatorChar, StringComparison.Ordinal))
                seperator = Path.AltDirectorySeparatorChar;
            else
                return Array.Empty<string>();

            // Reverse and skip the file name and any ~ characters.
            var parentFolders = name.Split(seperator)
                .Where(f => !f.Equals("~", StringComparison.Ordinal))
                .Reverse()
                .Skip(1);

            return parentFolders;
        }

        private IEnumerable<RazorProjectItem> EnumerateRazorFiles()
        {
            foreach (var file in razorProjectEngine.FileSystem.EnumerateItems("/"))
            {
                if (file.FilePath.EndsWith(".cshtml", StringComparison.Ordinal))
                    yield return file;
            }
        }

        private static bool IsViewComponent(string filePath)
        {
            var razorFileName = Path
                .GetFileNameWithoutExtension(filePath);

            return razorFileName.EndsWith(
                "ViewComponent",
                StringComparison.Ordinal);
        }

        private static string GetLayout(string text)
        {
            var layoutMatch = Regex.Match(
                    text,
                    "(?<=Layout\\s?=\\s?\")(.*)(?=\";)");

            return layoutMatch.Success ? layoutMatch.Groups[1].Value : null;
        }

        private static IEnumerable<string> GetAllPartialPages(string text)
        {
            var partialPagesAsyncHtmlMatch = Regex.Matches(
                    text,
                    "(?<=await Html\\.PartialAsync\\(\")(.*?)(?=\")");

            var partialPagesSyncHtmlMatch = Regex.Matches(
                text,
                "(?<=Html\\.Partial\\(\")(.*?)(?=\")");

            var partialPagesTagHelperMatch = Regex.Matches(
                text,
                "(?<=<partial\\s.*?name=\")(.*?)(?=\")");

            var allPartialPages = partialPagesAsyncHtmlMatch
                .Concat(partialPagesSyncHtmlMatch)
                .Concat(partialPagesTagHelperMatch);

            foreach (var match in allPartialPages)
            {
                if (match.Success)
                {
                    yield return match.Groups[1].Value;
                }
            }
        }

        private static IEnumerable<string> GetAllViewComponents(string text)
        {
            // Retrieve all viewcomponents.
            var viewCompInvokeAsyncMatches = Regex.Matches(
                text,
                "(?<=await Component\\.InvokeAsync\\(\")(.*?)(?=\")");

            var viewCompTagHelperMatches = Regex.Matches(
                text,
                @"(?<=<vc:)(.[^\s]*?)(?=\s)");

            var allViewComponentMatches = viewCompInvokeAsyncMatches
                .Concat(viewCompTagHelperMatches);

            foreach (var match in allViewComponentMatches)
            {
                if (match.Success)
                {
                    yield return match.Groups[1].Value;
                }
            }
        }

        private static IEnumerable<string> GetAllPropertyReferences(string text)
        {
            var allModelMembersMatch = Regex.Matches(
                text,
                @"(?<=Model\.)([^\(\)<>\s]+)");

            var allViewBagMatches = Regex.Matches(
                text,
                @"(?<=ViewBag\.)([^\s<>\(\)]*)");

            var allViewDataMatches = Regex.Matches(
                text,
                "(?<=ViewData\\[\")(.*?)(?=\"])");

            var allPropertyMatches = allModelMembersMatch
                .Concat(allViewBagMatches)
                .Concat(allViewDataMatches);

            foreach (var match in allPropertyMatches)
            {
                if (match.Success)
                {
                    yield return match.Groups[1].Value;
                }
            }
        }

        private static string GetNamespace(RazorProjectItem razorProjectItem)
        {
            var ns = String.Join(
                '.',
                GetParentFolders(razorProjectItem.RelativePhysicalPath).Reverse());

            // Replace all spaces with '_'.
            ns = Regex.Replace(
                ns,
                @"\s+",
                m => String.Concat(Enumerable.Repeat('_', m.Length)));

            // Append to all namespace segements that don't start with a letter
            // with a '_'.
            ns = Regex.Replace(ns, @"(?<=\.|^)([^a-zA-Z])", m => $"_{m.Value}");

            // Uppercase the first letter (if possible) of all namespace
            // segments.
            ns = Regex.Replace(
                ns,
                @"(?<=\.|^)([a-z])",
                m => m.Value.ToUpperInvariant());

            // Replace all '-' followed by a letter with an uppercase letter
            // if possible.
            ns = Regex.Replace(
                ns,
                @"-([a-z])",
                m => m.Groups[1].Value.ToUpperInvariant());

            // Remove all '-'.
            ns = Regex.Replace(ns, @"-", "");

            // Replace multiple periods with a single period.
            ns = Regex.Replace(ns, @"\.{2,}", ".");

            // Remove all non-char or non-digit characters.
            ns = Regex.Replace(ns, @"[^a-zA-Z0-9\._]", "");

            return ns;
        }

        //private NTree<SourceSpan> GetCodeAllBlockRegions(RazorProjectItem razorProjectItem)
        //{
        //    if (razorProjectItem == null)
        //        throw new ArgumentNullException(nameof(razorProjectItem));

        //    var sources = new List<SourceSpan>();
        //    var codeDoc = razorProjectEngine.Process(razorProjectItem);
        //    var codeBlockNodes = codeDoc.GetDocumentIntermediateNode()
        //        .FindPrimaryMethod()
        //        .FindDescendantNodes<CSharpCodeIntermediateNode>();

        //    foreach (var codeBlockNode in codeBlockNodes)
        //    {
        //        if (!codeBlockNode.Source.HasValue)
        //            continue;

        //        var span = codeBlockNode.Source.Value;

        //        // Check for any newlines.
        //        if (Regex.IsMatch(span.ToString(), @"\n"))
        //            sources.Add(span);
        //    }

        //    var tree = new List<NTree<SourceSpan>>();
        //    var orderedSources = sources.OrderBy(s => s.AbsoluteIndex);

        //    foreach (var source in orderedSources)
        //    {
        //        var sourceRange = new Range(source);

        //        var parentSourceSpan = tree
        //            .Where(s => sourceRange.IsInRange(new Range(s.Value)))
        //            .OrderByDescending(s => s.Value.AbsoluteIndex)
        //            .FirstOrDefault();

        //        if (parentSourceSpan == null)
        //        {
        //            // Add a root node.
        //        }
        //        else
        //        {
        //            var key = tree.Data
        //                .First(e => e.Value.Equals(parentSourceSpan))
        //                .Key;

        //            var node = tree[key];
        //        }
        //    }

        //    return tree;
        //}

        private struct Range
        {
            public Range(SourceSpan sourceSpan)
            {
                StartIndex = sourceSpan.AbsoluteIndex;
                EndIndex = sourceSpan.AbsoluteIndex + sourceSpan.Length;
                Length = sourceSpan.Length;
            }

            public int StartIndex { get; }
            public int EndIndex { get; }
            public int Length { get; }

            public bool IsInRange(Range range)
            {
                // Check if the range starts before and ends after this.
                return range.StartIndex < StartIndex
                    && range.EndIndex > EndIndex;
            }
        }

        #endregion
    }
}
