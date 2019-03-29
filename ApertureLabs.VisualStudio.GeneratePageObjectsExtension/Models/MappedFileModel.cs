using EnvDTE;
using System.Collections.Generic;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models
{
    public class MappedFileModel
    {
        public bool IsNewFile { get; set; }
        public bool IsIgnored { get; set; }
        public string OriginalPathRelativeToProject { get; set; }
        public string FileName { get; set; }
        public string NewPath { get; set; }
        public int SelectedInheritFromIndex { get; set; }
        public ProjectItem ProjectItemReference { get; set; }
        public IReadOnlyList<string> AvailableComponentTypeNames { get; set; }
    }
}
