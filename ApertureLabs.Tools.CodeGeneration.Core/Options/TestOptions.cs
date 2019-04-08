using ApertureLabs.Selenium.CodeGeneration;
using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Tools.CodeGeneration.Core.Options
{
    [Verb(
        name: "test",
        HelpText = "For debugging purposes only.")]
    public class TestOptions : LogOptions, ICommand
    {
        private string originalProjectOutputPath;

        public async Task ExecuteAsync(
            IProgress<double> progress,
            CancellationToken cancellationToken)
        {
            await Program.InitializeAsync(this).ConfigureAwait(false);

            Program.Log.Info("Test");
            Program.Log.Debug("Test");
            Program.Log.Warning("Test");
            Program.Log.Error("Test");

            var (workspace, solution) = await Program
                .GetWorkspaceAndSolution("C:/Users/Alexander/Documents/GitHub/ApertureLabs.Selenium/ApertureLabs.Selenium.sln")
                .ConfigureAwait(false);

            var originalProject = solution.Projects.FirstOrDefault(
                p => p.Name.Equals(
                    "MockServer",
                    StringComparison.OrdinalIgnoreCase));

            var destinationProject = solution.Projects.FirstOrDefault(
                p => p.Name.Equals(
                    "MockServer.PageObjects",
                    StringComparison.OrdinalIgnoreCase));

            #region Compiling

            // Get the compiliation of the original project.
            var originalCompiliation = default(Compilation);
            var destinationCompiliation = default(Compilation);

            try
            {
                originalCompiliation = await originalProject.GetCompilationAsync(cancellationToken)
                    .ConfigureAwait(false);
                destinationCompiliation = await originalProject.GetCompilationAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Program.Log.Error("Failed to compile project " + originalProject.Name);
                Program.Log.Error("Failed to compile project " + destinationProject.Name);
                Program.Log.Error(e, true);
            }

            Program.Log.Info($"Successfully compiled {originalProject.Name}");
            Program.Log.Info($"Successfully compiled {destinationProject.Name}");

            #endregion

            #region Project info.

            originalProjectOutputPath = originalProject.OutputFilePath;

            Program.Log.Info("Original project info:");
            LogInfoOf(() => originalProject.OutputFilePath);
            LogInfoOf(() => originalProject.SupportsCompilation);

            #endregion

            #region Adding a new project.

            // Try and create a new project.
            //Program.Log.Info("Attempting to create a new project.");
            //var newProjectInfo = ProjectInfo.Create(
            //    ProjectId.CreateNewId(),
            //    VersionStamp.Default,
            //    "MockServer.Tests",
            //    "MockServer.Tests",
            //    LanguageNames.CSharp);

            //var invalidChangesSolution = solution.AddProject(newProjectInfo);
            //LogSupportedChanges(workspace);
            //LogSolutionChanges(invalidChangesSolution, solution);

            //// Try and apply changes.
            //try
            //{
            //    if (!workspace.TryApplyChanges(invalidChangesSolution))
            //        throw new Exception("Failed to modify the solution.");

            //    Program.Log.Info("Successfully added a project.");
            //    solution = invalidChangesSolution;
            //}
            //catch (Exception e)
            //{
            //    Program.Log.Error(e, false);
            //}

            #endregion

            #region Adding a document.

            // Commented this out because I know it works.
            // Try and add a document.
            //var addedDocument = destinationProject.AddDocument(
            //    name: "TestingAddDoc.txt",
            //    text: SourceText.From(
            //        String.Join(Environment.NewLine, new string[]
            //        {
            //            "using System;",
            //            "",
            //            "namespace MockServer.PageObjects",
            //            "{",
            //            "   internal class GeneratedClass",
            //            "   {",
            //            "       public string SomeProperty { get; set; }",
            //            "   }",
            //            "}",
            //        }),
            //        Encoding.UTF8));

            //LogSolutionChanges(addedDocument.Project.Solution, solution);

            //try
            //{
            //    if (!workspace.TryApplyChanges(addedDocument.Project.Solution))
            //        throw new Exception("Failed to add the document");

            //    Program.Log.Info("Successfully added the document.");
            //    solution = addedDocument.Project.Solution;
            //    destinationProject = addedDocument.Project;
            //}
            //catch (Exception e)
            //{
            //    Program.Log.Error(e, false);
            //}

            #endregion

            #region Removing a document.

            // Commented this out because it works.
            // Try and remove the document.
            //var addedDocument = destinationProject.Documents.FirstOrDefault(
            //    d => d.Name.Equals(
            //        "TestingAddDoc.cs",
            //        StringComparison.Ordinal));

            //if (addedDocument == null)
            //    throw new FileNotFoundException();

            //var modifiedSolution = destinationProject.RemoveDocument(addedDocument.Id);

            //LogSolutionChanges(destinationProject.Solution, solution);
            //Program.Log.Info("Attempting to remove the added document.");

            //try
            //{
            //    if (!workspace.TryApplyChanges(destinationProject.Solution))
            //        throw new Exception("Failed to remove the document");

            //    // Need to manually remove the file.
            //    File.Delete(addedDocument.FilePath);

            //    Program.Log.Info("Successfully removed the document.");
            //    solution = destinationProject.Solution;
            //}
            //catch (Exception e)
            //{
            //    Program.Log.Error(e, false);
            //}

            #endregion

            #region Loading the compiled assembly.

            var cancellationTokenSource = new CancellationTokenSource();
            var _progress = new Progress<CodeGenerationProgress>();
            var loadContext = new PluginLoadContext(destinationProject.OutputFilePath);
            var assembly = loadContext.LoadFromAssemblyPath(destinationProject.OutputFilePath);

            var codeGeneratorTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.Name == "ICodeGenerator")
                    && !t.IsAbstract
                    && t.IsClass
                    && t.GetConstructors().Any( // Check for public default ctor.
                        c => c.IsPublic
                        && !c.GetParameters().Any()));

            if (!codeGeneratorTypes.Any())
                Program.Log.Error("Failed to locate any ICodeGenerators.", true);

            var destProjRef = destinationProject;

            foreach (var codeGeneratorType in codeGeneratorTypes)
            {
                var codeGenerator = Activator.CreateInstance(codeGeneratorType)
                    as ICodeGenerator;

                if (codeGenerator == null)
                    continue;

                var contexts = await codeGenerator.GetContexts(
                        originalProject,
                        destinationProject,
                        cancellationTokenSource.Token)
                    .ConfigureAwait(false);

                // Generate the code.
                foreach (var context in contexts)
                {
                    var originalDocument = originalProject.GetDocument(context.OriginalDocumentId);
                    var destinationDocument = destProjRef.GetDocument(context.DestinationDocumentId);

                    var modifiedDoc = await codeGenerator.Generate(
                            originalDocument,
                            destinationDocument,
                            context.Metadata,
                            _progress,
                            cancellationTokenSource.Token)
                        .ConfigureAwait(false);

                    destProjRef = modifiedDoc.Project;
                }

                // Get list of changes.
                var changes = destProjRef.Solution.GetChanges(destinationProject.Solution);
                LogSolutionChanges(destProjRef.Solution, destinationProject.Solution);

                // Prompt for accepting changes.
                Console.Write("Accept changes y/n");
                var response = Console.ReadLine();

                var isPositiveResponse = response.Equals("y", StringComparison.OrdinalIgnoreCase)
                    || response.Equals("yes", StringComparison.OrdinalIgnoreCase);

                if (!isPositiveResponse)
                {
                    // Exit if not applying the changes.
                    Program.Log.Error("Exiting program");
                    return;
                }

                // Apply changes.
                if (workspace.TryApplyChanges(destProjRef.Solution))
                {
                    // Success.
                    Program.Log.Info("Applied changes successfully");
                }
                else
                {
                    // Error.
                    Program.Log.Error("Failed to apply changes.");
                }
            }

            // TODO: Unload assembly.

            #endregion
        }

        private Assembly CurrentDomain_AssemblyResolve(
            object sender,
            ResolveEventArgs args)
        {
            // Check the *.deps.json file for libraries matching that name.

            // Check for assembly in built project location.
            var files = new FileInfo(originalProjectOutputPath).Directory.GetFiles();

            foreach (var file in files)
            {
                var name = default(string);

                try
                {
                    name = AssemblyName
                        .GetAssemblyName(file.FullName)
                        .ToString();
                }
                catch (BadImageFormatException)
                {
                    continue;
                }

                if (args.Name.Equals(name, StringComparison.Ordinal))
                {
                    return Assembly.LoadFrom(file.FullName);
                }
            }

            throw new FileNotFoundException(args.Name);
        }

        private static void LogInfoOf<T>(Expression<Func<T>> expression)
        {
            var result = expression.Compile().Invoke();

            switch (expression.Body)
            {
                case MethodCallExpression methodCall:
                    Program.Log.Info($"{methodCall.Method.Name}: {result.ToString()}");
                    break;
                case MemberExpression member:
                    Program.Log.Info($"{member.Member.Name}: {result.ToString()}");
                    break;
            }
        }

        private static void LogChanges<T>(
            Expression<Func<IEnumerable<T>>> changes,
            Func<T, string> formatter = null)
        {
            var results = changes.Compile().Invoke();

            string name;
            switch (changes.Body)
            {
                case MethodCallExpression methodCallExpression:
                    name = methodCallExpression.Method.Name;
                    break;
                default:
                    name = String.Empty;
                    break;
            }

            if (formatter == null)
                formatter = (T t) => t.ToString();

            foreach (var change in results)
            {
                Program.Log.Info($"\t* {name}: {formatter(change)}");
            }
        }

        private static void LogSupportedChanges(MSBuildWorkspace workspace)
        {
            Program.Log.Info("Supported changes in the workspace:");

            foreach (var changeKind in Enum.GetNames(typeof(ApplyChangesKind)))
            {
                var canChangeKind = workspace.CanApplyChange(
                    (ApplyChangesKind)Enum.Parse(
                        typeof(ApplyChangesKind),
                        changeKind));

                Program.Log.Info($"\t{changeKind}: {canChangeKind}");
            }

            Program.Log.Info($"\t{nameof(workspace.CanOpenDocuments)}: {workspace.CanOpenDocuments}");
        }

        private static void LogSolutionChanges(
            Solution modifiedSolution,
            Solution originalSolution)
        {
            var changes = modifiedSolution.GetChanges(originalSolution);

            foreach (var addedProject in changes.GetAddedProjects())
            {
                Program.Log.Info($"\tAdded project {addedProject.Name}");
            }

            foreach (var changedProject in changes.GetProjectChanges())
            {
                Program.Log.Info($"\tChanged project {changedProject.OldProject.Name} -> {changedProject.NewProject.Name}");

                LogChanges(() => changedProject.GetAddedAdditionalDocuments());
                LogChanges(() => changedProject.GetAddedAnalyzerReferences());
                LogChanges(() => changedProject.GetAddedDocuments());
                LogChanges(() => changedProject.GetAddedMetadataReferences());
                LogChanges(() => changedProject.GetAddedProjectReferences());
                LogChanges(() => changedProject.GetChangedAdditionalDocuments());
                LogChanges(() => changedProject.GetChangedDocuments());
                LogChanges(() => changedProject.GetRemovedAdditionalDocuments());
                LogChanges(() => changedProject.GetRemovedAnalyzerReferences());
                LogChanges(() => changedProject.GetRemovedDocuments());
                LogChanges(() => changedProject.GetRemovedMetadataReferences());
                LogChanges(() => changedProject.GetRemovedProjectReferences());
            }

            foreach (var removedProject in changes.GetRemovedProjects())
            {
                Program.Log.Info($"\tRemoved project {removedProject.Name}");
            }
        }
    }
}
