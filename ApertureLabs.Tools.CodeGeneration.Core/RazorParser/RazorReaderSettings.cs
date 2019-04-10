using System;

namespace ApertureLabs.Tools.CodeGeneration.Core.RazorParser
{
    public sealed class RazorReaderSettings
    {
        public RazorReaderSettings()
        {
            Async = false;
            CheckCharacters = false;
            CloseInput = true;
        }

        public bool Async { get; set; }
        public bool CheckCharacters { get; set; }
        public bool CloseInput { get; set; }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public RazorReaderSettings Clone()
        {
            throw new NotImplementedException();
        }
    }
}
