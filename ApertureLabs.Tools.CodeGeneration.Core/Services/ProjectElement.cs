using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ApertureLabs.Tools.CodeGeneration.Core.Services.CsProjectUtilities
{
    public class ProjectElement
    {
        #region Fields

        private readonly XDocument projectDocument;
        private readonly string filePath;

        #endregion

        #region Constructor

        public ProjectElement(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            filePath = project.FilePath;
            projectDocument = XDocument.Load(filePath);
        }

        #endregion

        #region Methods

        public ProjectElement SetProperty(string propertyName, string propertyValue)
        {
            var propertyGroupsEls = projectDocument.Elements("PropertyGroup");

            foreach (var propertyGroupEl in propertyGroupsEls)
            {
                var matchingEl = propertyGroupEl.Element(propertyName);

                if (matchingEl == null)
                    continue;

                matchingEl.SetValue(propertyValue);

                break;
            }

            return this;
        }

        public ProjectElement RemoveProperty(string propertyName)
        {
            var propertyGroupEls = projectDocument.Elements("PropertyGroup");

            foreach (var propertyGroupEl in propertyGroupEls)
            {
                var matchingEl = propertyGroupEl.Element(propertyName);

                if (matchingEl == null)
                    continue;

                matchingEl.Remove();

                break;
            }

            return this;
        }

        public string GetPropertyValue(string propertyName)
        {
            var propertyGroupsEls = projectDocument.Elements("PropertyGroup");

            foreach (var propertyGroupEl in propertyGroupsEls)
            {
                var matchingEl = propertyGroupEl.Element(propertyName);

                if (matchingEl == null)
                    continue;

                return matchingEl.Value;
            }

            return null;
        }

        public ProjectElement AddPackageReference(string packageId, string version)
        {
            var itemGroupEls = projectDocument.Elements("ItemGroup");

            var newEl = false;
            var itemGroupEl = itemGroupEls.FirstOrDefault(
                e => e.Elements().Any() && e.Elements().All(
                    _e => _e.Name.Equals("PackageReference")));

            if (itemGroupEl == null)
            {
                // No package references added to the project yet, need to
                // create the element.
                newEl = true;
                itemGroupEl = new XElement(XName.Get("ItemGroup"));
                projectDocument.Add(itemGroupEl);
            }

            if (!newEl)
            {
                // Check if the package exists.
                var exists = itemGroupEl.Elements("PackageReference")
                    .FirstOrDefault(
                        e => e.Attribute("Include")?.Value.Equals(
                            packageId,
                            StringComparison.Ordinal) ?? false);

                if (exists != null)
                {
                    // Set the version and return early.
                    exists.SetValue(version);
                    return this;
                }
            }

            itemGroupEl.Add(new XElement(
                XName.Get("PackageReference"),
                new XAttribute("Include", packageId),
                new XAttribute("Version", version)));

            return this;
        }

        public ProjectElement RemovePackageReference(string packageId)
        {
            var element = projectDocument.Elements("PackageReference")
                .FirstOrDefault(
                    e => e.Attribute("Include")?.Value.Equals(
                        packageId, StringComparison.Ordinal) ?? false);

            element?.Remove();

            return this;
        }

        public IEnumerable<(string, string)> GetPackageReferences()
        {
            var packageReferenceEls = projectDocument.Elements("PackageReference");

            foreach (var packageRefEl in packageReferenceEls)
            {
                var packageId = packageRefEl.Attribute("Include").Value;
                var version = packageRefEl.Attribute("Version").Value;

                yield return (packageId, version);
            }
        }

        public void Save()
        {
            projectDocument.Save(filePath);
        }

        #endregion
    }
}
