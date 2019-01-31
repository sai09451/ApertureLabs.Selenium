using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Base class which adds protected methods for using the
    /// selenium-server-standalone{x}.{y}.{z}.jar.
    /// </summary>
    public class SeleniumServerStandAloneHelper
    {
        #region Fields

        private readonly Uri onlineVersionList;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumServerStandAloneHelper"/> class.
        /// </summary>
        public SeleniumServerStandAloneHelper()
        {
            onlineVersionList = new Uri("http://selenium-release.storage.googleapis.com/");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves all online versions of the standalone jar file. They're
        /// ordered the newest version as first and the oldest as last.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(int x, int y, int z)> GetOnlineVersions()
        {
            using (var client = new WebClient())
            {
                var xmlStr = client.DownloadString(onlineVersionList);
                var xml = XDocument.Parse(xmlStr);
                var comparer = new KeyComparer(this);

                var results = xml.Root.Elements(XName.Get(
                        "Contents",
                        "http://doc.s3.amazonaws.com/2006-03-01"))
                    .Where(element =>
                    {
                        var value = element.Element(
                                XName.Get(
                                    "Key",
                                    "http://doc.s3.amazonaws.com/2006-03-01"))
                            .Value;

                        var isStandalone = value.Contains(
                            "selenium-server-standalone");

                        var isBeta = value.Contains("beta");

                        return isStandalone && !isBeta;
                    })
                    .Select(element =>
                    {
                        var value = element.Element(
                                XName.Get(
                                    "Key",
                                    "http://doc.s3.amazonaws.com/2006-03-01"))
                            .Value;

                        var version = GetXYZVerion(value);

                        return version;
                    })
                    .OrderByDescending(version => version, comparer)
                    .ToList();

                return results;
            }
        }

        /// <summary>
        /// Retrieves all local versions of the standalone jar file.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(int x, int y, int z)> GetLocalVersions()
        {
            var workingDirectory = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(
                workingDirectory,
                "selenium-server-standalone-*.jar");

            var comparer = new KeyComparer(this);

            var localVersions = files
                .Select(f => GetXYZVerion(f))
                .OrderByDescending(f => f, comparer);

            return localVersions;
        }

        /// <summary>
        /// Installs the version of the standalone jar file.
        /// </summary>
        /// <param name="version">
        /// The version to install. If null then the latest version will be
        /// installed.
        /// </param>
        public void InstallVersion((int x, int y, int z)? version = null)
        {
            var _version = version ?? GetOnlineVersions().First();

            var downloadUrl = new Uri(
                onlineVersionList,
                String.Format(
                    "{0}.{1}/selenium-server-standalone-{0}.{1}.{2}.jar",
                    _version.x,
                    _version.y,
                    _version.z));

            using (var client = new WebClient())
            {
                client.DownloadFile(
                    downloadUrl,
                    VersionToName(_version));
            }
        }

        /// <summary>
        /// Gets the latest local standalone version.
        /// </summary>
        /// <returns></returns>
        public (int x, int y, int z)? GetLatestLocalStandalone()
        {
            var workingDirectory = Directory.GetCurrentDirectory();
            var comparer = new KeyComparer(this);

            var files = Directory.GetFiles(
                workingDirectory,
                "selenium-server-standalone-*.jar");

            var latestVersion = files
                .Select(f => GetXYZVerion(f))
                .OrderByDescending(file => file, comparer)
                .FirstOrDefault();

            return latestVersion;
        }

        /// <summary>
        /// Determines whether there are any standalone jar files available
        /// locally.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has local standalone jar file]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasLocalStandaloneJarFile()
        {
            var workingDirectory = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(
                workingDirectory,
                "selenium-server-standalone-*.jar");

            return files.Any();
        }

        private string VersionToName((int x, int y, int z) version)
        {
            return $"selenium-server-standalone-{version.x}.{version.y}.{version.z}.jar";
        }

        private (int X, int Y, int Z) GetXYZVerion(string str)
        {
            // Ignore beta releasese.
            var versionPart = Regex.Match(
                str,
                @"selenium-server-standalone-(?<x>.*?)\.(?<y>.*?)\.(?<z>.*?)\.jar");

            var xStr = versionPart.Groups["x"].Value;
            var yStr = versionPart.Groups["y"].Value;
            var zStr = versionPart.Groups["z"].Value;

            var x = int.Parse(xStr);
            var y = int.Parse(yStr);
            var z = int.Parse(zStr);

            return (x, y, z);
        }

        #endregion

        #region Nested Classes

        private class KeyComparer : IComparer<(int X, int Y, int Z)>,
            IComparer<string>
        {
            private readonly SeleniumServerStandAloneHelper parent;

            public KeyComparer(SeleniumServerStandAloneHelper parent)
            {
                this.parent = parent;
            }

            public int Compare((int X, int Y, int Z) x, (int X, int Y, int Z) y)
            {
                if (x.X == y.X && x.Y == y.Y && x.Z == y.Z)
                    return 0;

                if (x.X != y.X)
                    return x.X > y.X ? 1 : -1;
                else if (x.Y != y.Y)
                    return x.Y > y.Y ? 1 : -1;
                else if (x.Z != y.Z)
                    return x.Z > y.Z ? 1 : -1;

                return 0;
            }

            public int Compare(string x, string y)
            {
                if (x == y)
                    return 0;

                var xVersion = parent.GetXYZVerion(x);
                var yVersion = parent.GetXYZVerion(y);

                return Compare(xVersion, yVersion);
            }
        }

        #endregion
    }
}
