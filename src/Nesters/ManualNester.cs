using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    static class ManualNester
    {
        private const string CordovaKind = "{262852C6-CD72-467D-83FE-5EEB1973A190}";
        public static void Nest(IEnumerable<ProjectItem> items)
        {
            ItemSelector selector = new ItemSelector(items);

            if (!selector.ShowDialog().Value)
            {
                return;
            }

            foreach (ProjectItem item in items)
            {
                string path = item.FileNames[0];
                ProjectItem parent = item.DTE.Solution.FindProjectItem(selector.SelectedFile);
                if (parent == null) continue;

                bool mayNeedAttributeSet = item.ContainingProject.Kind.Equals(CordovaKind, System.StringComparison.OrdinalIgnoreCase);
                if (mayNeedAttributeSet)
                {
                    SetDependentUpon(item, parent.Name);
                }
                else
                {
                    item.Remove();
                    parent.ProjectItems.AddFromFile(path);
                }
            }
        }

        public static void UnNest(ProjectItem item)
        {
            foreach (ProjectItem child in item.ProjectItems)
            {
                UnNest(child);
            }

            UnNestItem(item);
        }

        private static void UnNestItem(ProjectItem item)
        {
            string path = item.FileNames[0];
            object parent = item.Collection.Parent;

            bool shouldAddToParentItem = item.ContainingProject.Kind == CordovaKind;

            while (parent != null)
            {
                var pi = parent as ProjectItem;

                if (pi != null)
                {
                    if (!Path.HasExtension(pi.FileNames[0]))
                    {
                        object itemType = item.Properties.Item("ItemType").Value;

                        DeleteAndAdd(item, path);

                        ProjectItem newItem;
                        if (shouldAddToParentItem)
                        {
                            newItem = pi.ProjectItems.AddFromFile(path);
                        }
                        else
                        {
                            newItem = pi.ContainingProject.ProjectItems.AddFromFile(path);
                        }
                        newItem.Properties.Item("ItemType").Value = itemType;
                        break;
                    }

                    parent = pi.Collection.Parent;
                }
                else
                {
                    var pj = parent as Project;
                    if (pj != null)
                    {
                        object itemType = item.Properties.Item("ItemType").Value;

                        DeleteAndAdd(item, path);

                        ProjectItem newItem = pj.ProjectItems.AddFromFile(path);
                        newItem.Properties.Item("ItemType").Value = itemType;
                        break;
                    }
                }
            }
        }

        private static void DeleteAndAdd(ProjectItem item, string path)
        {
            if (!File.Exists(path))
                return;

            string temp = Path.GetTempFileName();
            File.Copy(path, temp, true);
            item.Delete();
            File.Copy(temp, path);
            File.Delete(temp);
        }

        private static void RemoveDependentUpon(ProjectItem item)
        {
            SetDependentUpon(item, null);
        }

        private static void SetDependentUpon(ProjectItem item, string value)
        {
            if (item.ContainsProperty("DependentUpon"))
            {
                item.Properties.Item("DependentUpon").Value = value;
            }
        }
    }
}
