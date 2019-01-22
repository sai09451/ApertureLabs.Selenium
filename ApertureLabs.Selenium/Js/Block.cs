using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Selenium.Js
{
    /// <summary>
    /// Surrounds a bit of js with brackets.
    /// </summary>
    public class Block : JavaScript
    {
        #region Constructor

        internal Block()
        {
            Before = String.Empty;
            Content = new List<JavaScript>();
            After = String.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        /// What's placed before the start of the brackets.
        /// </summary>
        public string Before { get; set; }

        /// <summary>
        /// Will be put after the brackets.
        /// </summary>
        public string After { get; set; }

        /// <summary>
        /// Placed inside the brackets.
        /// </summary>
        public IEnumerable<JavaScript> Content { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Before);
            sb.Append("{");

            foreach (var s in Content)
                sb.Append(s);

            sb.Append("}");
            sb.Append(After);

            return sb.ToString();
        }

        /// <summary>
        /// Creates a function js block.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Block Function(string name,
            IEnumerable<string> arguments,
            IEnumerable<JavaScript> content)
        {
            var block = new Block();

            block.Before = string.Format("function {0} ({1})",
                name,
                String.Join(",", arguments));

            block.Content = content;

            return block;
        }

        #endregion
    }
}
