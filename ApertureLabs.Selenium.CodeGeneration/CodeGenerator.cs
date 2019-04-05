using ApertureLabs.Selenium.CodeGeneration.Extensions;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.CodeGeneration
{
    internal class DemoCodeGenerator : ICodeGenerator
    {
        public void Generate(
            CodeGenerationContext originalContext,
            CodeGenerationContext newContext)
        {
            if (originalContext == null)
                throw new ArgumentNullException(nameof(originalContext));
            else if (newContext == null)
                throw new ArgumentNullException(nameof(newContext));

            // Only handle C# for now.
            if (newContext.FileCodeModel.Language != CodeModelLanguageConstants.vsCMLanguageCSharp)
                return;

            var newCodeModel = originalContext.FileCodeModel;

            // Get all namespace elements.
            var namespaceEls = newCodeModel
                .CodeElements
                .SearchForAll(el => el.Kind == vsCMElement.vsCMElementNamespace);

            // Create the namespace if needed.
            if (!namespaceEls.Any())
            {
                // Get the last using element.
                var usingEl = newCodeModel
                    .CodeElements
                    .Recurse()
                    .LastOrDefault(el => el.Kind == vsCMElement.vsCMElementUsingStmt);

                // Determine where to place the namespace. It should be placed
                // one line after the last using declaration.
                var namespaceStartPoint = usingEl
                    .GetEndPoint(vsCMPart.vsCMPartWholeWithAttributes)
                    .CreateEditPoint();

                namespaceStartPoint.LineDown();
                newCodeModel.AddNamespace(
                    originalContext.NewNamespace,
                    namespaceStartPoint);
            }

            var classes = newCodeModel.CodeElements
                .SearchForAll(el => el.Kind == vsCMElement.vsCMElementClass)
                .ToList();

            if (!classes.Any())
            {
                // No classes already exist.
            }
            else
            {
                // Classes do exist.
                var generatingType = classes.LastOrDefault(
                    el => el.Name.Equals(
                        originalContext.GeneratedTypeName));

                if (generatingType != null)
                {
                    // Need to create class.
                }
                else
                {
                    // Need to update class.
                }
            }

            if (newCodeModel.CodeElements.Count > 0)
            {
                foreach (CodeElement codeElement in newCodeModel.CodeElements)
                {
                    if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
                    {
                        // Set the starting point to the last point.
                        //startingCodeElement = codeElement
                        //    .Children
                        //    .Item(codeElement.Children.Count - 1);
                        break;
                    }
                }
            }

            switch (originalContext.GeneratedType)
            {
                case vsCMElement.vsCMElementClass:
                    newCodeModel.AddClass(
                        Name: originalContext.GeneratedTypeName,
                        Position: null,
                        Bases: null,
                        ImplementedInterfaces: null,
                        Access: vsCMAccess.vsCMAccessPublic);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public IEnumerable<CodeChange> GetChanges(CodeGenerationContext originalContext, CodeGenerationContext newContext)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CodeGenerationContext> GetContexts(Project project)
        {
            throw new NotImplementedException();
        }

        private CodeElement GetClass(FileCodeModel fileCodeModel, string className)
        {
            var classElement = default(CodeElement);

            foreach (CodeElement codeElement in fileCodeModel.CodeElements)
            {
                if (codeElement.Children.Count > 0)
                {
                    classElement = GetClass(codeElement, className);

                    if (classElement != null)
                        break;
                }
            }

            return classElement;
        }

        private CodeElement GetClass(CodeElement codeElement, string className)
        {
            var classElement = default(CodeElement);

            foreach (CodeElement childCodeElement in codeElement.Children)
            {
                if (childCodeElement.Children.Count > 0)
                {
                    return GetClass(childCodeElement, className);
                }
            }

            return classElement;
        }
    }
}
