using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;

namespace MadsKristensen.FileNesting
{
    class Helpers
    {
        public static IEnumerable<ProjectItem> GetSelectedItems()
        {
            var items = (Array)FileNestingPackage.DTE.ToolWindows.SolutionExplorer.SelectedItems;
            foreach (UIHierarchyItem selItem in items)
            {
                var item = selItem.Object as ProjectItem;
                if (item != null)
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
