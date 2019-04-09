using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;

namespace ApertureLabs.Tools.CodeGeneration.Core.RazorParser
{
    public class RazorSemanticModel
    {
        #region Properties

        public string Kind { get; private set; }

        #endregion
    }
}
