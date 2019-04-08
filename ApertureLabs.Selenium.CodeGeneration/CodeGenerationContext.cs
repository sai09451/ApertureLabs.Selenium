using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.CodeGeneration
{
    /// <summary>
    /// Contains the file being used to generate the code and the file being
    /// generated.
    /// </summary>
    public class CodeGenerationContext
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CodeGenerationContext"/> class.
        /// </summary>
        public CodeGenerationContext()
        {
            Metadata = new Dictionary<string, object>();
        }

        /// <summary>
        /// The id of the original document that isn't being modified. Used to
        /// generate the <see cref="DestinationDocumentId"/>.
        /// </summary>
        public DocumentId OriginalDocumentId { get; set; }

        /// <summary>
        /// The id of the document being generated.
        /// </summary>
        public DocumentId DestinationDocumentId { get; set; }

        /// <summary>
        /// Gets or sets the metadata associated with this context.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public IDictionary<string, object> Metadata { get; }
    }
}
