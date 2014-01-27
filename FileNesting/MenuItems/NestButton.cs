using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using EnvDTE;
using System.Linq;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.IO;

namespace MadsKristensen.FileNesting
{
    class NestButton
    {
        private static IEnumerable<ProjectItem> _items;

        public static void Register(DTE2 dte, OleMenuCommandService mcs)
        {
            CommandID nestId = new CommandID(GuidList.guidFileNestingCmdSet, (int)PkgCmdIDList.cmdNest);
            OleMenuCommand menuNest = new OleMenuCommand(Nest, nestId);
            mcs.AddCommand(menuNest);
            menuNest.BeforeQueryStatus += BeforeNest;
        }

        private static void BeforeNest(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            _items = Helpers.GetSelectedItems().Where(itm => !Directory.Exists(itm.FileNames[0]));
            button.Enabled = _items.Any();
        }

        private static void Nest(object sender, EventArgs e)
        {
            var items = Helpers.GetSelectedItems();
            ManualNester.Nest(items);
        }
    }
}