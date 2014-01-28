using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio;

namespace MadsKristensen.FileNesting
{
    class Helpers
    {
        public static IEnumerable<ProjectItem> GetSelectedItems()
        {
            var items = (Array)FileNestingPackage.DTE.ToolWindows.SolutionExplorer.SelectedItems;
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
                if (item.Kind == VSConstants.ItemTypeGuid.PhysicalFolder_string)
                {
                    var files = Directory.EnumerateFiles(item.FileNames[0], "*", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        ProjectItem child = FileNestingPackage.DTE.Solution.FindProjectItem(file);

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

        public static Project GetSelectedProject()
        {
            var items = (Array)FileNestingPackage.DTE.ToolWindows.SolutionExplorer.SelectedItems;
            foreach (UIHierarchyItem selItem in items)
            {
                var item = selItem.Object as Project;
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
