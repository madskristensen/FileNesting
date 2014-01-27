using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.FileNesting
{
    class RunAutoNestingButton
    {
        public static void Register(DTE2 dte, OleMenuCommandService mcs)
        {
            CommandID nestAllId = new CommandID(GuidList.guidFileNestingCmdSet, (int)PkgCmdIDList.cmdRunNesting);
            OleMenuCommand menuNestAll = new OleMenuCommand(NestAll, nestAllId);
            mcs.AddCommand(menuNestAll);
        }

        private static void NestAll(object sender, EventArgs e)
        {
            var selected = Helpers.GetSelectedItems();
            List<ProjectItem> items = new List<ProjectItem>(selected);

            foreach (ProjectItem item in selected.Where(i => Directory.Exists(i.FileNames[0])))
                foreach (ProjectItem child in item.ProjectItems)
                {
                    if (File.Exists(child.FileNames[0]))
                        items.Add(child);
                }

            foreach (ProjectItem item in items)
            {
                FileNestingFactory.RunNesting(item);
            }
        }
    }
}