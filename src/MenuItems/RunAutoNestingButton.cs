using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.FileNesting
{
    static class RunAutoNestingButton
    {
        private static DTE2 _dte;

        public static void Register(DTE2 dte, MenuCommandService mcs)
        {
            _dte = dte;
            CommandID nestAllId = new CommandID(PackageGuids.guidFileNestingCmdSet, PackageIds.cmdRunNesting);
            OleMenuCommand menuNestAll = new OleMenuCommand(NestAll, nestAllId);
            mcs.AddCommand(menuNestAll);
        }

        private static void NestAll(object sender, EventArgs e)
        {
            var selected = Helpers.GetSelectedItemsRecursive().Distinct();
            _dte.StatusBar.Text = "Nesting files...";

            foreach (ProjectItem item in selected)
            {
                FileNestingFactory.RunNesting(item);
            }

            _dte.StatusBar.Clear();
            Telemetry.TrackEvent("Run auto-nesting");
        }
    }
}