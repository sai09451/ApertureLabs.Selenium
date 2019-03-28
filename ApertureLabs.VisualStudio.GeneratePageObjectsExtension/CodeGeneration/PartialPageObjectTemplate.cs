using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.CodeGeneration
{
    public partial class PageObjectTemplate
    {
        private readonly object model;

        public PageObjectTemplate(object model)
        {
            this.model = model
                ?? throw new ArgumentNullException(nameof(model));
        }
    }
}
