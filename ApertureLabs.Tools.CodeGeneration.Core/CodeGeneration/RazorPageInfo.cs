using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ApertureLabs.Tools.CodeGeneration.Core.CodeGeneration
{
    /// <summary>
    /// Contains general info on a razor page.
    /// </summary>
    public class RazorPageInfo
    {
        public RazorPageInfo()
        {
            IncludedPartialPages = new List<RazorPageInfo>();
            IncludedViewComponents = new List<RazorPageInfo>();
            IncludedProperties = new List<string>();
        }

        public string Namespace { get; set; }

        public string Name => Path.GetFileNameWithoutExtension(RelativePath);

        public string GeneratedClassName => IsViewComponent
            ? $"{Name}PageComponent"
            : $"{Name}PageObject";

        public string GeneratedFullClassName => $"{Namespace}.{GeneratedClassName}";

        public string GeneratedFullInterfaceName => $"{Namespace}.{GeneratedInterfaceName}";

        public string GeneratedInterfaceName => $"I{GeneratedClassName}";

        public string PhysicalPath { get; set; }

        public string RelativePath { get; set; }

        public RazorPageInfo Layout { get; set; }

        public bool IsViewComponent { get; set; }

        public IList<RazorPageInfo> IncludedPartialPages { get; }

        public IList<RazorPageInfo> IncludedViewComponents { get; }

        public IList<string> IncludedProperties { get; }

        public static RazorPageInfo UnkownPageInfo()
        {
            var pageInfo = new RazorPageInfo();

            return pageInfo;
        }
    }

    public class RazorComponent
    {
        public bool Multiple { get; set; }
    }
}
