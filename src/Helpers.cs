using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio;

namespace MadsKristensen.FileNesting
{
    static class Helpers
    {
        public static IEnumerable<ProjectItem> GetSelectedItems()
        {
            var items = (Array)VSPackage.DTE.ToolWindows.SolutionExplorer.SelectedItems;
            foreach (UIHierarchyItem selItem in items)
            {
                var project = selItem.Object as Project;
                if (project != null)
                {
                    foreach (ProjectItem item in project.ProjectItems)
                        yield return item;
                }
                else
                {
                    var item = selItem.Object as ProjectItem;
                    if (item != null)
                    {
                        yield return item;
                    }
                }
            }
        }

        public static IEnumerable<ProjectItem> GetSelectedItemsRecursive()
        {
            foreach (ProjectItem item in GetSelectedItems())
            {
                Guid kind;

                if (!Guid.TryParse(item.Kind, out kind))
                    continue;

                if (kind == VSConstants.ItemTypeGuid.PhysicalFolder_guid)
                {
                    var files = Directory.EnumerateFiles(item.FileNames[0], "*", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        ProjectItem child = VSPackage.DTE.Solution.FindProjectItem(file);

                        if (child != null)
                            yield return child;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }

        public static bool ContainsProperty(this ProjectItem projectItem, string propertyName)
        {
          if (projectItem.Properties != null)
          {
            foreach (Property item in projectItem.Properties)
            {
              if (item != null && item.Name == propertyName)
              {
                return true;
              }
            }
          }

          return false;
        }
    }
}
