using OpenQA.Selenium;
using System;
using System.IO;

namespace ApertureLabs.Selenium.WebElements.Inputs
{
    /// <summary>
    /// Input element with the type set to file.
    /// </summary>
    public class FileElement : InputElement
    {
        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="element"></param>
        public FileElement(IWebElement element) : base(element)
        {
            if (Type != "file")
                throw new InvalidElementStateException("The type must be 'file'.");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Convience method for uploading a file.
        /// </summary>
        /// <param name="filepath"></param>
        public void UploadFile(string filepath)
        {
            SetValue(filepath);
        }

        /// <summary>
        /// Convience method for uploading a file.
        /// </summary>
        /// <param name="file"></param>
        public void UploadFile(FileInfo file)
        {
            SetValue(file.FullName);
        }

        /// <summary>
        /// Convience method for uploading a file.
        /// </summary>
        /// <param name="filePath"></param>
        public void UploadFile(Uri filePath)
        {
            SetValue(filePath.AbsolutePath);
        }

        #endregion
    }
}
