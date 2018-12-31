using System.Diagnostics;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApertureLabs.Selenium.UnitTests.Infrastructure
{
    [TestClass]
    public class Startup
    {
        private static Process ServerProcess;
        public static string ServerUrl = "http://localhost:5000";

        [AssemblyInitialize]
        public static void AssemblyInitalize(TestContext testContext)
        {
            var requiresServer = testContext.Properties
                .ContainsKey(ServerRequiredAttribute.AttributeName);

            if (requiresServer)
            {
                var pathToMockServer = "../../../../MockServer";
                ServerProcess = new Process
                {
                    StartInfo =
                        {
                            FileName = "dotnet",
                            Arguments = "run",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            WorkingDirectory = pathToMockServer
                        }
                };

                // Start the process.
                var started = ServerProcess.Start();
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // Stop the server process if it was started.
            ServerProcess?.Close();
            ServerProcess?.Dispose();
        }
    }
}
