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
                Guid kind;

                if (!Guid.TryParse(item.Kind, out kind))
                    continue;

                if (kind == VSConstants.ItemTypeGuid.PhysicalFolder_guid)
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

        public static bool IsKind(this Project project, params string[] kindGuids)
        {
            foreach (var guid in kindGuids)
            {
                if (project.Kind.Equals(guid, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }

    public static class ProjectTypes
    {
        public const string ASPNET_5 = "{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}";
        public const string DOTNET_Core = "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}";
        public const string WEBSITE_PROJECT = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
        public const string UNIVERSAL_APP = "{262852C6-CD72-467D-83FE-5EEB1973A190}";
        public const string NODE_JS = "{9092AA53-FB77-4645-B42D-1CCCA6BD08BD}";
        public const string SSDT = "{00d1a9c2-b5f0-4af3-8072-f6c62b433612}";
        public const string SHARED_PROJECT = "{D954291E-2A0B-460D-934E-DC6B0785DB48}";
        public const string NETSTANDARD = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
    }
}
