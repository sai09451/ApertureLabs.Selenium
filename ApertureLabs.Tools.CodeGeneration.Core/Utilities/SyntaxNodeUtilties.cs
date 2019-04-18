using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace ApertureLabs.Tools.CodeGeneration.Core.Utilities
{
    public static class SyntaxNodeUtilties
    {
        public static T GetFirstOrDefaultNode<T>(this SyntaxNode syntaxNode,
            Func<T, string> getIdentifierFunc,
            string identifier)
            where T : SyntaxNode
        {
            return syntaxNode.DescendantNodes()
                .OfType<T>()
                .FirstOrDefault(
                    n => getIdentifierFunc(n).Equals(
                        identifier,
                        StringComparison.Ordinal));
        }

        public static TNodeType GetFirstOrDefaultNode<TNodeType, TNodeIdentifier>(
            this SyntaxNode syntaxNode,
            Func<TNodeType, TNodeIdentifier> getIdentifierFunc,
            TNodeIdentifier identifier)
            where TNodeType : SyntaxNode
        {
            return syntaxNode.DescendantNodes()
                .OfType<TNodeType>()
                .FirstOrDefault(n => getIdentifierFunc(n).Equals(identifier));
        }

        public static NamespaceDeclarationSyntax GetNamespace(
            this SyntaxNode node,
            string @namespace)
        {
            return node.DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault(n => n.Name.ToString().Equals(
                    @namespace,
                    StringComparison.Ordinal));
        }

        public static InterfaceDeclarationSyntax GetInterface(
            this SyntaxNode node,
            string interfaceName,
            string @namespace)
        {
            return node.GetNamespace(@namespace)
                ?.GetFirstOrDefaultNode<InterfaceDeclarationSyntax>(
                    n => n.Identifier.ToString(),
                    interfaceName);
        }

        public static ClassDeclarationSyntax GetClass(
            this SyntaxNode node,
            string className,
            string @namespace)
        {
            return node.GetNamespace(@namespace)
                ?.GetFirstOrDefaultNode<ClassDeclarationSyntax>(
                    n => n.Identifier.ToString(),
                    className);
        }

        public static PropertyDeclarationSyntax GetProperty(this SyntaxNode node,
            string propertyName,
            string className,
            string @namespace)
        {
            return GetClass(node, className, @namespace)
                ?.GetFirstOrDefaultNode<PropertyDeclarationSyntax>(
                    n => n.Identifier.ToString(),
                    propertyName);
        }
    }
}
