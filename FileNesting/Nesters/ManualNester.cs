using System.Collections.Generic;
using System.IO;
using EnvDTE;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.FileNesting
{
    static class ManualNester
    {
        private const string CordovaKind = "{262852C6-CD72-467D-83FE-5EEB1973A190}";
        public static void Nest(IEnumerable<ProjectItem> items)
        {
            ItemSelector selector = new ItemSelector(items);

            if (!selector.ShowDialog().Value)
                return;

            foreach (ProjectItem item in items)
            {
                string path = item.FileNames[0];
                ProjectItem parent = item.DTE.Solution.FindProjectItem(selector.SelectedFile);
                if (parent == null) continue;

                bool mayNeedAttributeSet = item.ContainingProject.Kind == CordovaKind;

                if (mayNeedAttributeSet) DeleteAndAdd(item, path);
                ProjectItem newItem = parent.ProjectItems.AddFromFile(path);
                if (mayNeedAttributeSet)
                {
                    //  It is possible this may not be required in future cordova project versions
                    //   so do a test first
                    var pitems = parent.ProjectItems
                                       .Cast<ProjectItem>();
                    if (!pitems.Any(x => x.Name == path))
                    {
                        SetDependentUponAttribute(newItem, parent);
                    }
                }
            }
        }

        private static void SetDependentUponAttribute(ProjectItem item, ProjectItem parent)
        {
            //  See: https://msdn.microsoft.com/en-us/library/bb458043.aspx "How to: Add an Attribute to a Project Item"
            Project project = item.ContainingProject;
            
            string uniqueName = project.UniqueName;
            var solution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));
            IVsHierarchy hierarchy;
            solution.GetProjectOfUniqueName(uniqueName, out hierarchy);
            var buildPropertyStorage = hierarchy as IVsBuildPropertyStorage;
            if (buildPropertyStorage != null)
            {
                uint itemId;
                string fullPath = (string)item.Properties.Item("FullPath").Value;
                hierarchy.ParseCanonicalName(fullPath, out itemId);
                buildPropertyStorage.SetItemAttribute(itemId, "DependentUpon", parent.Name);
            }
            project.Save();
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
            if (!File.Exists(path))
                return;

            string temp = Path.GetTempFileName();
            File.Copy(path, temp, true);
            item.Delete();
            File.Copy(temp, path);
            File.Delete(temp);
        }
    }
}
