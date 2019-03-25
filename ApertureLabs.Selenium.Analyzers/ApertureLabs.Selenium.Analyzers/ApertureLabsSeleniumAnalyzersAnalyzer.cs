using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ApertureLabs.Selenium.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApertureLabsSeleniumAnalyzersAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Usage";
        public const string DiagnosticIdVirtualRule = "ApertureLabsSeleniumAnalyzersPublicMembersVirtual";
        public const string DiagnosticIdSuffixRule = "ApertureLabsSeleniumAnalyzersSuffix";
        public const string DiagnosticIdImplRule = "ApertureLabsSeleniumAnalyzersImpl";

        // You can change these strings in the Resources.resx file. If you do
        // not want your analyzer to be localize-able, you can use regular
        // strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md
        // for more on localization
        private static readonly LocalizableString TitleVirtualRule = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitleVirtual),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly LocalizableString MessageFormatVirtualRule = new LocalizableResourceString(
            nameof(Resources.AnalyzerMessageFormatVirtual),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly LocalizableString DescriptionVirtualRule = new LocalizableResourceString(
            nameof(Resources.AnalyzerDescriptionVirtual),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly DiagnosticDescriptor VirtualRule = new DiagnosticDescriptor(
            DiagnosticIdVirtualRule,
            TitleVirtualRule,
            MessageFormatVirtualRule,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DescriptionVirtualRule);

        private static readonly LocalizableString TitleSuffixRule = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitleSuffix),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly LocalizableString MessageFormatSuffixRule = new LocalizableResourceString(
            nameof(Resources.AnalyzerMessageFormatSuffix),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly LocalizableString DescriptionSuffixRule = new LocalizableResourceString(
            nameof(Resources.AnalyzerDescriptionSuffix),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly DiagnosticDescriptor SuffixRule = new DiagnosticDescriptor(
            DiagnosticIdSuffixRule,
            TitleSuffixRule,
            MessageFormatSuffixRule,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DescriptionSuffixRule);

        private static readonly LocalizableString TitleImplRule = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitleImpl),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly LocalizableString MessageFormatImplRule = new LocalizableResourceString(
            nameof(Resources.AnalyzerMessageFormatImpl),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly LocalizableString DescriptionImplRule = new LocalizableResourceString(
            nameof(Resources.AnalyzerDescriptionImpl),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly DiagnosticDescriptor ImplRule = new DiagnosticDescriptor(
            DiagnosticIdImplRule,
            TitleImplRule,
            MessageFormatImplRule,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DescriptionImplRule);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    VirtualRule,
                    SuffixRule,
                    ImplRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax
            // instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md
            // for more information
            context.RegisterSymbolAction(
                AnalyzePublicMembers,
                SymbolKind.NamedType);
            context.RegisterSymbolAction(
                AnalyzeNameSuffixes,
                SymbolKind.NamedType);
            context.RegisterSymbolAction(
                AnalyzeInterfaceImpls,
                SymbolKind.NamedType);

            context.RegisterSyntaxTreeAction(a => a.Tree.)
        }

        private static bool GetInterfaceTypeDefs(
            INamedTypeSymbol classSymbol,
            SymbolAnalysisContext context,
            out bool isPageObject,
            out bool isPageComponent)
        {
            isPageObject = false;
            isPageComponent = false;

            var pageObjectInterfaceType = context
                .Compilation
                .GetTypeByMetadataName("ApertureLabs.Selenium.PageObjects.IPageObject");

            var pageComponentInterfaceType = context
                .Compilation
                .GetTypeByMetadataName("ApertureLabs.Selenium.PageObjects.IPageComponent");

            if (pageObjectInterfaceType == null || pageComponentInterfaceType == null)
                return false;

            if (pageObjectInterfaceType != null)
            {
                isPageObject = classSymbol
                    .AllInterfaces
                    .Any(i => i.Equals(pageObjectInterfaceType));
            }

            if (pageComponentInterfaceType != null)
            {
                isPageComponent = classSymbol
                    .AllInterfaces
                    .Any(i => i.Equals(pageComponentInterfaceType));
            }

            return true;
        }

        private static void AnalyzePublicMembers(
            SymbolAnalysisContext context)
        {
            var classSymbol = (INamedTypeSymbol)context.Symbol;

            // Ignore if it's not a class or not a public class.
            if (classSymbol.TypeKind != TypeKind.Class
                || classSymbol.DeclaredAccessibility != Accessibility.Public)
            {
                return;
            }

            var usingPageObjectLib =  GetInterfaceTypeDefs(
                classSymbol,
                context,
                out var isPageObj,
                out var isPageCom);

            if (!usingPageObjectLib)
                return;

            // Ignore if not an IPageObject or IPageComponent.
            if (!isPageObj && !isPageCom)
                return;

            // Get all public members (except constructors).
            var publicSymbols = classSymbol
                .GetMembers()
                .Except(classSymbol.Constructors)
                .Where(member =>
                    member.DeclaredAccessibility == Accessibility.Public)
                .ToList();

            // Get all methods.
            var methodSymbols = publicSymbols
                .Where(member => member is IMethodSymbol)
                .Cast<IMethodSymbol>();

            // Get all properties.
            var propertySymbols = publicSymbols
                .Where(member => member is IPropertySymbol)
                .Cast<IPropertySymbol>();

            // Ignore the IDisposable.Dispose methods. That should be sealed.
            var disposeDecl = context.Compilation
                .GetTypeByMetadataName("System.IDisposable");

            var disposeMethodDecl = disposeDecl
                .GetMembers("Dispose")
                .Cast<IMethodSymbol>()
                .First();

            var disposeMethodImpl = classSymbol
                .FindImplementationForInterfaceMember(disposeMethodDecl);

            foreach (var methodSymbol in methodSymbols)
            {
                var ignore = methodSymbol.IsVirtual
                    || methodSymbol.IsOverride
                    || methodSymbol.IsSealed
                    || methodSymbol.IsStatic
                    || methodSymbol.Equals(disposeMethodImpl);

                if (ignore)
                    continue;

                var diagnostic = Diagnostic.Create(
                    VirtualRule,
                    methodSymbol.Locations[0],
                    methodSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }

            foreach (var propertySymbol in propertySymbols)
            {
                var ignore = propertySymbol.IsVirtual
                    || propertySymbol.IsSealed
                    || propertySymbol.IsOverride
                    || propertySymbol.IsStatic;

                if (ignore)
                    continue;

                var diagnostic = Diagnostic.Create(
                    VirtualRule,
                    propertySymbol.Locations[0],
                    propertySymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }

        private static void AnalyzeNameSuffixes(SymbolAnalysisContext context)
        {
            var classSymbol = (INamedTypeSymbol)context.Symbol;
            var usingPageObjectLib = GetInterfaceTypeDefs(
                classSymbol,
                context,
                out var isPageObj,
                out var isPageCom);

            if (!usingPageObjectLib)
                return;

            // Ignore if both IPageObject and IPageComponent.
            if (isPageObj && isPageCom)
                return;

            if (isPageObj)
            {
                if (!classSymbol.Name.EndsWith("Page") && !classSymbol.Name.EndsWith("PageObject"))
                {
                    var diagnostic = Diagnostic.Create(
                        descriptor: SuffixRule,
                        location: classSymbol.Locations[0],
                        properties: ImmutableDictionary.CreateRange(
                            new[]
                            {
                                new KeyValuePair<string, string>("suffix", "Page"),
                                new KeyValuePair<string, string>("explicitSuffix", "PageObject")
                            }),
                        messageArgs: classSymbol.Name);

                    context.ReportDiagnostic(diagnostic);
                }
            }
            else if (isPageCom)
            {
                if (!classSymbol.Name.EndsWith("Component") && !classSymbol.Name.EndsWith("PageComponent"))
                {
                    var diagnostic = Diagnostic.Create(
                        descriptor: SuffixRule,
                        location: classSymbol.Locations[0],
                        properties: ImmutableDictionary.CreateRange(
                            new[]
                            {
                                new KeyValuePair<string, string>("suffix", "Component"),
                                new KeyValuePair<string, string>("explicitSuffix", "PageComponent")
                            }),
                        messageArgs: classSymbol.Name);

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static void AnalyzeInterfaceImpls(SymbolAnalysisContext context)
        {
            var classSymbol = (INamedTypeSymbol)context.Symbol;
            var usingPageObjectLib = GetInterfaceTypeDefs(
                classSymbol,
                context,
                out var isPageObj,
                out var isPageCom);

            if (!usingPageObjectLib)
                return;

            if (isPageObj && isPageCom)
            {
                var diagnostic = Diagnostic.Create(
                ImplRule,
                classSymbol.Locations[0],
                classSymbol.Name);

                diagnostic.Properties.Add("suffix", "Component");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
