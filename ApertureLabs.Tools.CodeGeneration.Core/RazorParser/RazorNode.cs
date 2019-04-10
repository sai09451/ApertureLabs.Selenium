using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ApertureLabs.Tools.CodeGeneration.Core.RazorParser
{
    public abstract class RazorNode : RazorObject
    {
        #region Properties

        public RazorNode NextNode { get; protected set; }

        public RazorNode PreviousNode { get; protected set; }

        #endregion

        #region Methods

        public static int CompareDocumentOrder(RazorNode n1, RazorNode n2)
        {
            if (n1 == null)
                throw new ArgumentNullException(nameof(n1));
            else if (n2 == null)
                throw new ArgumentNullException(nameof(n2));

            var lineInfoN1 = n1 as IXmlLineInfo;
            var lineInfoN2 = n2 as IXmlLineInfo;

            if (lineInfoN1 == null || lineInfoN2 == null)
                throw new NotImplementedException();

            if (!lineInfoN1.HasLineInfo() || !lineInfoN2.HasLineInfo())
                throw new NotImplementedException();

            if (lineInfoN1.LineNumber < lineInfoN2.LineNumber)
            {
                return -1;
            }
            else if (lineInfoN1.LineNumber == lineInfoN2.LineNumber)
            {
                return lineInfoN1.LinePosition.CompareTo(lineInfoN2.LinePosition);
            }
            else
            {
                return 1;
            }
        }

        public static bool DeepEquals(RazorNode n1, RazorNode n2)
        {
            // If both are null
            if (n1 == null && n2 == null)
                return true;

            // If only one of them is null return false.
            if ((n1 == null && n2 != null) || (n1 != null && n2 == null))
                return false;

            // Compare node types.
            if (n1.NodeType != n2.NodeType)
                return false;

            if (n1.NodeType == RazorNodeType.Element)
            {
                // TODO
                // Compare tag name.
                // Compare same attibutes with values.
                // Ignore comments and processing instructions.
                // Compare length sequences and content nodes.
                throw new NotImplementedException();
            }
            else if (n1.NodeType == RazorNodeType.Document)
            {
                // TODO: Compare root nodes.
                throw new NotImplementedException();
            }
            else if (n1.NodeType == RazorNodeType.Comment)
            {
                // TODO: Compare same comment text.
                throw new NotImplementedException();
            }
            else if (n1.NodeType == RazorNodeType.ProcessingInstruction)
            {
                // TODO: Compare same target + data.
                throw new NotImplementedException();
            }
            else if (n1.NodeType == RazorNodeType.DocumentType)
            {
                // TODO: Compare name, public id, system id, and internal subset.
                throw new NotImplementedException();
            }
            else
            {
                // Shouldn't reach this far.
                throw new NotImplementedException();
            }
        }

        public static RazorNode ReadFrom(RazorReader reader)
        {
            throw new NotImplementedException();
        }

        public virtual void AddAfterSelf(object content)
        {
            throw new NotImplementedException();
        }

        public virtual void AddAfterSelf(params object[] content)
        {
            throw new NotImplementedException();
        }

        public virtual void AddBeforeSelf(object content)
        {
            throw new NotImplementedException();
        }

        public virtual void AddBeforeSelf(params object[] content)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<RazorElement> Ancestors()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<RazorElement> Ancestors(string name)
        {
            throw new NotImplementedException();
        }

        public virtual RazorReader CreateReader()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<RazorElement> ElementsBeforeSelf()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<RazorElement> ElementsBeforeSelf(string name)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<RazorElement> ElementsAfterSelf()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<RazorElement> ElementsAfterSelf(string name)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsAfter(RazorNode node)
        {
            return 1 == CompareDocumentOrder(this, node);
        }

        public virtual bool IsBefore(RazorNode node)
        {
            return -1 == CompareDocumentOrder(this, node);
        }

        public virtual IEnumerable<RazorNode> NodesAfterSelf()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<RazorNode> NodesBeforeSelf()
        {
            throw new NotImplementedException();
        }

        public virtual void Remove()
        {
            throw new NotImplementedException();
        }

        public virtual void ReplaceWith(object content)
        {
            throw new NotImplementedException();
        }

        public virtual void ReplaceWith(params object[] content)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public virtual void WriteTo(RazorWriter writer)
        {
            throw new NotImplementedException();
        }

        public virtual Task WriteToAsync(
            RazorWriter writer,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
