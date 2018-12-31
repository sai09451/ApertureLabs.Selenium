using System;
using System.Diagnostics;
using System.Threading;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]
namespace ApertureLabs.Selenium.UnitTests.Infrastructure
{
    [TestClass]
    public class Startup
    {
        private static ManualResetEvent Signal = new ManualResetEvent(false);
        private static Process ServerProcess;
        public static string ServerUrl = "http://localhost:5000";

        [AssemblyInitialize]
        public static void AssemblyInitalize(TestContext testContext)
        {
            var requiresServer = testContext.Properties
                .ContainsKey(ServerRequiredAttribute.AttributeName);

            if (requiresServer)
            {
                var pathToMockServerRootFolder = "../../../../MockServer";

                // Build the server.
                var buildProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = "dotnet",
                        Arguments = "build",
                        UseShellExecute = false,
                        WorkingDirectory = pathToMockServerRootFolder
                    }
                };

                buildProcess.Start();
                buildProcess.WaitForExit();

                // Run the server.
                ServerProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = "dotnet",
                        Arguments = "bin/Debug/netcoreapp2.1/MockServer.dll",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = pathToMockServerRootFolder
                    }
                };

                // Start the process.
                ServerProcess.Start();

                ServerProcess.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data?.StartsWith("Now listening on: ") ?? false)
                    {
                        ServerUrl = e.Data.Substring(18);
                        Signal.Set();
                    }

                    Console.WriteLine(e.Data);
                    testContext.WriteLine("Server process output: " + e.Data);
                };

                ServerProcess.ErrorDataReceived += (sender, e) =>
                {
                    Console.WriteLine(e.Data);
                    testContext.WriteLine("Server process error: " + e.Data);
                };

                ServerProcess.BeginOutputReadLine();
                ServerProcess.BeginErrorReadLine();

                testContext.WriteLine($"Started process: " +
                    $"'{ServerProcess.ProcessName}' with an id of " +
                    $"${ServerProcess.Id}.");

                Signal.WaitOne();
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // Stop the server process if it was started.
            ServerProcess?.Kill();
            ServerProcess?.WaitForExit();
            ServerProcess?.Dispose();
        }
    }
}
