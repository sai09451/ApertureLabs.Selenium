using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            var block = new Block
            {
                Before = string.Format("function {0} ({1})",
                    name,
                    String.Join(",", arguments)),
                Content = content
            };

            return block;
        }

        /// <summary>
        /// Creates a for loop js block.
        /// </summary>
        /// <param name="initialExpression"></param>
        /// <param name="condition"></param>
        /// <param name="incrementExpression"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Block ForLoop(string initialExpression,
            string condition,
            string incrementExpression,
            JavaScript content)
        {
            var block = new Block
            {
                Before = string.Format("for ({0}; {1}; {2})",
                    initialExpression,
                    condition,
                    incrementExpression),

                Script = condition
            };

            return block;
        }

        private static void Test()
        {
            var value = Expression.Parameter(typeof(int), "value");
            var result = Expression.Parameter(typeof(int), "result");
            var label = Expression.Label(typeof(int));

            var expression = Expression.Block(
                new[] { result },
                Expression.Assign(result, Expression.Constant(1)),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.GreaterThan(value, Expression.Constant(1)),
                        Expression.MultiplyAssign(result,
                            Expression.PostDecrementAssign(value)),
                        Expression.Break(label, result)),
                    label
                )
            );

            var func = Expression.Lambda<Func<int,int>>(expression, value)
                .Compile();

            var funcResult = func(5);
        }

        #endregion
    }
}
