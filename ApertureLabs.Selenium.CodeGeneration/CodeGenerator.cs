using ApertureLabs.Selenium.CodeGeneration.Extensions;
using EnvDTE;
using System;
using System.Linq;

namespace ApertureLabs.Selenium.CodeGeneration
{
    public abstract class CodeGenerator
    {
        public abstract void Generate(CodeGenerationContext codeGenerationContext);
    }

    internal class DemoCodeGenerator : CodeGenerator
    {
        public override void Generate(CodeGenerationContext codeGenerationContext)
        {
            if (codeGenerationContext == null)
                throw new NotImplementedException();

            if (codeGenerationContext.NewFileCodeModel.Language != CodeModelLanguageConstants.vsCMLanguageCSharp)
                return;

            var newCodeModel = codeGenerationContext.NewFileCodeModel;
            object startingCodeElement = -1;

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
                    codeGenerationContext.NewNamespace,
                    namespaceStartPoint);
            }

            if (newCodeModel.CodeElements.Count > 0)
            {
                foreach (CodeElement codeElement in newCodeModel.CodeElements)
                {
                    if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
                    {
                        // Set the starting point to the last point.
                        startingCodeElement = codeElement
                            .Children
                            .Item(codeElement.Children.Count - 1);
                        break;
                    }
                }
            }

            switch (codeGenerationContext.GeneratedType)
            {
                case GeneratedType.Class:
                    newCodeModel.AddClass(
                        Name: codeGenerationContext.GeneratedTypeName,
                        Position: null,
                        Bases: null,
                        ImplementedInterfaces: null,
                        Access: vsCMAccess.vsCMAccessPublic);
                    break;
                default:
                    throw new NotImplementedException();
            }
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
