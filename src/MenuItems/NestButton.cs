using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.FileNesting
{
    static class NestButton
    {
        private static IEnumerable<ProjectItem> _items;

        public static void Register(MenuCommandService mcs)
        {
            CommandID nestId = new CommandID(PackageGuids.guidFileNestingCmdSet, PackageIds.cmdNest);
            OleMenuCommand menuNest = new OleMenuCommand(Nest, nestId);
            mcs.AddCommand(menuNest);
            menuNest.BeforeQueryStatus += BeforeNest;
        }

        private static void BeforeNest(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            _items = Helpers.GetSelectedItems().Where(i => (i.Kind.Equals(VSConstants.ItemTypeGuid.PhysicalFile_string, StringComparison.OrdinalIgnoreCase) && i.ProjectItems != null));
            button.Enabled = _items.Any();
        }

        private static void Nest(object sender, EventArgs e)
        {
            ManualNester.Nest(_items);
        }
    }
}