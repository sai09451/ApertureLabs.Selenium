using System;

namespace ApertureLabs.Selenium.Js
{
    /// <summary>
    /// Convience class for executing scripts in a setTimeout context.
    /// </summary>
    public class SetTimeout : JavaScript
    {
        #region Fields

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        public SetTimeout()
        {
            IsAsync = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Will always be true.
        /// </summary>
        public override bool IsAsync => true;

        /// <summary>
        /// The script to be executed.
        /// </summary>
        public override string Script
        {
            get => base.Script;
            set
            {
                var script =
                    $"setTimeout(" +
                        $"function(){{" +
                            $"{value}" +
                        $"}}," +
                        $"{Timeout.TotalMilliseconds})";

                // Cleanup the script.
                Clean(script);
                RemoveComments(script);

                base.Script = script;
            }
        }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        #endregion
    }
}
