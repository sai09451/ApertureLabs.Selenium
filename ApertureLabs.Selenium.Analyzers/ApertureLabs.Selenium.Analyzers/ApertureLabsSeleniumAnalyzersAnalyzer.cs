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
        public const string DiagnosticId = "ApertureLabsSeleniumAnalyzers";

        // You can change these strings in the Resources.resx file. If you do
        // not want your analyzer to be localize-able, you can use regular
        // strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md
        // for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitle),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.AnalyzerMessageFormat),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.AnalyzerDescription),
            Resources.ResourceManager,
            typeof(Resources));
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        private const string Category = "Usage";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(Rule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax
            // instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md
            // for more information
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var classSymbol = (INamedTypeSymbol)context.Symbol;

            //var pageObjectInterfaceType = context.Compilation
            //    .GetTypeByMetadataName("ApertureLabs.PageObjects.IPageObject");

            //if (pageObjectInterfaceType == null)
            //    return;

            //var isPageObject = classSymbol
            //    .AllInterfaces.Any(i => i.Equals(pageObjectInterfaceType));

            var isPageObject = classSymbol
                .AllInterfaces.Any(i => i.Name == "IPageObject");

            if (!isPageObject)
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

            foreach (var methodSymbol in methodSymbols)
            {
                var ignore = methodSymbol.IsVirtual
                    || methodSymbol.IsOverride
                    || methodSymbol.IsSealed
                    || methodSymbol.IsStatic;

                if (ignore)
                    continue;

                var diagnostic = Diagnostic.Create(
                    Rule,
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
                    Rule,
                    propertySymbol.Locations[0],
                    propertySymbol.Name);
            }
        }
    }
}
