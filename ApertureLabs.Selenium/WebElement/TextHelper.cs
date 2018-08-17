using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium.WebElement
{
    public class TextHelper
    {
        #region Fields

        private readonly WebElementV2 element;

        #endregion

        #region Constructor

        public TextHelper(WebElementV2 element)
        {
            this.element = element;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Retrieves the inner text of the element.
        /// </summary>
        public string InnerText => element.WebElement.Text;

        #endregion

        #region Methods

        /// <summary>
        /// Extracts a number from the Text of the element. If the text
        /// of the element is "Some text...-34.32...more text" it will
        /// return -34. It completely ignores the decimal unless the 
        /// optional parameter roundUp is true.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int ExtractInteger(bool roundUp = false)
        {
            var r = new Regex(@"^.*?((-?\d+)(.\d+)?)");
            var matches = r.Match(InnerText);

            var number = int.Parse(matches.Groups[2].ToString());

            return number;
        }

        #endregion
    }
}
