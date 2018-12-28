using Microsoft.AspNetCore.Hosting;
using MockServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MockServer.Factories
{
    /// <summary>
    /// Used for creating/populating FrameworkModels.
    /// </summary>
    public class FrameworkModelFactory : IFrameworkModelFactory
    {
        #region Fields

        private readonly IHostingEnvironment hostingEnvironment;

        #endregion

        #region Constructor

        public FrameworkModelFactory(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        #endregion

        #region Methods

        public IList<FrameworkModel> PrepareFrameworkModels()
        {
            var frameworks = new List<FrameworkModel>();
            var rootPath = hostingEnvironment.ContentRootPath;

            var dirs = Directory.EnumerateDirectories(Path.Combine(rootPath, "Pages"))
                .Where(d => !String.Equals("Shared", new DirectoryInfo(d).Name))
                .ToList();

            // Retrieve all frameworks.
            foreach (var frameworkDir in dirs)
            {
                var framework = new FrameworkModel
                {
                    Name = new DirectoryInfo(frameworkDir).Name
                };

                // Retrieve all versions.
                foreach (var versionPath in Directory.EnumerateDirectories(frameworkDir))
                {
                    var versionName = new DirectoryInfo(versionPath).Name;
                    var paths = new List<string>();

                    // Retrieve all files in that version.
                    foreach (var filePath in Directory.EnumerateFiles(versionPath, "*.cshtml"))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(filePath);

                        // Ignore all files that start with an '_'.
                        if (fileName.StartsWith('_'))
                            continue;

                        var webLink = $"/{framework.Name}/{versionName}/{fileName}";

                        if (paths.Contains(webLink))
                            continue;

                        paths.Add(webLink);
                    }

                    framework.VersionPathMap[versionName] = paths;
                    frameworks.Add(framework);
                }
            }

            return frameworks;
        }

        #endregion
    }
}
