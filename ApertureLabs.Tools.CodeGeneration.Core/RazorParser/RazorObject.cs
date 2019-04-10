using System;
using System.Collections.Generic;
using System.Xml;

namespace ApertureLabs.Tools.CodeGeneration.Core.RazorParser
{
    public class RazorObject : IXmlLineInfo
    {
        #region Events

        public EventHandler<RazorObjectChangeEventArgs> Changed;
        public EventHandler<RazorObjectChangeEventArgs> Changing;

        #endregion

        #region Properties

        public int LineNumber { get; protected set; }

        public int LinePosition { get; protected set; }

        public RazorDocument Document { get; protected set; }

        public RazorNodeType NodeType { get; protected set; }

        public RazorElement Parent { get; protected set; }

        #endregion

        #region Methods

        public virtual void AddAnnotation(object annotation)
        {
            throw new NotImplementedException();
        }

        public virtual T Annotation<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public virtual object Annotation(Type type)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<object> Annotations(Type type)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> Annotations<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveAnnotation(Type type)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveAnnotation<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public virtual bool HasLineInfo()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
