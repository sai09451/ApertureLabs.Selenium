using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.WebElements.Table
{
    /// <summary>
    /// Represents a table element.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.WebElements.BaseElement" />
    public class TableElement : BaseElement
    {
        #region Fields

        private readonly By captionSelector = By.CssSelector("caption");
        private readonly By cellsSelector = By.CssSelector("tr > th, tr > td");
        private readonly By rowsSelector = By.CssSelector("tr");

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TableElement"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <exception cref="UnexpectedTagNameException">table</exception>
        /// <exception cref="NotImplementedException">
        /// Thrown if there are nested tables.
        /// </exception>
        public TableElement(IWebElement element)
            : base(element)
        {
            var isTable = String.Equals(
                element.TagName,
                "table",
                StringComparison.OrdinalIgnoreCase);

            // Validate the tagname is correct.
            if (!isTable)
            {
                throw new UnexpectedTagNameException("table", element.TagName);
            }

            // Validate there are no nested tables.
            if (element.FindElements(By.CssSelector("table")).Any())
            {
                throw new NotImplementedException("Nested tables not " +
                    "supported.");
            }
        }

        #endregion

        #region Properties

        private IReadOnlyCollection<IWebElement> CellElements => WrappedElement
            .FindElements(cellsSelector);

        private IReadOnlyCollection<IWebElement> RowElements => WrappedElement
            .FindElements(rowsSelector);

        #endregion

        #region Methods

        /// <summary>
        /// Gets the caption. Returns null if there isn't a caption.
        /// </summary>
        /// <returns></returns>
        public virtual IWebElement GetCaption()
        {
            return WrappedElement
                .FindElements(captionSelector)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the table body rows.
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyCollection<IWebElement> GetTableBodyRows()
        {
            return WrappedElement.FindElements(By.CssSelector("tbody > tr"));
        }

        /// <summary>
        /// Gets the table header rows.
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyCollection<IWebElement> GetTableHeaderRows()
        {
            return WrappedElement.FindElements(By.CssSelector("thead > tr"));
        }

        /// <summary>
        /// Gets the table footer rows.
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyCollection<IWebElement> GetTableFooterRows()
        {
            return WrappedElement.FindElements(By.CssSelector("tfoot > tr"));
        }

        /// <summary>
        /// Gets all rows.
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyCollection<IWebElement> GetAllRows()
        {
            return WrappedElement.FindElements(By.CssSelector("tr"));
        }

        /// <summary>
        /// Gets the cell. The row number is zero-based and should be based off
        /// of ALL rows. Same with the col number.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        public virtual IWebElement GetCell(int row, int col)
        {
            return RowElements.ElementAt(row)
                .FindElements(By.CssSelector("td,th"))
                .ElementAt(col);
        }

        #endregion
    }
}
