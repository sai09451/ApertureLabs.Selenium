using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApertureLabs.GeneratePageObjectsExtension.Models
{
    public abstract class BaseTemplateModel
    {
        public Uri InputPath { get; set; }
        public Uri OutputPath { get; set; }
    }
}
