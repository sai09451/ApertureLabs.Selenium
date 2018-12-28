using System.Collections.Generic;

namespace MockServer.Models
{
    public class FrameworkModel
    {
        #region Constructor

        public FrameworkModel()
        {
            VersionPathMap = new Dictionary<string, IEnumerable<string>>();
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        /// <summary>
        /// [ version str ] / [ web relative path to views ].
        /// </summary>
        public IDictionary<string, IEnumerable<string>> VersionPathMap { get; set; }

        #endregion

        #region Methods

        public ICollection<string> GetVersions()
        {
            return VersionPathMap.Keys;
        }

        #endregion
    }
}
