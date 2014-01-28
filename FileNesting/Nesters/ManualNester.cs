using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    static class ManualNester
    {
        public static void Nest(IEnumerable<ProjectItem> items)
        {
            Dialog.ItemSelector selector = new Dialog.ItemSelector(items);

            if (!selector.ShowDialog().Value)
                return;

            foreach (ProjectItem item in items)
            {
                string path = item.Properties.Item("FullPath").Value.ToString();
                ProjectItem parent = item.DTE.Solution.FindProjectItem(selector.SelectedFile);

                if (parent != null)
                    parent.ProjectItems.AddFromFile(path);
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
            string path = item.Properties.Item("FullPath").Value.ToString();
            object parent = item.Collection.Parent;

            while (parent != null)
            {
                var pi = parent as ProjectItem;

                if (pi != null)
                {
                    if (!Path.HasExtension(pi.FileNames[0]))
                    {
                        object itemType = item.Properties.Item("ItemType").Value;

                        DeleteAndAdd(item, path);

                        ProjectItem newItem = pi.ContainingProject.ProjectItems.AddFromFile(path);
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
            string temp = Path.GetTempFileName();
            File.Copy(path, temp, true);
            item.Delete();
            File.Copy(temp, path);
            File.Delete(temp);
        }
    }
}
