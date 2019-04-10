using System;

namespace ApertureLabs.Tools.CodeGeneration.Core.RazorParser
{
    public class RazorObjectChangeEventArgs : EventArgs
    {
        #region Fields

        public static readonly RazorObjectChangeEventArgs Add
            = new RazorObjectChangeEventArgs(RazorObjectChange.Add);
        public static readonly RazorObjectChangeEventArgs Remove
            = new RazorObjectChangeEventArgs(RazorObjectChange.Remove);
        public static readonly RazorObjectChangeEventArgs Name
            = new RazorObjectChangeEventArgs(RazorObjectChange.Name);
        public static readonly RazorObjectChangeEventArgs Value
            = new RazorObjectChangeEventArgs(RazorObjectChange.Value);

        #endregion

        #region Constructor

        public RazorObjectChangeEventArgs(RazorObjectChange change)
        {
            ObjectChange = change;
        }

        #endregion

        #region Properties

        public RazorObjectChange ObjectChange { get; private set; }

        #endregion
    }
}
