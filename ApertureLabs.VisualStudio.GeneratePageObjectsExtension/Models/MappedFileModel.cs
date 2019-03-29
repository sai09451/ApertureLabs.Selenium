using EnvDTE;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models
{
    public class MappedFileModel
    {
        public bool IsNewFile { get; set; }
        public bool IsIgnored { get; set; }
        public string OriginalPath { get; set; }
        public string NewPath { get; set; }
        public string InheritFrom { get; set; }
        public ProjectItem ProjectItemReference { get; set; }
    }
}
