using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace ApertureLabs.Selenium.UnitTests.Infrastructure
{
    public class Startup
    {
        private static Process ServerProcess;

        [AssemblyInitialize]
        public static void AssemblyInitalize(TestContext testContext)
        {
            // TODO: Check if tests inherit from ServerTest.
            var requiresServer = testContext.Properties
                .ContainsKey(ServerRequiredAttribute.AttributeName);

            var pathToMockServer = "";

            if (requiresServer)
            {
                ServerProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = "dotnet",
                        Arguments = "run",
                        UseShellExecute = false,
                        WorkingDirectory = pathToMockServer
                    }
                };
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup(TestContext testContext)
        {
            // Stop the server process if it was started.
            ServerProcess?.Dispose();
        }
    }
}
