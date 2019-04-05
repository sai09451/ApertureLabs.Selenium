using EnvDTE;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.CodeGeneration
{
    public interface ICodeGenerator
    {
        IEnumerable<CodeGenerationContext> GetContexts(Project project);

        IEnumerable<CodeChange> GetChanges(
            CodeGenerationContext originalContext,
            CodeGenerationContext newContext);

        void Generate(
            CodeGenerationContext originalContext,
            CodeGenerationContext newContext);
    }
}
