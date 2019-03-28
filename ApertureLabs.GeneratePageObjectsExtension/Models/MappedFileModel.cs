using EnvDTE;

namespace ApertureLabs.GeneratePageObjectsExtension.Models
{
    public class MappedFileModel
    {
        public bool IsNewFile { get; set; }
        public bool IsIgnored { get; set; }
        public string OriginalPath { get; set; }
        public string NewPath { get; set; }
        public bool IsPageComponent { get; set; }
        public ProjectItem ProjectItemReference { get; set; }
    }
}
