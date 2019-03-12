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
        private const string title = "PageObject methods should be virtual";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ApertureLabsSeleniumAnalyzersAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis,
            // generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

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
                    .First();

            }

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => MakeVirtualAsync(
                        context.Document,
                        declaration,
                        c),
                    equivalenceKey: title),
                diagnostic);
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
