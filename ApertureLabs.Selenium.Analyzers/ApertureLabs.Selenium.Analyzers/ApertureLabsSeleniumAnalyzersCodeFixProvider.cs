using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace ApertureLabs.Selenium.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ApertureLabsSeleniumAnalyzersCodeFixProvider)), Shared]
    public class ApertureLabsSeleniumAnalyzersCodeFixProvider : CodeFixProvider
    {
        private const string virtualTitle = "Make member virtual";
        private const string suffixTitle = "Append suffix to class name";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    ApertureLabsSeleniumAnalyzersAnalyzer.DiagnosticIdVirtualRule,
                    ApertureLabsSeleniumAnalyzersAnalyzer.DiagnosticIdSuffixRule);
            }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            switch (diagnostic.Id)
            {
                case ApertureLabsSeleniumAnalyzersAnalyzer.DiagnosticIdSuffixRule:
                    RegisterSuffixCodeFix(context, root, diagnostic, diagnosticSpan);
                    break;
                case ApertureLabsSeleniumAnalyzersAnalyzer.DiagnosticIdVirtualRule:
                    RegisterVirtualCodeFix(context, root, diagnostic, diagnosticSpan);
                    break;
            }
        }

        private void RegisterVirtualCodeFix(CodeFixContext context,
            SyntaxNode root,
            Diagnostic diagnostic,
            TextSpan diagnosticSpan)
        {
            // Find the method or property declaration identified by the
            // diagnostic.
            MemberDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault();

            if (declaration == null)
            {
                declaration = root.FindToken(diagnosticSpan.Start)
                    .Parent
                    .AncestorsAndSelf()
                    .OfType<PropertyDeclarationSyntax>()
                    .FirstOrDefault();
            }

            if (declaration == null)
                return;

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: virtualTitle,
                    createChangedDocument: c => MakeVirtualAsync(
                        context.Document,
                        declaration,
                        c),
                    equivalenceKey: virtualTitle),
                diagnostic);
        }

        private void RegisterSuffixCodeFix(CodeFixContext context,
            SyntaxNode root,
            Diagnostic diagnostic,
            TextSpan diagnosticSpan)
        {
            var declaration = root.FindToken(diagnosticSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<TypeDeclarationSyntax>()
                .FirstOrDefault();

            if (declaration == null)
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: suffixTitle,
                    createChangedSolution: c => AppendSuffixAsync(
                        diagnostic,
                        context.Document,
                        declaration,
                        c),
                    equivalenceKey: suffixTitle),
                diagnostic);
        }

        private async Task<Solution> AppendSuffixAsync(Diagnostic diagnostic,
            Document document,
            TypeDeclarationSyntax typeDecl,
            CancellationToken cancellationToken)
        {
            // Compute new name.
            var identifierToken = typeDecl.Identifier;
            var newName = diagnostic.Properties.ContainsKey("suffix")
                ? identifierToken.Text + diagnostic.Properties["suffix"]
                : identifierToken.Text + "Component";

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

            // Produce a new solution that has all references to that type
            // renamed, including type declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(
                    document.Project.Solution,
                    typeSymbol,
                    newName,
                    optionSet,
                    cancellationToken)
                .ConfigureAwait(false);

            return newSolution;
        }

        private async Task<Document> MakeVirtualAsync(Document document,
            MemberDeclarationSyntax typeDecl,
            CancellationToken cancellationToken)
        {
            var root = await document
                .GetSyntaxRootAsync(cancellationToken)
                .ConfigureAwait(false);

            var newModifiers = default(SyntaxTokenList);
            var newMemberDecl = default(MemberDeclarationSyntax);

            switch (typeDecl)
            {
                case MethodDeclarationSyntax methodDecl:
                    newModifiers = SyntaxFactory.TokenList(
                        methodDecl.Modifiers.Concat(
                            new[] { SyntaxFactory.Token(SyntaxKind.VirtualKeyword) }));
                    newMemberDecl = methodDecl.WithModifiers(newModifiers);
                    break;
                case PropertyDeclarationSyntax propertyDecl:
                    newModifiers = SyntaxFactory.TokenList(
                        propertyDecl.Modifiers.Concat(
                            new[] { SyntaxFactory.Token(SyntaxKind.VirtualKeyword) }));
                    newMemberDecl = propertyDecl.WithModifiers(newModifiers);
                    break;
            }

            var newRoot = root.ReplaceNode(typeDecl, newMemberDecl);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}
