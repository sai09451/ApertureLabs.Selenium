using System;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models
{
    public abstract class BaseTemplateModel
    {
        public Uri InputPath { get; set; }
        public Uri OutputPath { get; set; }
    }
}
