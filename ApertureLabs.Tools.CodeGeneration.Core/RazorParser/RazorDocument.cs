using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApertureLabs.Tools.CodeGeneration.Core.RazorParser
{
    public class RazorDocument : RazorContainer
    {
        #region Fields

        #endregion

        #region Constructor

        public RazorDocument()
        {
            throw new NotImplementedException();
        }

        public RazorDocument(params object[] content)
        {
            throw new NotImplementedException();
        }

        public RazorDocument(RazorDocument document)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        public string FilePath { get; protected set; }

        public string Layout { get; protected set; }

        public RazorNode RootNode { get; protected set; }

        #endregion

        #region Methods

        public static RazorDocument Load(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static RazorDocument Load(TextReader reader)
        {
            throw new NotImplementedException();
        }

        public static RazorDocument Load(RazorReader reader)
        {
            throw new NotImplementedException();
        }

        public static RazorDocument Parse(string content)
        {
            throw new NotImplementedException();
        }

        public void Save(RazorWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Save(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Save(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Save(TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void WriteTo(RazorWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
