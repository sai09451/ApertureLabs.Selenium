using System;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.Attributes
{
    /// <summary>
    /// Used for in conjunction with the
    /// ApertureLabs.VisualStudio.GeneratePageObjects extention. Allows for the
    /// page object generation process to be customized. Should only be applied
    /// to classes that derive from <see cref="ILoadableComponent"/>. If
    /// defined on an interface then the <see cref="Implementation"/> property
    /// must be set to a class that has the
    /// <see cref="CodeGenerationAttribute"/> defined on it.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface,
        AllowMultiple = false,
        Inherited = false)]
    public class CodeGenerationAttribute : Attribute
    {
        private Action<object> codeGenerator;

        /// <summary>
        /// Gets or sets the HTML attribute value. Identifies all tag helpers
        /// that have the 'selenium-component-type' attribute helper and checks
        /// if that value matches this. If not specified the name of the class
        /// is used.
        /// </summary>
        /// <value>
        /// The HTML attribute value.
        /// </value>
        public string HTMLAttributeValue { get; set; }

        public Type Implementation { get; set; }

        /// <summary>
        /// Gets or sets the code generator.
        /// </summary>
        /// <value>
        /// The code generator.
        /// </value>
        public Action<object> CodeGenerator
        {
            get => codeGenerator ?? DefaultCodeGenerator;
            set => codeGenerator = value;
        }

        private void DefaultCodeGenerator(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
