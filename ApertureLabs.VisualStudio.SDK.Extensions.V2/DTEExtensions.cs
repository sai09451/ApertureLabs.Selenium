using EnvDTE;
using System;
using System.Collections.Generic;

namespace ApertureLabs.VisualStudio.SDK.Extensions.V2
{
    public static class DTEExtensions
    {
        public static IEnumerable<string> GetSelectedItemPaths(
            this EnvDTE80.DTE2 dte)
        {
            if (dte == null)
                throw new ArgumentNullException(nameof(dte));

            var items = (Array)dte.ToolWindows.SolutionExplorer.SelectedItems;
            foreach (UIHierarchyItem selItem in items)
            {
                var item = selItem.Object as ProjectItem;

                if (item != null && item.Properties != null)
                    yield return item.Properties.Item("FullPath").Value.ToString();
            }
        }
    }
}
