using Microsoft.AspNetCore.Razor.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ApertureLabs.Tools.CodeGeneration.Core.CodeGeneration
{
    public class RazorPageInfoFactory
    {
        #region Fields

        private readonly string razorProjectDirectory;
        private readonly RazorProjectEngine razorProjectEngine;

        #endregion

        #region Constructor

        public RazorPageInfoFactory(string razorProjectDirectory)
        {
            this.razorProjectDirectory = razorProjectDirectory;

            razorProjectEngine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(razorProjectDirectory));
        }

        #endregion

        #region Methods

        public IEnumerable<RazorPageInfo> GenerateRazorPages()
        {
            var results = new List<RazorPageInfo>();

            foreach (var file in EnumerateRazorFiles())
            {
                var razorInfo = new RazorPageInfo
                {
                    FullPathOfRazorFile = file.CombinedPath
                };

                results.Add(razorInfo);
            }

            var text = File.ReadAllText(razorProjectDirectory).AsSpan();

            return results;
        }

        private IEnumerable<RazorProjectItem> EnumerateRazorFiles()
        {
            throw new NotImplementedException();
        }

        private void GetClosingTag(Span<string> fileText, int startPos)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
