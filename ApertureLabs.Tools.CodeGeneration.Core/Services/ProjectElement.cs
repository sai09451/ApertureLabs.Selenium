using Microsoft.CodeAnalysis;
using System;
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

        public void SetProperty(string propertyName, string propertyValue)
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
        }

        public void RemoveProperty(string propertyName)
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
        }

        public void AddPackageReference(string packageId, string version)
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
                    // Set the version.
                    exists.SetValue(version);
                    return;
                }
            }

            itemGroupEl.Add(new XElement(
                XName.Get("PackageReference"),
                new XAttribute("Include", packageId),
                new XAttribute("Version", version)));
        }

        public void RemovePackageReference(string packageId)
        {
            var element = projectDocument.Elements("PackageReference")
                .FirstOrDefault(
                    e => e.Attribute("Include")?.Value.Equals(
                        packageId, StringComparison.Ordinal) ?? false);

            element?.Remove();
        }

        public void Save()
        {
            projectDocument.Save(filePath);
        }

        #endregion
    }
}
