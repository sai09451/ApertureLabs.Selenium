using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Tools.CodeGeneration.Core.RazorParser
{
    public class RazorContainer : RazorNode
    {
        #region Properties

        public RazorNode FirstNode { get; protected set; }

        public RazorNode LastNode { get; protected set; }

        #endregion

        #region Methods

        public void Add(object content)
        {
            throw new NotImplementedException();
        }

        public void Add(params object[] content)
        {
            throw new NotImplementedException();
        }

        public void AddFirst(object content)
        {
            throw new NotImplementedException();
        }

        public void AddFirst(params object[] content)
        {
            throw new NotImplementedException();
        }

        public RazorWriter CreateWriter()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RazorNode> DescendantNodes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RazorElement> Descendants()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RazorElement> Descendants(string name)
        {
            throw new NotImplementedException();
        }

        public RazorElement Element(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RazorElement> Elements()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RazorElement> Elements(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RazorNode> Nodes()
        {
            throw new NotImplementedException();
        }

        public void RemoveNodes()
        {
            throw new NotImplementedException();
        }

        public void ReplaceNodes(object content)
        {
            throw new NotImplementedException();
        }

        public void ReplaceNodes(params object[] content)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
