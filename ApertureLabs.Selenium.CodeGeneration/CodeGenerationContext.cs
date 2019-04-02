using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ApertureLabs.Selenium.CodeGeneration
{
    /// <summary>
    /// Contains information about the original file and what's being
    /// generated.
    /// </summary>
    public class CodeGenerationContext
    {
        /// <summary>
        /// Gets or sets the original file information.
        /// </summary>
        /// <value>
        /// The original file information.
        /// </value>
        public FileInfo OriginalFileInfo { get; set; }

        /// <summary>
        /// Creates new fileinfo.
        /// </summary>
        /// <value>
        /// The new file information.
        /// </value>
        public FileInfo NewFileInfo { get; set; }

        /// <summary>
        /// Gets the new namespace.
        /// </summary>
        /// <value>
        /// The new namespace.
        /// </value>
        public string NewNamespace { get; set; }

        /// <summary>
        /// Gets or sets the original file code model.
        /// </summary>
        /// <value>
        /// The original file code model.
        /// </value>
        public FileCodeModel OriginalFileCodeModel { get; set; }

        /// <summary>
        /// Gets or sets the new file code model.
        /// </summary>
        /// <value>
        /// The new file code model.
        /// </value>
        public FileCodeModel NewFileCodeModel { get; set; }

        /// <summary>
        /// Gets or sets the name of the generated type.
        /// </summary>
        /// <value>
        /// The name of the generated type.
        /// </value>
        public string GeneratedTypeName { get; set; }

        /// <summary>
        /// Gets or sets the type being generated.
        /// </summary>
        /// <value>
        /// The type of the generated.
        /// </value>
        public GeneratedType GeneratedType { get; set; }

        /// <summary>
        /// Gets or sets the base class. Can be null.
        /// </summary>
        /// <value>
        /// The base class.
        /// </value>
        public Type BaseClass { get; set; }

        /// <summary>
        /// Gets or sets the interfaces to be implemented.
        /// </summary>
        /// <value>
        /// The implemented interfaces.
        /// </value>
        public IReadOnlyList<ImplementInterfaceContext> ImplementedInterfaces { get; set; }
    }
}
