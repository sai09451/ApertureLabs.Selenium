using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public async Task<Project> GenerateInterfaceDocumentAsync(
            IEnumerable<string> defaultImports,
            RazorPageInfo razorPageInfo,
            Document interfaceDocument)
        {
            var semanticModel = await interfaceDocument.GetSemanticModelAsync()
                .ConfigureAwait(false);

            var syntaxRoot = await interfaceDocument.GetSemanticModelAsync()
                .ConfigureAwait(false);

            var root = semanticModel.SyntaxTree.GetCompilationUnitRoot();
            var walker = new Walker();
            walker.Visit(root);

            return interfaceDocument.Project;
        }

        public async Task<Project> GeneratedClassDocumentAsync(
            IEnumerable<string> defaultUsings,
            RazorPageInfo razorPageInfo,
            Document classDocument)
        {
            var semanticModel = await classDocument.GetSemanticModelAsync()
                .ConfigureAwait(false);

            var root = semanticModel.SyntaxTree.GetCompilationUnitRoot();
            var walker = new Walker();
            walker.Visit(root);

            // Add the default imports.
            var usingDirList = SyntaxFactory.List(
                defaultUsings.Where(u => !walker.DoesUsingExist(u)).Select(
                    u => SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName(u))));

            root = root.WithUsings(usingDirList);

            if (!walker.DoesNamespaceExist(razorPageInfo.Namespace))
            {
                // Need to create everything.
                var ns = SyntaxFactory.NamespaceDeclaration(
                    name: SyntaxFactory.IdentifierName(razorPageInfo.Namespace),
                    externs: SyntaxFactory.List(
                        Array.Empty<ExternAliasDirectiveSyntax>()),
                    usings: SyntaxFactory.List(
                        Array.Empty<UsingDirectiveSyntax>()),
                    members: SyntaxFactory.List<MemberDeclarationSyntax>(
                        new[]
                        {
                            CreateClassDeclaration(razorPageInfo)
                        }));

                var newProject = await GetNewProject(ns, classDocument)
                    .ConfigureAwait(false);

                return newProject;
            }

            if (!walker.DoesClassExist(razorPageInfo.GeneratedClassName, razorPageInfo.Namespace))
            {
                // Create the class.
                var ns = GetNamespace(root, razorPageInfo.Namespace);
                ns = ns.AddMembers(CreateClassDeclaration(razorPageInfo));

                var newProject = await GetNewProject(ns, classDocument)
                    .ConfigureAwait(false);

                return newProject;
            }

            var viewComponentsAndPartialViews = Enumerable.Concat(
                razorPageInfo.IncludedViewComponents,
                razorPageInfo.IncludedPartialPages);

            foreach (var properties in viewComponentsAndPartialViews)
            {
                
            }

            return classDocument.Project;
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

        private static async Task<Project> GetNewProject(
            SyntaxNode node,
            Document document)
        {
            var root = await node.SyntaxTree.GetRootAsync();

            return document.WithSyntaxRoot(root).Project;
        }

        private static NamespaceDeclarationSyntax GetNamespace(SyntaxNode node,
            string @namespace)
        {
            return node.DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault(
                    ns => ns.Name.ToString().Equals(
                        @namespace,
                        StringComparison.Ordinal));
        }

        private static ClassDeclarationSyntax GetClass(SyntaxNode node,
            string className,
            string @namespace)
        {
            var ns = GetNamespace(node, @namespace);

            if (ns == null)
                return null;

            var classNode = ns.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(
                    c => c.Identifier.ToString().Equals(
                        className,
                        StringComparison.Ordinal));

            return classNode;
        }

        private PropertyDeclarationSyntax GetProperty(SyntaxNode node,
            string propertyName,
            string className,
            string @namespace)
        {
            var classNode = GetClass(node, className, @namespace);

            if (classNode == null)
                return null;

            var propertyNode = classNode.DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .FirstOrDefault(
                    c => c.Identifier.ToString().Equals(
                        propertyName,
                        StringComparison.Ordinal));

            return propertyNode;
        }

        private static IEnumerable<string> GetDependencies(RazorPageInfo razorPageInfo)
        {
            var currentLayout = razorPageInfo.Layout;

            while (currentLayout != null)
            {
                yield return currentLayout.GeneratedFullInterfaceName;
                currentLayout = currentLayout.Layout;
            }
        }

        private static string TypeNameToArgumentName(string typeName)
        {
            if (typeName[0].Equals('I'))
                typeName = typeName.Substring(1);

            typeName = Char.ToLowerInvariant(typeName[0]) + typeName.Substring(1);

            return typeName;
        }

        private static BaseListSyntax GetClassBaseList(RazorPageInfo razorPageInfo)
        {
            var baseTypes = new List<BaseTypeSyntax>
            {
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(
                        razorPageInfo.IsViewComponent ? "PageComponent" : "PageObject")),
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(
                        razorPageInfo.GeneratedInterfaceName))
            };

            if (razorPageInfo.Layout != null)
            {
                baseTypes.Add(SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(
                        razorPageInfo.Layout.GeneratedInterfaceName)));
            }

            return SyntaxFactory.BaseList(SyntaxFactory.SeparatedList(baseTypes));
        }

        private static ClassDeclarationSyntax CreateClassDeclaration(RazorPageInfo razorPageInfo)
        {
            var dependencies = GetDependencies(razorPageInfo);
            var fields = new List<MemberDeclarationSyntax>();
            var ctors = new List<MemberDeclarationSyntax>();
            var properties = new List<MemberDeclarationSyntax>();
            var methods = new List<MemberDeclarationSyntax>();

            // Fields.
            foreach (var dependency in dependencies)
            {
                var modifiers = SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));

                var fieldName = TypeNameToArgumentName(dependency);
                var declaration = SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.ParseTypeName(dependency),
                    variables: SyntaxFactory.SeparatedList(
                        new VariableDeclaratorSyntax[]
                        {
                            SyntaxFactory.VariableDeclarator(fieldName)
                        }));

                fields.Add(
                    SyntaxFactory.FieldDeclaration(
                        SyntaxFactory.List<AttributeListSyntax>(),
                        modifiers,
                        declaration));
            }

            // Ctor.
            var parameters = dependencies.Select(
                d => SyntaxFactory.Parameter(
                    attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                    modifiers: SyntaxFactory.TokenList(),
                    type: SyntaxFactory.ParseTypeName(d),
                    identifier: SyntaxFactory.Identifier(
                        TypeNameToArgumentName(d)),
                    @default: null));

            var body = SyntaxFactory.Block(SyntaxFactory.List<StatementSyntax>());

            foreach (var dependency in dependencies)
            {
                var dependencyName = TypeNameToArgumentName(dependency);

                var expression = SyntaxFactory.AssignmentExpression(
                    kind: SyntaxKind.EqualsKeyword,
                    left: SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.ThisExpression(),
                        SyntaxFactory.IdentifierName(dependencyName)),
                    right: SyntaxFactory.ParseExpression(dependencyName));

                var statement = SyntaxFactory.ExpressionStatement(expression);
                body = body.AddStatements(statement);
            }

            ctors.Add(
                SyntaxFactory.ConstructorDeclaration(
                    attributeLists: SyntaxFactory.List(Array.Empty<AttributeListSyntax>()),
                    modifiers: SyntaxFactory.TokenList(
                        SyntaxFactory.Token(
                            SyntaxKind.PublicKeyword)),
                    identifier: SyntaxFactory.ParseToken(
                        razorPageInfo.GeneratedClassName),
                    parameterList: SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList(
                            parameters)),
                    initializer: null,
                    body: body));

            // Properties.
            var props = Enumerable.Concat(
                razorPageInfo.IncludedViewComponents,
                razorPageInfo.IncludedPartialPages);

            foreach (var prop in props)
            {
                var accessorList = SyntaxFactory.AccessorList(
                    SyntaxFactory.List(
                        new AccessorDeclarationSyntax[]
                        {
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        }));

                var propName = prop.GeneratedClassName;

                var modifiers = SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                    SyntaxFactory.Token(SyntaxKind.VirtualKeyword));

                var property = SyntaxFactory.PropertyDeclaration(
                    attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                    modifiers: modifiers,
                    type: SyntaxFactory.ParseTypeName(prop.GeneratedFullInterfaceName),
                    explicitInterfaceSpecifier: null,
                    identifier: SyntaxFactory.Identifier(propName),
                    accessorList: accessorList);

                properties.Add(property);
            }

            // Methods.

            // Concat all members.
            var allMembers = fields.Concat(ctors)
                .Concat(properties)
                .Concat(methods);

            return SyntaxFactory.ClassDeclaration(
                attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                modifiers: SyntaxFactory.TokenList(
                    SyntaxFactory.Token(
                        SyntaxKind.PublicKeyword)),
                identifier: SyntaxFactory.Identifier(razorPageInfo.GeneratedClassName),
                typeParameterList: null,
                baseList: GetClassBaseList(razorPageInfo),
                constraintClauses: SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
                members: SyntaxFactory.List(allMembers));
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

        #endregion

        #region Nested Classes

        private class Walker : CSharpSyntaxWalker
        {
            public Walker()
            {
                Usings = new List<UsingDirectiveSyntax>();
                Namespaces = new List<NamespaceDeclarationSyntax>();
                Classes = new List<ClassDeclarationSyntax>();
                Methods = new List<MethodDeclarationSyntax>();
                Properties = new List<PropertyDeclarationSyntax>();
                Fields = new List<FieldDeclarationSyntax>();
            }

            private IList<UsingDirectiveSyntax> Usings { get; }
            private IList<NamespaceDeclarationSyntax> Namespaces { get; }
            private IList<ClassDeclarationSyntax> Classes { get; }
            private IList<MethodDeclarationSyntax> Methods { get; }
            private IList<PropertyDeclarationSyntax> Properties { get; }
            private IList<FieldDeclarationSyntax> Fields { get; }

            public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
            {
                base.VisitNamespaceDeclaration(node);

                Namespaces.Add(node);
            }

            public override void VisitUsingDirective(UsingDirectiveSyntax node)
            {
                base.VisitUsingDirective(node);

                Usings.Add(node);
            }

            public override void VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                base.VisitClassDeclaration(node);

                Classes.Add(node);
            }

            public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                base.VisitMethodDeclaration(node);
            }

            public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                base.VisitPropertyDeclaration(node);
            }

            public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                base.VisitFieldDeclaration(node);
            }

            public bool DoesUsingExist(string @using)
            {
                return Usings.Any(
                    u => u.Name.ToString().Equals(
                        @using,
                        StringComparison.Ordinal));
            }

            public bool DoesNamespaceExist(string @namespace)
            {
                return null != GetNamespace(@namespace);
            }

            public NamespaceDeclarationSyntax GetNamespace(string @namespace)
            {
                return Namespaces.FirstOrDefault(
                    ns => ns.Name.ToString().Equals(
                        @namespace,
                        StringComparison.Ordinal));
            }

            public bool DoesClassExist(string className, string @namespace)
            {
                return null != GetClass(className, @namespace);
            }

            public ClassDeclarationSyntax GetClass(string className, string @namespace)
            {
                var ns = GetNamespace(@namespace);

                if (ns == null)
                    return null;

                var classNode = ns.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .FirstOrDefault(
                        c => c.Identifier.ToString().Equals(
                            className,
                            StringComparison.Ordinal));

                return classNode;
            }

            public PropertyDeclarationSyntax GetProperty(string propertyName,
                string className,
                string @namespace)
            {
                var classNode = GetClass(className, @namespace);

                if (classNode == null)
                    return null;

                var propertyNode = classNode.DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .FirstOrDefault(
                        c => c.Identifier.ToString().Equals(
                            propertyName,
                            StringComparison.Ordinal));

                return propertyNode;
            }
        }

        #endregion
    }
}
