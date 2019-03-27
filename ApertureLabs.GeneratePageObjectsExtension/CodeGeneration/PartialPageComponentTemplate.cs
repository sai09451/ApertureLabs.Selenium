using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApertureLabs.GeneratePageObjectsExtension.CodeGeneration
{
    public partial class PageComponentTemplate
    {
        private readonly object model;

        public PageComponentTemplate(object model)
        {
            this.model = model
                ?? throw new ArgumentNullException(nameof(model));
        }
    }
}
