﻿namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models
{
    public class AvailableProjectModel
    {
        public bool IsNew { get; set; }
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string PathToProjectFolder { get; set; }
    }
}