using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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

        public string Name => IsUnknownPage
            ? null
            : Path.GetFileNameWithoutExtension(RelativePath);

        public string GeneratedClassName => IsViewComponent
            ? $"{Name}PageComponent"
            : $"{Name}PageObject";

        public string GeneratedFullClassName
        {
            get
            {
                if (IsUnknownPage)
                    return null;

                return String.IsNullOrEmpty(Namespace)
                    ? GeneratedClassName
                    : $"{Namespace}.{GeneratedClassName}";
            }
        }

        public string GeneratedFullInterfaceName
        {
            get
            {
                if (IsUnknownPage)
                    return null;

                return String.IsNullOrEmpty(Namespace)
                    ? GeneratedInterfaceName
                    : $"{Namespace}.{GeneratedInterfaceName}";
            }
        }

        public string GeneratedInterfaceName
        {
            get
            {
                if (IsUnknownPage)
                    return null;

                return Regex.Replace(
                    GeneratedClassName,
                    "^([^a-zA-Z]*?)([a-zA-Z])",
                    m => $"{m.Groups[1].Value}I{m.Groups[2].Value}");
            }
        }

        public string PhysicalPath { get; set; }

        public string RelativePath { get; set; }

        public RazorPageInfo Layout { get; set; }

        public bool IsViewComponent { get; set; }

        public bool IsUnknownPage { get; private set; }

        public IList<RazorPageInfo> IncludedPartialPages { get; }

        public IList<RazorPageInfo> IncludedViewComponents { get; }

        public IList<string> IncludedProperties { get; }

        public static RazorPageInfo UnkownPageInfo()
        {
            var pageInfo = new RazorPageInfo
            {
                IsUnknownPage = true
            };

            return pageInfo;
        }
    }

    public class RazorComponent
    {
        public bool Multiple { get; set; }
    }
}
