using ApertureLabs.Tools.CodeGeneration.Core.Utilities;
using Humanizer;
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

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
            IEnumerable<string> defaultUsings,
            RazorPageInfo razorPageInfo,
            Document interfaceDocument)
        {
            CompilationUnitSyntax root;

            if (!interfaceDocument.TryGetSemanticModel(out var semanticModel))
                root = CompilationUnit();
            else
                root = semanticModel.SyntaxTree.GetCompilationUnitRoot();

            var walker = new Walker();
            walker.Visit(root);
            var uniqueNameVerifier = new UniqueNameVerifier();

            // Add the default imports.
            var usingDirList = List(
                defaultUsings.Where(u => !walker.DoesUsingExist(u)).Select(
                    u => UsingDirective(
                            ParseName(u))
                        .NormalizeWhitespace()));

            root = root.WithUsings(usingDirList);

            if (!walker.DoesNamespaceExist(razorPageInfo.Namespace))
            {
                // Need to create everything.
                root = root.WithMembers(
                    List(
                        new MemberDeclarationSyntax[]
                        {
                            NamespaceDeclaration(
                                name: IdentifierName(razorPageInfo.Namespace),
                                externs: List(
                                    Array.Empty<ExternAliasDirectiveSyntax>()),
                                usings: List(
                                    Array.Empty<UsingDirectiveSyntax>()),
                                members: List<MemberDeclarationSyntax>(
                                    new[]
                                    {
                                        CreateInterfaceDeclaration(razorPageInfo, uniqueNameVerifier)
                                    }))
                        }));

                return await GetNewProject(root, interfaceDocument)
                    .ConfigureAwait(false);
            }

            if (!walker.DoesInterfaceExist(
                razorPageInfo.GeneratedInterfaceName,
                razorPageInfo.Namespace))
            {
                // Create the interface.
                var ns = root.GetNamespace(razorPageInfo.Namespace)
                    .AddMembers(
                        CreateInterfaceDeclaration(
                            razorPageInfo,
                            uniqueNameVerifier));

                return await GetNewProject(ns, interfaceDocument)
                    .ConfigureAwait(false);
            }

            // TODO: Verify all properties exist on the interface.

            return await GetNewProject(root, interfaceDocument)
                .ConfigureAwait(false);
        }

        public async Task<Project> GeneratedClassDocumentAsync(
            IEnumerable<string> defaultUsings,
            RazorPageInfo razorPageInfo,
            Document classDocument)
        {
            CompilationUnitSyntax root;

            if (!classDocument.TryGetSemanticModel(out var semanticModel))
                root = CompilationUnit();
            else
                root = semanticModel.SyntaxTree.GetCompilationUnitRoot();

            var walker = new Walker();
            walker.Visit(root);
            var uniqueNameVerifier = new UniqueNameVerifier();

            // Add the default imports.
            var usingDirList = List(
                defaultUsings.Where(u => !walker.DoesUsingExist(u)).Select(
                    u => UsingDirective(
                            ParseName(u))
                        .NormalizeWhitespace()));

            root = root.WithUsings(usingDirList);

            if (!walker.DoesNamespaceExist(razorPageInfo.Namespace))
            {
                // Need to create everything.
                root = root.WithMembers(
                    List(
                        new MemberDeclarationSyntax[]
                        {
                            NamespaceDeclaration(
                                name: IdentifierName(razorPageInfo.Namespace),
                                externs: List(
                                    Array.Empty<ExternAliasDirectiveSyntax>()),
                                usings: List(
                                    Array.Empty<UsingDirectiveSyntax>()),
                                members: List<MemberDeclarationSyntax>(
                                    new[]
                                    {
                                        CreateClassDeclaration(razorPageInfo, uniqueNameVerifier)
                                    }))
                        }));

                var newProject = await GetNewProject(root, classDocument)
                    .ConfigureAwait(false);

                return newProject;
            }

            if (!walker.DoesClassExist(razorPageInfo.GeneratedClassName, razorPageInfo.Namespace))
            {
                // Create the class.
                var ns = root.GetNamespace(razorPageInfo.Namespace)
                    .AddMembers(
                        CreateClassDeclaration(
                            razorPageInfo,
                            uniqueNameVerifier));

                return await GetNewProject(ns, classDocument)
                    .ConfigureAwait(false);
            }

            var viewComponentsAndPartialViews = Enumerable.Concat(
                razorPageInfo.IncludedViewComponents,
                razorPageInfo.IncludedPartialPages);

            var propertyMembers = new List<MemberDeclarationSyntax>();

            foreach (var property in viewComponentsAndPartialViews)
            {
                // Check if the property already exists.
                if (!walker.DoesPropertyExist(property.GeneratedClassName,
                    razorPageInfo.GeneratedClassName,
                    razorPageInfo.Namespace))
                {
                    // Create the property.
                    var modifiers = TokenList(
                        Token(
                            SyntaxKind.PublicKeyword));

                    var accessorList = AccessorList(
                        List(
                            new AccessorDeclarationSyntax[]
                            {
                                AccessorDeclaration(
                                    SyntaxKind.GetAccessorDeclaration)
                            }));

                    var member = PropertyDeclaration(
                        attributeLists: List<AttributeListSyntax>(),
                        modifiers: modifiers,
                        type: ParseTypeName(property.GeneratedInterfaceName),
                        explicitInterfaceSpecifier: null,
                        identifier: Identifier(property.GeneratedClassName),
                        accessorList: accessorList);

                    propertyMembers.Add(member);
                }
            }

            var classNode = root.GetClass(
                    razorPageInfo.GeneratedClassName,
                    razorPageInfo.Namespace)
                .AddMembers(propertyMembers.ToArray());

            return await GetNewProject(classNode, classDocument)
                .ConfigureAwait(false);
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
                fileInfo.Namespace = GetValidNamespace(file);

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

                    if (resolvedPage.IsUnknownPage)
                        continue;

                    fileInfo.IncludedPartialPages.Add(resolvedPage);
                }

                // Step 2.d.
                foreach (var viewComponent in GetAllViewComponents(text))
                {
                    var resolvedPage = LocateRazorPageInfo(
                        viewComponent,
                        file.PhysicalPath,
                        fileInfoDictionary);

                    if (resolvedPage.IsUnknownPage)
                        continue;

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
            var root = await node.SyntaxTree.GetRootAsync()
                .ConfigureAwait(false);

            root = root.NormalizeWhitespace();

            return document.WithSyntaxRoot(root).Project;
        }

        /// <summary>
        /// [Type of razorInfoPage interface / property interface / property class / property name].
        /// </summary>
        /// <param name="razorPageInfo"></param>
        /// <param name="uniqueNameVerifier"></param>
        /// <returns></returns>
        public static IEnumerable<(string, string, string, string)> GetInheritedProperties(
            RazorPageInfo razorPageInfo,
            UniqueNameVerifier uniqueNameVerifier)
        {
            var currentLayout = razorPageInfo.Layout;

            while (currentLayout != null)
            {
                foreach (var (pInt, pCls, pName) in GetProperties(currentLayout, uniqueNameVerifier))
                {
                    yield return (currentLayout.GeneratedFullInterfaceName, pInt, pCls, pName);
                }

                currentLayout = currentLayout.Layout;
            }
        }

        private static IEnumerable<(string, string)> GetDependencies(
            RazorPageInfo razorPageInfo,
            UniqueNameVerifier uniqueNameVerifier)
        {
            var trackedDependencies = new List<RazorPageInfo>();

            // Get the dependencies for each inherited layout.
            var currentLayout = razorPageInfo.Layout;

            while (currentLayout != null)
            {
                if (trackedDependencies.Contains(currentLayout))
                    continue;

                var uniqueName = uniqueNameVerifier.MakeUniqueName(
                    currentLayout.GeneratedClassName.Camelize());

                yield return (currentLayout.GeneratedFullInterfaceName, uniqueName);

                trackedDependencies.Add(currentLayout);
                currentLayout = currentLayout.Layout;
            }
        }

        private static IEnumerable<(string, string, string)> GetProperties(
            RazorPageInfo razorPageInfo,
            UniqueNameVerifier uniqueNameVerifier)
        {
            var allProps = razorPageInfo.IncludedPartialPages
                .Concat(razorPageInfo.IncludedViewComponents);

            foreach (var prop in allProps)
            {
                var uniqueName = uniqueNameVerifier.MakeUniqueName(
                    prop.GeneratedClassName.Pascalize());

                yield return (prop.GeneratedFullInterfaceName,
                    prop.GeneratedFullClassName,
                    uniqueName);
            }
        }

        private static string TypeNameToArgumentName(string typeName)
        {
            var lastType = typeName.Split('.').Last();
            lastType = Regex.Replace(lastType, @"[^a-zA-Z]+", "");

            if (lastType[0].Equals('I'))
                lastType = lastType.Substring(1);

            lastType = Char.ToLowerInvariant(lastType[0]) + lastType.Substring(1);

            return lastType;
        }

        private static BaseListSyntax GetInterfaceBaseList(RazorPageInfo razorPageInfo)
        {
            var baseTypes = new List<BaseTypeSyntax>();

            if (razorPageInfo.Layout == null)
            {
                // Inherit from IPageComponent or IPageObject.
                var baseTypeName = razorPageInfo.IsViewComponent
                    ? "IPageComponent"
                    : "IPageObject";

                baseTypes.Add(
                    SimpleBaseType(
                        ParseTypeName(baseTypeName)));
            }
            else
            {
                baseTypes.Add(
                    SimpleBaseType(
                        ParseTypeName(razorPageInfo.Layout.GeneratedFullInterfaceName)));
            }

            return BaseList(SeparatedList(baseTypes));
        }

        private static BaseListSyntax GetClassBaseList(RazorPageInfo razorPageInfo)
        {
            var baseTypes = new List<BaseTypeSyntax>
            {
                SimpleBaseType(
                    ParseTypeName(
                        razorPageInfo.IsViewComponent ? "PageComponent" : "PageObject")),
                SimpleBaseType(
                    ParseTypeName(
                        razorPageInfo.GeneratedFullInterfaceName))
            };

            //if (razorPageInfo.Layout != null)
            //{
            //    baseTypes.Add(SimpleBaseType(
            //        ParseTypeName(
            //            razorPageInfo.Layout.GeneratedFullInterfaceName)));
            //}

            return BaseList(SeparatedList(baseTypes));
        }

        private static InterfaceDeclarationSyntax CreateInterfaceDeclaration(
            RazorPageInfo razorPageInfo,
            UniqueNameVerifier uniqueNameVerifier)
        {
            var properties = GetProperties(razorPageInfo, uniqueNameVerifier);
            var members = new List<MemberDeclarationSyntax>();

            foreach (var (propInt, propCls, propName) in properties)
            {
                var member = PropertyDeclaration(
                    ParseTypeName(propInt),
                    Identifier(propName))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(
                    AccessorList(
                        SingletonList(
                            AccessorDeclaration(
                                SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(
                                Token(SyntaxKind.SemicolonToken)))));

                members.Add(member);
            }

            var interfaceDecl = InterfaceDeclaration(
                Identifier(razorPageInfo.GeneratedInterfaceName))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithBaseList(GetInterfaceBaseList(razorPageInfo))
            .WithMembers(List(members));

            return interfaceDecl;
        }

        private static ClassDeclarationSyntax CreateClassDeclaration(
            RazorPageInfo razorPageInfo,
            UniqueNameVerifier uniqueNameVerifier)
        {
            var dependencies = GetDependencies(razorPageInfo, uniqueNameVerifier).ToList();
            var props = GetProperties(razorPageInfo, uniqueNameVerifier).ToList();
            var fields = new List<MemberDeclarationSyntax>();
            var ctors = new List<MemberDeclarationSyntax>();
            var properties = new List<MemberDeclarationSyntax>();
            var methods = new List<MemberDeclarationSyntax>();

            // Fields.
            foreach (var (depType, depName) in dependencies)
            {
                var modifiers = TokenList(
                    Token(SyntaxKind.PrivateKeyword),
                    Token(SyntaxKind.ReadOnlyKeyword));

                fields.Add(
                    FieldDeclaration(
                        List<AttributeListSyntax>(),
                        TokenList(
                            Token(SyntaxKind.PrivateKeyword),
                            Token(SyntaxKind.ReadOnlyKeyword)),
                        VariableDeclaration(
                            type: ParseTypeName(depType),
                            variables: SeparatedList(
                                new VariableDeclaratorSyntax[]
                                {
                                    VariableDeclarator(depName)
                                }))));
            }

            // Ctor.
            List<ParameterSyntax> parameters;
            
            // [Full type name / unique identifier].
            var ctorParameterDict = new Dictionary<string, string>();
            UniqueNameVerifier ctorUniqueNameVerifier;

            if (razorPageInfo.IsViewComponent)
            {
                parameters = new List<ParameterSyntax>
                {
                    Parameter(Identifier("by"))
                        .WithType(ParseTypeName("OpenQA.Selenium.By")),
                    Parameter(Identifier("driver"))
                        .WithType(ParseTypeName("OpenQA.Selenium.IWebDriver"))
                };

                ctorUniqueNameVerifier = new UniqueNameVerifier(
                    new[]
                    {
                        "by",
                        "driver"
                    });
            }
            else
            {
                parameters = new List<ParameterSyntax>
                {
                    Parameter(Identifier("driver"))
                        .WithType(ParseTypeName("OpenQA.Selenium.IWebDriver")),
                    Parameter(Identifier("baseUri"))
                        .WithType(ParseTypeName("System.Uri")),
                    Parameter(Identifier("route"))
                        .WithType(ParseTypeName("System.UriTemplate"))
                };

                ctorUniqueNameVerifier = new UniqueNameVerifier(
                    new[]
                    {
                        "driver",
                        "baseUri",
                        "route"
                    });
            }

            // Add the rest of the parameters.
            foreach (var (depType, depName) in dependencies)
            {
                var uniqueName = ctorUniqueNameVerifier.MakeUniqueName(
                    depName.Camelize());

                ctorParameterDict[depType] = uniqueName;

                var param = Parameter(
                    attributeLists: List<AttributeListSyntax>(),
                    modifiers: TokenList(),
                    type: ParseTypeName(depType),
                    identifier: Identifier(
                        TypeNameToArgumentName(uniqueName)),
                    @default: null);

                parameters.Add(param);
            }

            var body = Block(List<StatementSyntax>());

            foreach (var (depType, depName) in dependencies)
            {
                // This has become a little absurd. Not sure how to clean it
                // up.
                var binaryExpression = BinaryExpression(
                    SyntaxKind.CoalesceExpression,
                    IdentifierName(depName),
                    ThrowExpression(
                        ObjectCreationExpression(
                            IdentifierName("ArgumentNullException"))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(
                                        InvocationExpression(
                                            IdentifierName("nameof"))
                                        .WithArgumentList(
                                            ArgumentList(
                                                SingletonSeparatedList(
                                                    Argument(
                                                        IdentifierName(depName)))))))))));

                var statement = ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ThisExpression(),
                            IdentifierName(depName)),
                        binaryExpression));

                body = body.AddStatements(statement);
            }

            foreach (var (propInt, propCls, propName) in props)
            {
                var statement = ExpressionStatement(
                    AssignmentExpression(
                        kind: SyntaxKind.SimpleAssignmentExpression,
                        left: MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ThisExpression(),
                            IdentifierName(propName)),
                        right: ObjectCreationExpression(
                                IdentifierName(propCls))
                            .WithArgumentList(ArgumentList())));

                body = body.AddStatements(statement);
            }

            // Properties.
            foreach (var (propInt, propCls, propName) in props)
            {
                // Create properties for each 'local' property.
                var accessorList = AccessorList(
                    List(
                        new AccessorDeclarationSyntax[]
                        {
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        }));

                var modifiers = TokenList(
                    Token(SyntaxKind.PrivateKeyword),
                    Token(SyntaxKind.VirtualKeyword));

                var property = PropertyDeclaration(
                    ParseTypeName(propInt),
                    Identifier(propName))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(
                    AccessorList(
                        SingletonList(
                            AccessorDeclaration(
                                SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(
                                Token(SyntaxKind.SemicolonToken)))));

                properties.Add(property);
            }

            foreach (var (fromInt, propInt, propCls, propName) in GetInheritedProperties(razorPageInfo, uniqueNameVerifier))
            {
                var (depType, depName) = dependencies.First(
                    d => d.Item1.Equals(
                        fromInt,
                        StringComparison.Ordinal));

                // Implement inherited properties thru the injected dependencies.
                var property = PropertyDeclaration(
                        ParseTypeName(propInt),
                        Identifier(propName))
                    .WithModifiers(
                        TokenList(
                            new[]
                            {
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.VirtualKeyword)
                            }))
                    .WithExpressionBody(
                        ArrowExpressionClause(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(depName),
                                IdentifierName(propName))))
                    .WithSemicolonToken(
                        Token(SyntaxKind.SemicolonToken));

                properties.Add(property);
            }

            var baseArgs = new List<ArgumentSyntax>();

            if (razorPageInfo.IsViewComponent)
            {
                baseArgs.AddRange(
                    new[]
                    {
                        Argument(IdentifierName("by")),
                        Argument(IdentifierName("driver"))
                    });
            }
            else
            {
                baseArgs.AddRange(
                    new[]
                    {
                        Argument(IdentifierName("driver")),
                        Argument(IdentifierName("baseUri")),
                        Argument(IdentifierName("route"))
                    });
            }

            // Finish the ctor.
            ctors.Add(
                ConstructorDeclaration(
                    Identifier(
                        razorPageInfo.GeneratedClassName))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(
                    ParameterList(
                        SeparatedList(
                            parameters,
                            Enumerable.Repeat(
                                ParseToken("," + Environment.NewLine),
                                parameters.Count - 1))))
                .WithBody(body)
                .WithInitializer(
                    ConstructorInitializer(
                        SyntaxKind.BaseConstructorInitializer,
                        ArgumentList(
                            SeparatedList(
                                baseArgs)))));

            // Methods.
            if (properties.Count > 0)
            {
                var loadBody = new List<StatementSyntax>
                {
                    ExpressionStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                BaseExpression(),
                                IdentifierName("Load"))))
                };

                foreach (var (propInt, propCls, propName) in props)
                {
                    loadBody.Add(
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(propName),
                                    IdentifierName("Load")))));
                }

                foreach (var (depType, depName) in dependencies)
                {
                    loadBody.Add(
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(depName),
                                    IdentifierName("Load")))));
                }

                loadBody.Add(ReturnStatement(ThisExpression()));

                methods.Add(
                    MethodDeclaration(
                        ParseTypeName("ILoadableComponent"),
                        Identifier("Load"))
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword),
                            Token(SyntaxKind.OverrideKeyword)))
                    .WithBody(Block(loadBody)));
            }

            // Concat all members.
            var allMembers = fields
                .Concat(ctors)
                .Concat(properties)
                .Concat(methods);

            return ClassDeclaration(
                attributeLists: List<AttributeListSyntax>(),
                modifiers: TokenList(
                    Token(
                        SyntaxKind.PublicKeyword)),
                identifier: Identifier(razorPageInfo.GeneratedClassName),
                typeParameterList: null,
                baseList: GetClassBaseList(razorPageInfo),
                constraintClauses: List<TypeParameterConstraintClauseSyntax>(),
                members: List(allMembers));
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

        private static string GetValidNamespace(RazorProjectItem razorProjectItem)
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

        public class UniqueNameVerifier
        {
            private readonly IDictionary<string, int> names;

            public UniqueNameVerifier()
            {
                names = new Dictionary<string, int>();
            }

            public UniqueNameVerifier(IEnumerable<string> existingNames)
                : this()
            {
                foreach (var name in existingNames)
                    names.Add(name, 1);
            }

            public string MakeUniqueName(string name)
            {
                if (names.ContainsKey(name))
                {
                    var count = names[name];
                    names[name] += 1;

                    return $"{name}_{count}";
                }
                else
                {
                    names[name] = 1;

                    return name;
                }
            }
        }

        private class Walker : CSharpSyntaxWalker
        {
            public Walker()
            {
                Usings = new List<UsingDirectiveSyntax>();
                Namespaces = new List<NamespaceDeclarationSyntax>();
                Interfaces = new List<InterfaceDeclarationSyntax>();
                Classes = new List<ClassDeclarationSyntax>();
                Methods = new List<MethodDeclarationSyntax>();
                Properties = new List<PropertyDeclarationSyntax>();
                Fields = new List<FieldDeclarationSyntax>();
            }

            private IList<UsingDirectiveSyntax> Usings { get; }
            private IList<NamespaceDeclarationSyntax> Namespaces { get; }
            private IList<InterfaceDeclarationSyntax> Interfaces { get; }
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

            public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
            {
                base.VisitInterfaceDeclaration(node);

                Interfaces.Add(node);
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

            private NamespaceDeclarationSyntax GetNamespace(string @namespace)
            {
                return Namespaces.FirstOrDefault(
                    ns => ns.Name.ToString().Equals(
                        @namespace,
                        StringComparison.Ordinal));
            }

            public bool DoesInterfaceExist(string interfaceName, string @namespace)
            {
                return null != GetInterface(interfaceName, @namespace);
            }

            private InterfaceDeclarationSyntax GetInterface(string interfaceName,
                string @namespace)
            {
                var ns = GetNamespace(@namespace);

                if (ns == null)
                    return null;

                var interfaceNode = ns.DescendantNodes()
                    .OfType<InterfaceDeclarationSyntax>()
                    .FirstOrDefault(i => i.Identifier.ToString().Equals(
                        interfaceName,
                        StringComparison.Ordinal));

                return interfaceNode;
            }

            public bool DoesClassExist(string className, string @namespace)
            {
                return null != GetClass(className, @namespace);
            }

            private ClassDeclarationSyntax GetClass(string className, string @namespace)
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

            public bool DoesPropertyExist(string propertyName,
                string className,
                string @namespace)
            {
                var classNode = GetClass(className, @namespace);

                if (classNode == null)
                    return false;

                return classNode.DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .Any(
                        c => c.Identifier.ToString().Equals(
                            propertyName,
                            StringComparison.Ordinal));
            }
        }

        #endregion
    }
}
